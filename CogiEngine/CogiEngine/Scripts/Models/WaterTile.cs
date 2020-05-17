namespace CogiEngine
{
    public class WaterTile
    {
        public  const float TILE_SIZE = 60;
	
        private float height;
        private float x,z;

        public float Height => height;

        public float X => x;

        public float Z => z;

        public WaterTile(float centerX, float centerZ, float height){
            this.x = centerX;
            this.z = centerZ;
            this.height = height;
        }
    }
}