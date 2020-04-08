using System;
using System.Collections.Generic;
using OpenGL;

namespace CogiEngine
{
    public class TerrainRenderer
    {
        private TerrainShader shader;

        public TerrainRenderer(TerrainShader shader, Matrix4x4f projectionMatrix)
        {
            this.shader = shader;
            this.shader.Start();
            this.shader.LoadProjectionMatrix(projectionMatrix);
            this.shader.Stop();
        }

        public void Render(List<Terrain> terrains)
        {
            for (int i = 0; i < terrains.Count; i++)
            {
                Terrain terrain = terrains[i];
                PrepareTerrain(terrain);
                LoadModelMatrix(terrain);
                Gl.DrawElements(PrimitiveType.Triangles, terrain.Model.VertexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
                Unbind();
            }
        }
        
        private void PrepareTerrain(Terrain terrain)
        {
            RawModel rawModel = terrain.Model;
            Gl.BindVertexArray(rawModel.VaoID);
            Gl.EnableVertexAttribArray(0);// Position
            Gl.EnableVertexAttribArray(1);// UV 매핑 데이터 Slot 활성
            Gl.EnableVertexAttribArray(2);// Normal
            
            ModelTexture texture = terrain.Texture;
            this.shader.LoadShineVariables(texture.ShineDamper, texture.Reflectivity);
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2d, texture.ID);
        }

        private void Unbind()
        {
            Gl.DisableVertexAttribArray(0);
            Gl.DisableVertexAttribArray(1); // UV 매핑 데이터 Slot 비활성
            Gl.DisableVertexAttribArray(2);
            Gl.BindVertexArray(0);
        }
        
        private void LoadModelMatrix(Terrain terrain)
        {
            Matrix4x4f transformationMatrix = Maths.CreateTransformationMatrix(new Vertex3f(terrain.X, 0, terrain.Z), 0, 0, 0, 1f);
            this.shader.LoadTransformationMatrix(transformationMatrix);
        }
    }
}