using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using OpenGL;

namespace CogiEngine
{
    public class GUIRenderer
    {
        private RawModel quadModel;
        private GUIShader shader;

        public GUIRenderer(Loader loader)
        {
            float[] positions =
            {
                -1, 1,//V1
                -1, -1,//V2
                1, 1,//V3
                1, -1//V4
            };
            this.quadModel = loader.LoadVAO(positions, 2);
            this.shader = new GUIShader();
        }

        public void Render(List<GUITexture> guiList)
        {
            this.shader.Start();
            Gl.BindVertexArray(this.quadModel.VaoID);
            Gl.EnableVertexAttribArray(0);// Position
            Gl.Enable(EnableCap.Blend);
            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            Gl.Disable(EnableCap.DepthTest);
            for (int i = 0; i < guiList.Count; i++)
            {
                Gl.ActiveTexture(TextureUnit.Texture0);
                Gl.BindTexture(TextureTarget.Texture2d, guiList[i].TextureID);
                Matrix4x4f transformationMatrix = Maths.CreateTransformationMatrix(guiList[i].Position, guiList[i].Scale);
                this.shader.LoadTransformationMatrix(transformationMatrix);
                Gl.DrawArrays(PrimitiveType.TriangleStrip, 0, this.quadModel.VertexCount);
            }
            Gl.Enable(EnableCap.DepthTest);
            Gl.Disable(EnableCap.Blend);
            Gl.DisableVertexAttribArray(0);
            Gl.BindVertexArray(0);
            this.shader.Stop();
        }

        public void CleanUp()
        {
            this.shader.CleanUp();
        }
    }
}