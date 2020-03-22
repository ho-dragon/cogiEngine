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

        public void Render(Entity entity, StaticShader shader)
        {
            TextureModel model = entity.Model;
            RawModel rawModel = model.RawModel;
            
            Gl.BindVertexArray(rawModel.VaoID);
            Gl.EnableVertexAttribArray(0);
            Gl.EnableVertexAttribArray(1);// UV 매핑 데이터 Slot 활성

            Matrix4x4f transformationMatrix = Maths.CreateTransformationMatrix(entity.Position, entity.RotX, entity.RotY, entity.RotZ, entity.Scale);
            shader.LoadTransformationMatrix(transformationMatrix);
            
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2d, model.Texture.ID);
            
            Gl.DrawElements(PrimitiveType.Triangles, rawModel.VertexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);

            Gl.DisableVertexAttribArray(0);
            Gl.DisableVertexAttribArray(1); // UV 매핑 데이터 Slot 비활성
            Gl.BindVertexArray(0);
        }
    }
    
}