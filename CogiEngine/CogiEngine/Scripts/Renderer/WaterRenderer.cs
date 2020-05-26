using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using CogiEngine.Water;
using OpenGL;

namespace CogiEngine
{
    public class WaterRenderer
    {
        private const string DUDV_MAP_NAME = "waterDUDV";
        private const string NORMAL_MAP_NAME = "waterNormalMap";
        private const float WAVE_SPEED = 0.03f;
        private RawModel quad;
        private WaterShader shader;
        private WaterFrameBuffers fbos;
        private uint dudvTexture;
        private uint normalMap;
        private float moveFactor = 0f;
        private float shineDamper = 20f;
        private float reflectivity = 0.5f;

        public WaterRenderer(Loader loader, WaterShader shader, Matrix4x4f projectionMatrix, WaterFrameBuffers fbos)
        {
            this.shader = shader;
            this.fbos = fbos;
            this.dudvTexture = loader.LoadRepeatTexture(DUDV_MAP_NAME);
            this.normalMap = loader.LoadRepeatTexture(NORMAL_MAP_NAME);
            this.shader.Start();
            this.shader.ConnectTextureUnits();
            this.shader.LoadProjectionMatrix(projectionMatrix);
            this.shader.Stop();
            SetUpVAO(loader);
        }

        public void Render(List<WaterTile> water, List<Light> lightList, Camera camera, float frameTimeSec)
        {
            PrepareRender(lightList, camera, frameTimeSec);
            for (int i = 0; i < water.Count; i++)
            {
                WaterTile tile = water[i];
                Matrix4x4f modelMatrix = Maths.CreateTransformationMatrix(new Vertex3f(tile.X, tile.Height, tile.Z), 0, 0, 0, WaterTile.TILE_SIZE);
                this.shader.LoadModelMatrix(modelMatrix);
                Gl.DrawArrays(PrimitiveType.Triangles, 0, quad.VertexCount);
            }

            Unbind();
        }

        private void PrepareRender(List<Light> lightList, Camera camera, float frameTimeSec)
        {
            this.shader.Start();
            this.shader.LoadViewMatrix(camera);
            this.shader.LoadShineVariables(this.shineDamper, this.reflectivity);
            this.shader.LoadLights(lightList);

            this.moveFactor += WAVE_SPEED * frameTimeSec;
            this.moveFactor %= 1;
            this.shader.LoadMoveFactor(this.moveFactor);

            Gl.BindVertexArray(quad.VaoID);
            Gl.EnableVertexAttribArray(0);

            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2d, this.fbos.ReflectionTexture);

            Gl.ActiveTexture(TextureUnit.Texture1);
            Gl.BindTexture(TextureTarget.Texture2d, this.fbos.RefractionTexture);

            Gl.ActiveTexture(TextureUnit.Texture2);
            Gl.BindTexture(TextureTarget.Texture2d, this.dudvTexture);

            Gl.ActiveTexture(TextureUnit.Texture3);
            Gl.BindTexture(TextureTarget.Texture2d, this.normalMap);

            Gl.ActiveTexture(TextureUnit.Texture4);
            Gl.BindTexture(TextureTarget.Texture2d, this.fbos.RefractionDepthTexture);

            Gl.Enable(EnableCap.Blend);
            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        private void Unbind()
        {
            Gl.Disable(EnableCap.Blend);
            Gl.DisableVertexAttribArray(0);
            Gl.BindVertexArray(0);
            this.shader.Stop();
        }

        private void SetUpVAO(Loader loader)
        {
            // Just x and z vectex positions here, y is set to 0 in v.shader
            float[] vertices = {-1, -1, -1, 1, 1, -1, 1, -1, -1, 1, 1, 1};
            quad = loader.LoadVAO(vertices, 2);
        }
    }
}