using System;
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

        float AspectRatio
        {
            get { return (float)clientWidth / (float)clientHeight; }
        }

        public Renderer(StaticShader shader, int width, int height)
        {
            SetViewRect(width, height);
            this.projectionMatrix = Maths.CreateProjectionMatrix(FOV, AspectRatio, NEAR_PLANE, FAR_PLANE);
            shader.Start();
            shader.LoadProjectionMatrix(this.projectionMatrix);
            shader.Stop();
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
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Gl.ClearColor(1, 0, 0, 1f);
        }
        public bool IsEnabledDepthTest()
        {
            ulong[] values = new ulong[1];
            Gl.GetIntegerNV(GetPName.DepthTest, values);
            return values[0] == 1;
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