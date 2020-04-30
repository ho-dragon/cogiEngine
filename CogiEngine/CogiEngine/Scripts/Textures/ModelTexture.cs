namespace CogiEngine
{
    public class ModelTexture
    {
        private uint textureID;
        private float shineDamper = 1f;
        private float reflectivity = 0f;
        private bool hasTransparency = false;
        private bool useFakeLigihting = false;
        private int numberOfRows = 1;

        public int NumberOfRows
        {
            get => numberOfRows;
            set => numberOfRows = value;
        }

        public float ShineDamper
        {
            get => shineDamper;
            set => shineDamper = value;
        }

        public bool UseFakeLigihting
        {
            get => useFakeLigihting;
            set => useFakeLigihting = value;
        }

        public float Reflectivity
        {
            get => reflectivity;
            set => reflectivity = value;
        }

        public uint TextureID
        {
            get => textureID;
        }

        public bool HasTransparency
        {
            get => hasTransparency;
            set => hasTransparency = value;
        }

        public ModelTexture(uint textureId)
        {
            this.textureID = textureId;
        }
    }
}