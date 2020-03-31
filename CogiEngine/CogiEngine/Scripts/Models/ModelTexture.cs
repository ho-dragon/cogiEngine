namespace CogiEngine
{
    public class ModelTexture
    {
        private uint textureID;
        private float shineDamper = 1f;
        private float reflectivity = 0f;

        public float ShineDamper
        {
            get => shineDamper;
            set => shineDamper = value;
        }

        public float Reflectivity
        {
            get => reflectivity;
            set => reflectivity = value;
        }

        public uint ID
        {
            get => textureID;
        }

        public ModelTexture(uint id)
        {
            this.textureID = id;
        }
    }
}