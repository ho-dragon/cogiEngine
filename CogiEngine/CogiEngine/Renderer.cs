using System;
using OpenGL;

namespace CogiEngine
{
    public class Renderer
    {
        public void Prepare()
        {
            Gl.Clear(ClearBufferMask.ColorBufferBit);
            Gl.ClearColor(0, 0, 1, 1);
        }

        public void Render(RawModel model)
        {
            Gl.BindVertexArray(model.VaoID);
            Gl.EnableVertexAttribArray(0);

            Gl.DrawElements(PrimitiveType.Triangles, model.VertexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);

            Gl.DisableVertexAttribArray(0);
            Gl.BindVertexArray(0);
        }
    }
    
}