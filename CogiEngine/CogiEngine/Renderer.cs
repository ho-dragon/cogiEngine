using OpenGL;

namespace CogiEngine
{
    public class Renderer
    {
        public void Prepare()
        {
            Gl.ClearColor(1, 0, 0, 1);
            Gl.Clear(ClearBufferMask.ColorBufferBit);
        }

        public void Render(RawModel model)
        {
            Gl.BindVertexArray(model.VaoID);
            Gl.EnableVertexAttribArray(0);

            Gl.DrawArrays(PrimitiveType.Triangles, 0, model.VertexCount);

            Gl.DisableVertexAttribArray(0);
            Gl.BindVertexArray(0);
        }
    }
    
}