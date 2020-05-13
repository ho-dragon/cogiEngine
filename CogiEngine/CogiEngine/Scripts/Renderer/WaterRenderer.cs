using System.Collections.Generic;
using OpenGL;

namespace CogiEngine
{
    public class WaterRenderer
    {
        private RawModel quad;
        private WaterShader shader;

        public WaterRenderer(Loader loader, WaterShader shader, Matrix4x4f projectionMatrix) {
            this.shader = shader;
            shader.Start();
            shader.LoadProjectionMatrix(projectionMatrix);
            shader.Stop();
            setUpVAO(loader);
        }

        public void render(List<WaterTile> water, Camera camera) {
            prepareRender(camera);
            for (int i = 0; i < water.Count; i++)
            {
                WaterTile tile = water[i];
                Matrix4x4f modelMatrix = Maths.CreateTransformationMatrix(new Vertex3f(tile.getX(), tile.getHeight(), tile.getZ()), 0, 0, 0, WaterTile.TILE_SIZE);
                shader.loadModelMatrix(modelMatrix);
                Gl.DrawArrays(PrimitiveType.Triangles, 0, quad.VertexCount);                
            }
            unbind();
        }
	
        private void prepareRender(Camera camera){
            this.shader.Start();
            shader.LoadViewMatrix(camera);
            Gl.BindVertexArray(quad.VaoID);
            Gl.EnableVertexAttribArray(0);
        }
	
        private void unbind(){
            Gl.DisableVertexAttribArray(0);
            Gl.BindVertexArray(0);
            this.shader.Stop();
        }

        private void setUpVAO(Loader loader) {
            // Just x and z vectex positions here, y is set to 0 in v.shader
            float[] vertices = { -1, -1, -1, 1, 1, -1, 1, -1, -1, 1, 1, 1 };
            quad = loader.LoadVAO(vertices, 2);
        }

    }
}