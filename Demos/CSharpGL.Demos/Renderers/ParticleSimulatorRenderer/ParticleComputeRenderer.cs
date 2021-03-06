﻿using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CSharpGL.Demos
{
    internal class ParticleComputeRenderer : RendererBase
    {
        private ShaderProgram computeProgram;

        //private uint[] textureBufferPosition = new uint[1];
        //private uint[] textureBufferVelocity = new uint[1];
        private Texture positionTexture;

        private Texture velocityTexture;

        //private uint[] attractor_buffer = new uint[1];
        private GLBuffer attractorBuffer;

        private GLBuffer positionBuffer;
        private GLBuffer velocityBuffer;
        private float time = 0;
        private Random random = new Random();

        private static readonly GLDelegates.void_uint_uint_uint glBindBufferBase;
        private static readonly GLDelegates.void_uint_uint_int_bool_int_uint_uint glBindImageTexture;
        static ParticleComputeRenderer()
        {
            glBindBufferBase = OpenGL.GetDelegateFor("glBindBufferBase", GLDelegates.typeof_void_uint_uint_uint) as GLDelegates.void_uint_uint_uint;
            glBindImageTexture = OpenGL.GetDelegateFor("glBindImageTexture", GLDelegates.typeof_void_uint_uint_int_bool_int_uint_uint) as GLDelegates.void_uint_uint_int_bool_int_uint_uint;
        }

        public ParticleComputeRenderer(GLBuffer positionBuffer, GLBuffer velocityBuffer)
        {
            this.positionBuffer = positionBuffer;
            this.velocityBuffer = velocityBuffer;
        }

        protected override void DoInitialize()
        {
            {
                // particleSimulator-fountain.comp is also OK.
                var shaderCode = new ShaderCode(File.ReadAllText(@"shaders\ParticleSimulatorRenderer\particleSimulator.comp"), ShaderType.ComputeShader);
                this.computeProgram = shaderCode.CreateProgram();
            }
            {
                GLBuffer buffer = this.positionBuffer;
                Texture texture = buffer.DumpBufferTexture(OpenGL.GL_RGBA32F, autoDispose: false);
                texture.Initialize();
                this.positionTexture = texture;
            }
            {
                GLBuffer buffer = this.velocityBuffer;
                Texture texture = buffer.DumpBufferTexture(OpenGL.GL_RGBA32F, autoDispose: false);
                texture.Initialize();
                this.velocityTexture = texture;
            }
            {
                const int length = 64;
                UniformBuffer buffer = UniformBuffer.Create(typeof(vec4), length, BufferUsage.DynamicCopy);
                buffer.Bind();
                glBindBufferBase((uint)BindBufferBaseTarget.UniformBuffer, 0, buffer.BufferId);
                buffer.Unbind();
                this.attractorBuffer = buffer;

                OpenGL.CheckError();
            }
        }

        protected override void DoRender(RenderEventArgs arg)
        {
            float deltaTime = (float)random.NextDouble() * 5;
            time += (float)random.NextDouble() * 5;

            IntPtr attractors = this.attractorBuffer.MapBufferRange(
                0, 64 * Marshal.SizeOf(typeof(vec4)),
                MapBufferRangeAccess.MapWriteBit | MapBufferRangeAccess.MapInvalidateBufferBit);
            unsafe
            {
                var array = (vec4*)attractors.ToPointer();
                for (int i = 0; i < 64; i++)
                {
                    array[i] = new vec4(
                        (float)(Math.Sin(time)) * 50.0f,
                        (float)(Math.Cos(time)) * 50.0f,
                        (float)(Math.Cos(time)) * (float)(Math.Sin(time)) * 5.0f,
                        ParticleModel.attractor_masses[i]);
                }
            }
            this.attractorBuffer.UnmapBuffer();

            // Activate the compute program and bind the position and velocity buffers
            computeProgram.Bind();
            glBindImageTexture(0, this.velocityTexture.Id, 0, false, 0, OpenGL.GL_READ_WRITE, OpenGL.GL_RGBA32F);
            glBindImageTexture(1, this.positionTexture.Id, 0, false, 0, OpenGL.GL_READ_WRITE, OpenGL.GL_RGBA32F);
            // Set delta time
            computeProgram.glUniform("dt", deltaTime);
            // Dispatch
            OpenGL.GetDelegateFor<OpenGL.glDispatchCompute>()(ParticleModel.particleGroupCount, 1, 1);
        }

        protected override void DisposeUnmanagedResources()
        {
            this.computeProgram.Dispose();
            this.positionTexture.Dispose();
            this.velocityTexture.Dispose();
            this.attractorBuffer.Dispose();
        }
    }
}