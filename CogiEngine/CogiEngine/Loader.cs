using System;
using System.Collections.Generic;
using OpenGL;

namespace CogiEngine
{
    public class Loader
    {
        List<uint> vaoList = new List<uint>();
        List<uint> vboList = new List<uint>();

        public RawModel loadToVAO(float [] positions)
        {
            uint vaoID = CreateVAO();
            StoreDataInAttributeList(0, positions);
            UnbindVAO();
            return new RawModel(vaoID, positions.Length / 3);
        }

        public void CleanUp()
        {
            Gl.DeleteVertexArrays(vaoList.ToArray());
            Gl.DeleteBuffers(vboList.ToArray());
        }

        uint CreateVAO()
        {
            uint vaoID = Gl.GenVertexArray();
            vaoList.Add(vaoID);
            Gl.BindVertexArray(vaoID);
            return vaoID;
        }

        void StoreDataInAttributeList(uint attributeNumber, float [] data)
        {
            uint vboID = Gl.GenBuffer();
            vboList.Add(vboID);

            Gl.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)(data.Length * sizeof(float)), data, BufferUsage.StaticDraw);

            Gl.VertexAttribPointer(attributeNumber, 3, VertexAttribType.Float, false, 0, IntPtr.Zero);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        void UnbindVAO()
        {
            Gl.BindVertexArray(0);
        }
    }
}