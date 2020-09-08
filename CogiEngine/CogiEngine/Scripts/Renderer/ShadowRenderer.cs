using System;
using System.Collections.Generic;
using OpenGL;

namespace CogiEngine
{
    public class ShadowRenderer
    {
        const float near_plane = 1.0f;
        const float far_plane = 1000f;
        private ShadowDepthShader shader;
        private ShadowDepthFrameBuffer depthFrameBuffer;
        private Matrix4x4f lightSpaceMatrix;
        
        public ShadowRenderer(DirectionalLight sun, ShadowDepthShader shader, ShadowDepthFrameBuffer depthFrameBuffer)
        {
            this.shader = shader;
            this.depthFrameBuffer = depthFrameBuffer;

            Matrix4x4f lightProjection = Matrix4x4f.Ortho(-100.0f, 100.0f, -100.0f, 100.0f, near_plane, far_plane);
            Matrix4x4f lightView = Matrix4x4f.LookAt(sun.Position, sun.Position + sun.Direction, Vertex3f.UnitY);
            lightSpaceMatrix = lightProjection * lightView;
            this.shader.Start();
            this.shader.LoadLightSpaceMatrixMatrix(lightSpaceMatrix);
            this.shader.Stop();
        }


        public void Render(Dictionary<TextureModel, List<Entity>> entities)
        {
            //this.depthFrameBuffer.BindFrameBuffer();
            this.shader.Start();
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
            this.shader.Stop();
        }

        private void PrepareTextureModel(TextureModel model)
        {
            RawModel rawModel = model.RawModel;
            Gl.BindVertexArray(rawModel.VaoID);
            Gl.EnableVertexAttribArray(0);// Position
        }

        private void Unbind()
        {
            Gl.DisableVertexAttribArray(0);
            Gl.BindVertexArray(0);
        }
        
        private void PrepareInstance(Entity entity)
        {
            Matrix4x4f transformationMatrix = Maths.CreateTransformationMatrix(entity.Position, entity.RotationX, entity.RotationY, entity.RotationZ, entity.Scale);
            this.shader.LoadModelMatrix(transformationMatrix);
        }
    }
}