﻿using System;

namespace CSharpGL
{
    /// <summary>
    /// Create, update, use and delete a framebuffer object.
    /// </summary>
    public partial class Framebuffer : IDisposable
    {
        private static GLDelegates.void_int_uintN glGenFramebuffers;
        private static GLDelegates.void_uint_uint glBindFramebuffer;
        private static GLDelegates.void_uint_uint_uint_uint_int glFramebufferTexture2D;
        //private static OpenGL.glDrawBuffers glDrawBuffers;
        private static GLDelegates.void_uint_uint_uint_uint glFramebufferRenderbuffer;
        private static GLDelegates.void_uint_uint_int glFramebufferParameteri;
        private static GLDelegates.uint_uint glCheckFramebufferStatus;
        private static GLDelegates.void_int_uintN glDeleteFramebuffers;

        private uint[] frameBuffer = new uint[1];

        /// <summary>
        /// Framebuffer Id.
        /// </summary>
        public uint Id { get { return frameBuffer[0]; } }

        /// <summary>
        /// 0 means no renderbuffer attached.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// 0 means no renderbuffer attached.
        /// </summary>
        public int Height { get; private set; }

        static Framebuffer()
        {
            glGenFramebuffers = OpenGL.GetDelegateFor("glGenFramebuffers", GLDelegates.typeof_void_int_uintN) as GLDelegates.void_int_uintN;
            glBindFramebuffer = OpenGL.GetDelegateFor("glBindFramebuffer", GLDelegates.typeof_void_uint_uint) as GLDelegates.void_uint_uint;
            glFramebufferTexture2D = OpenGL.GetDelegateFor("glFramebufferTexture2D", GLDelegates.typeof_void_uint_uint_uint_uint_int) as GLDelegates.void_uint_uint_uint_uint_int;
            glFramebufferRenderbuffer = OpenGL.GetDelegateFor("glFramebufferRenderbuffer", GLDelegates.typeof_void_uint_uint_uint_uint) as GLDelegates.void_uint_uint_uint_uint;
            glFramebufferParameteri = OpenGL.GetDelegateFor("glFramebufferParameteri", GLDelegates.typeof_void_uint_uint_int) as GLDelegates.void_uint_uint_int;
            glCheckFramebufferStatus = OpenGL.GetDelegateFor("glCheckFramebufferStatus", GLDelegates.typeof_uint_uint) as GLDelegates.uint_uint;
            glDeleteFramebuffers = OpenGL.GetDelegateFor("glDeleteFramebuffers", GLDelegates.typeof_void_int_uintN) as GLDelegates.void_int_uintN;
        }

        /// <summary>
        /// Create an empty framebuffer object.
        /// </summary>
        public Framebuffer()
        {
            glGenFramebuffers(1, frameBuffer);
        }

        /// <summary>
        /// start to use this framebuffer.
        /// </summary>
        /// <param name="target"></param>
        public void Bind(FramebufferTarget target = FramebufferTarget.Framebuffer)
        {
            glBindFramebuffer((uint)target, this.Id);
        }

        /// <summary>
        /// stop to use this framebuffer(and use default framebuffer).
        /// </summary>
        /// <param name="target"></param>
        public void Unbind(FramebufferTarget target = FramebufferTarget.Framebuffer)
        {
            glBindFramebuffer((uint)target, 0);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Framebuffer Id: {0}", this.Id);
        }
    }

    /// <summary>
    ///
    /// </summary>
    public enum FramebufferTarget : uint
    {
        /// <summary>
        /// used to draw(write only) something.
        /// </summary>
        DrawFramebuffer = OpenGL.GL_DRAW_FRAMEBUFFER,

        /// <summary>
        /// used to read from(read only).
        /// </summary>
        ReadFramebuffer = OpenGL.GL_READ_FRAMEBUFFER,

        /// <summary>
        /// both read/write.
        /// </summary>
        Framebuffer = OpenGL.GL_FRAMEBUFFER,
    }
}