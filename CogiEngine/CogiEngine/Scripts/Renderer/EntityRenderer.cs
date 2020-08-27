using System;
using System.Collections.Generic;
using OpenGL;

namespace CogiEngine
{
    public class EntityRenderer
    {
        private StaticShader shader;

        public EntityRenderer(StaticShader shader, int width, int height, Matrix4x4f projectionMatrix)
        {
            this.shader = shader;
            this.shader.Start();
            this.shader.LoadProjectionMatrix(projectionMatrix);
            this.shader.Stop();
        }

        public void Render(Dictionary<TextureModel, List<Entity>> entities)
        {
            var enumerator = entities.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TextureModel model = enumerator.Current.Key;
                PrepareTextureModel(model);
                List<Entity> batch = enumerator.Current.Value;
                for (int i = 0; i < batch.Count; i++)
                {
                    PrepareInstance(batch[i]);
                    Gl.DrawElements(PrimitiveType.Triangles, model.RawModel.VertexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
                }

                Unbind();
            }
        }

        private void PrepareTextureModel(TextureModel model)
        {
            RawModel rawModel = model.RawModel;
            Gl.BindVertexArray(rawModel.VaoID);
            Gl.EnableVertexAttribArray(0); // Position
            Gl.EnableVertexAttribArray(1); // UV 매핑 데이터 Slot 활성
            Gl.EnableVertexAttribArray(2); // Normal

            ModelTexture texture = model.Texture;
            if (texture.HasTransparency)
            {
                MasterRenderer.DisableCulling();
            }

            this.shader.LoadFakeLighting(texture.UseFakeLigihting);
            this.shader.LoadShineVariables(texture.ShineDamper, texture.Reflectivity);
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2d, texture.TextureID);
        }

        private void Unbind()
        {
            MasterRenderer.EnableCulling();
            Gl.DisableVertexAttribArray(0);
            Gl.DisableVertexAttribArray(1); // UV 매핑 데이터 Slot 비활성
            Gl.DisableVertexAttribArray(2);
            Gl.BindVertexArray(0);
        }

        private void PrepareInstance(Entity entity)
        {
            Matrix4x4f transformationMatrix = Maths.CreateTransformationMatrix(entity.Position, entity.RotationX, entity.RotationY, entity.RotationZ, entity.Scale);
            this.shader.LoadTransformationMatrix(transformationMatrix);
            this.shader.LoadAtlasInfo(entity.TextureModel.Texture.NumberOfRows, new Vertex2f(entity.GetTextureOffsetX(), entity.GetTextureOffsetY()));
        }
    }
}