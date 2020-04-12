using System.Net.NetworkInformation;

namespace CogiEngine
{
    public class TerrainTexturePack
    {
        private TerrainTexture baseTexture;
        private TerrainTexture redTexture;
        private TerrainTexture greenTexture;
        private TerrainTexture blueTexture;

        public TerrainTexturePack(TerrainTexture baseTexture, TerrainTexture redTexture, TerrainTexture greenTexture, TerrainTexture blueTexture)
        {
            this.baseTexture = baseTexture;
            this.redTexture = redTexture;
            this.greenTexture = greenTexture;
            this.blueTexture = blueTexture;
        }

        public TerrainTexture BaseTexture => baseTexture;

        public TerrainTexture RedTexture => redTexture;

        public TerrainTexture GreenTexture => greenTexture;

        public TerrainTexture BlueTexture => blueTexture;
    }
}