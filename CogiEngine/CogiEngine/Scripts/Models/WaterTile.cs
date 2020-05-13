namespace CogiEngine
{
    public class WaterTile
    {
        public  const float TILE_SIZE = 60;
	
        private float height;
        private float x,z;
	
        public WaterTile(float centerX, float centerZ, float height){
            this.x = centerX;
            this.z = centerZ;
            this.height = height;
        }

        public float getHeight() {
            return height;
        }

        public float getX() {
            return x;
        }

        public float getZ() {
            return z;
        }
    }
}