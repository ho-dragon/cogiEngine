namespace CogiEngine
{
    public class TerrainTexture
    {
        private uint textureID;

        public uint TextureId => textureID;

        public TerrainTexture(uint textureID)
        {
            this.textureID = textureID;
        }
    }
}