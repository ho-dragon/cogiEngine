using System.Collections.Generic;
using CogiEngine.Water;
using OpenGL;

namespace CogiEngine
{
    public class WaterRenderer
    {
        private RawModel quad;
        private WaterShader shader;
        private WaterFrameBuffers fbos;
        
        public WaterRenderer(Loader loader, WaterShader shader, Matrix4x4f projectionMatrix, WaterFrameBuffers fbos) {
            this.shader = shader;
            this.fbos = fbos;
            shader.Start();
            shader.ConnectTextureUnits();
            shader.LoadProjectionMatrix(projectionMatrix);
            shader.Stop();
            SetUpVAO(loader);
        }

        public void Render(List<WaterTile> water, Camera camera) {
            PrepareRender(camera);
            for (int i = 0; i < water.Count; i++)
            {
                WaterTile tile = water[i];
                Matrix4x4f modelMatrix = Maths.CreateTransformationMatrix(new Vertex3f(tile.X, tile.Height, tile.Z), 0, 0, 0, WaterTile.TILE_SIZE);
                shader.loadModelMatrix(modelMatrix);
                Gl.DrawArrays(PrimitiveType.Triangles, 0, quad.VertexCount);                
            }
            Unbind();
        }
	
        private void PrepareRender(Camera camera){
            this.shader.Start();
            shader.LoadViewMatrix(camera);
            Gl.BindVertexArray(quad.VaoID);
            Gl.EnableVertexAttribArray(0);
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2d, this.fbos.ReflectionTexture);
            Gl.ActiveTexture(TextureUnit.Texture1);
            Gl.BindTexture(TextureTarget.Texture2d, this.fbos.RefractionTexture);
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