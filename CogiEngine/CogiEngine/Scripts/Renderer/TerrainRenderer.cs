using System;
using System.Collections.Generic;
using OpenGL;

namespace CogiEngine
{
    public class TerrainRenderer
    {
        private TerrainShader shader;
        private const float SHINE_DAMPER = 1f;
        private const float REFLECTIVITY = 0f;

        public TerrainRenderer(TerrainShader shader, Matrix4x4f projectionMatrix)
        {
            this.shader = shader;
            this.shader.Start();
            this.shader.LoadProjectionMatrix(projectionMatrix);
            this.shader.ConnetTextureUnits();
            this.shader.Stop();
        }

        public void Render(List<Terrain> terrains)
        {
            for (int i = 0; i < terrains.Count; i++)
            {
                Terrain terrain = terrains[i];
                PrepareTerrain(terrain);
                LoadModelMatrix(terrain);
                Gl.DrawElements(PrimitiveType.TriangleStrip, terrain.Model.VertexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
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
            BindTextures(terrain);
            this.shader.LoadShineVariables(SHINE_DAMPER, REFLECTIVITY);
        }

        private void BindTextures(Terrain terrain)
        {
            TerrainTexturePack texturePack = terrain.TexturePack;
            
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2d, terrain.BlendMap.TextureId);
            
            Gl.ActiveTexture(TextureUnit.Texture1);
            Gl.BindTexture(TextureTarget.Texture2d, texturePack.BaseTexture.TextureId);
            
            Gl.ActiveTexture(TextureUnit.Texture2);
            Gl.BindTexture(TextureTarget.Texture2d, texturePack.RedTexture.TextureId);
            
            Gl.ActiveTexture(TextureUnit.Texture3);
            Gl.BindTexture(TextureTarget.Texture2d, texturePack.GreenTexture.TextureId);
            
            Gl.ActiveTexture(TextureUnit.Texture4);
            Gl.BindTexture(TextureTarget.Texture2d, texturePack.BlueTexture.TextureId);
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