using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using OpenGL;

namespace CogiEngine
{
    public class Renderer
    {
        private const float FOV = 70;
        private const float NEAR_PLANE = 0.1f;
        private const float FAR_PLANE = 1000f;
        private Matrix4x4f projectionMatrix = Matrix4x4f.Identity;
        private int clientWidth;
        private int clientHeight;
        private StaticShader shader;

        float AspectRatio
        {
            get { return (float)clientWidth / (float)clientHeight; }
        }

        public Renderer(StaticShader shader, int width, int height)
        {
            this.shader = shader;
            SetViewRect(width, height);
            this.projectionMatrix  = Maths.CreateProjectionMatrix(FOV, AspectRatio, NEAR_PLANE, FAR_PLANE);
            this.shader.Start();
            this.shader.LoadProjectionMatrix(this.projectionMatrix);
            this.shader.Stop();
        }

        public void SetViewRect(int width, int height)
        {
            if (clientWidth == width && clientHeight == height)
            {
                return;
            }
            this.clientWidth = width;
            this.clientHeight = height;

        }

        public void Prepare()
        {
            Gl.Viewport(0, 0, clientWidth, clientHeight);
            Gl.Enable(EnableCap.DepthTest);
            Gl.Enable(EnableCap.CullFace);//Optimize
            Gl.CullFace(CullFaceMode.Front);//Optimize
            
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Gl.ClearColor(0, 0.6f, 0, 1f);
        }
        public bool IsEnabledDepthTest()
        {
            ulong[] values = new ulong[1];
            Gl.GetIntegerNV(GetPName.DepthTest, values);
            return values[0] == 1;
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
                UnbindTextureModel();
            }
        }

        private void PrepareTextureModel(TextureModel model)
        {
            RawModel rawModel = model.RawModel;
            Gl.BindVertexArray(rawModel.VaoID);
            Gl.EnableVertexAttribArray(0);// Position
            Gl.EnableVertexAttribArray(1);// UV 매핑 데이터 Slot 활성
            Gl.EnableVertexAttribArray(2);// Normal
            
            ModelTexture texture = model.Texture;
            this.shader.LoadShineVariables(texture.ShineDamper, texture.Reflectivity);
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2d, model.Texture.ID);
        }

        private void UnbindTextureModel()
        {
            Gl.DisableVertexAttribArray(0);
            Gl.DisableVertexAttribArray(1); // UV 매핑 데이터 Slot 비활성
            Gl.DisableVertexAttribArray(2);
            Gl.BindVertexArray(0);
        }

        private void PrepareInstance(Entity entity)
        {
            Matrix4x4f transformationMatrix = Maths.CreateTransformationMatrix(entity.Position, entity.RotX, entity.RotY, entity.RotZ, entity.Scale);
            this.shader.LoadTransformationMatrix(transformationMatrix);
        }
    }
}