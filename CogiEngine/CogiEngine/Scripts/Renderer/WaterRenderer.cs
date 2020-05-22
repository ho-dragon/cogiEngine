using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using CogiEngine.Water;
using OpenGL;

namespace CogiEngine
{
    public class WaterRenderer
    {
        private const string DUDV_TEXTURE_NAME = "waterDUDV";
        private const float WAVE_SPEED = 0.03f;
        private RawModel quad;
        private WaterShader shader;
        private WaterFrameBuffers fbos;
        private uint dudvTexture;
        private float moveFactor = 0f;
        
        public WaterRenderer(Loader loader, WaterShader shader, Matrix4x4f projectionMatrix, WaterFrameBuffers fbos) {
            this.shader = shader;
            this.fbos = fbos;
            this.dudvTexture = loader.LoadRepeatTexture(DUDV_TEXTURE_NAME);
            this.shader.Start();
            this.shader.ConnectTextureUnits();
            this.shader.LoadProjectionMatrix(projectionMatrix);
            this.shader.Stop();
            SetUpVAO(loader);
        }

        public void Render(List<WaterTile> water, Camera camera, float frameTimeSec) {
            PrepareRender(camera, frameTimeSec);
            for (int i = 0; i < water.Count; i++)
            {
                WaterTile tile = water[i];
                Matrix4x4f modelMatrix = Maths.CreateTransformationMatrix(new Vertex3f(tile.X, tile.Height, tile.Z), 0, 0, 0, WaterTile.TILE_SIZE);
                shader.loadModelMatrix(modelMatrix);
                Gl.DrawArrays(PrimitiveType.Triangles, 0, quad.VertexCount);                
            }
            Unbind();
        }
	
        private void PrepareRender(Camera camera, float frameTimeSec){
            this.shader.Start();
            this.shader.LoadViewMatrix(camera);
            
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
        }
	
        private void Unbind(){
            Gl.DisableVertexAttribArray(0);
            Gl.BindVertexArray(0);
            this.shader.Stop();
        }

        private void SetUpVAO(Loader loader) {
            // Just x and z vectex positions here, y is set to 0 in v.shader
            float[] vertices = { -1, -1, -1, 1, 1, -1, 1, -1, -1, 1, 1, 1 };
            quad = loader.LoadVAO(vertices, 2);
        }

    }
}