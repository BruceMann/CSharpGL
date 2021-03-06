﻿using CSharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpGL.Models
{
    /// <summary>
    /// contains a single mesh.
    /// </summary>
    public class ObjMesh
    {
        internal int vertexCount;
        internal int normalCount;
        internal int faceCount;

        internal vec3[] vertexes;
        internal vec3[] normals;
        internal ObjFace[] faces;

        internal vec3 size;
        internal vec3 position;

        internal void CalculateSizePosition()
        {
            throw new NotImplementedException();
        }
    }
}
