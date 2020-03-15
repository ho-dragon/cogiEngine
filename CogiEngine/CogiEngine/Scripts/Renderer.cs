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

        public void Render(TextureModel textureModel)
        {
            RawModel model = textureModel.RawModel;
            Gl.BindVertexArray(model.VaoID);
            Gl.EnableVertexAttribArray(0);
            Gl.EnableVertexAttribArray(1);// UV 매핑 데이터 Slot 활성

            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2d, textureModel.Texture.ID);
            
            Gl.DrawElements(PrimitiveType.Triangles, model.VertexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);

            Gl.DisableVertexAttribArray(0);
            Gl.DisableVertexAttribArray(1); // UV 매핑 데이터 Slot 비활성
            Gl.BindVertexArray(0);
        }
    }
    
}