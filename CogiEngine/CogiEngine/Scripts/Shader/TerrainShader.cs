using OpenGL;

namespace CogiEngine
{
    public class TerrainShader : ShaderProgram
    {
        private const string VERTEX_FILE_PATH = "./Resources/Shader/terrainVertexShader.txt";
        private const string FRAGMENT_FILE_PATH = "./Resources/Shader/terrainFragmentShader.txt";
        private int locationTransformationMatrix;
        private int locationProjectionMatrix;
        private int locationViewMatrix;
        private int loccationLightPosition;
        private int locationLightColor;
        private int locationShineDamper;
        private int locationReflectivity;
        private int locationSkyColor;
        private int locationBlendMapTexture;
        private int locationBaseTexture;
        private int locationRedTexture;
        private int locationGreenTexture;
        private int locationBlueTexture;
        public TerrainShader() : base(VERTEX_FILE_PATH, FRAGMENT_FILE_PATH)
        {

        }

        protected override void BindAttributes()
        {
        }
        
        protected override void GetAllUniformLocations()
        {
            this.locationTransformationMatrix  = base.GetUniformLocation("_transformationMatrix");
            this.locationProjectionMatrix = base.GetUniformLocation("_projectionMatrix");
            this.locationViewMatrix = base.GetUniformLocation("_viewMatrix");
            this.loccationLightPosition = base.GetUniformLocation("_lightPosition");
            this.locationLightColor = base.GetUniformLocation("_lightColour");
            this.locationShineDamper = base.GetUniformLocation("_shineDamper");
            this.locationReflectivity = base.GetUniformLocation("_reflectivity");
            this.locationSkyColor = base.GetUniformLocation("_skyColor");
            this.locationBlendMapTexture = base.GetUniformLocation("_blendMap");
            this.locationBaseTexture = base.GetUniformLocation("_baseTexture");
            this.locationRedTexture = base.GetUniformLocation("_redTexture");
            this.locationGreenTexture = base.GetUniformLocation("_greenTexture");
            this.locationBlueTexture = base.GetUniformLocation("_blueTexture");
        }

        public void ConnetTextureUnits()
        {
            base.LoadInt(this.locationBlendMapTexture, 0);
            base.LoadInt(this.locationBaseTexture, 1);
            base.LoadInt(this.locationRedTexture, 2);
            base.LoadInt(this.locationGreenTexture, 3);
            base.LoadInt(this.locationBlueTexture, 4);
        }
        
        public void LoadSkyColor(float r, float g, float b)
        {
            base.LoadVector(this.locationSkyColor, new Vertex3f(r, g, b));
        }
        
        public void LoadShineVariables(float damper, float reflectivity)
        {
            base.LoadFloat(this.locationShineDamper, damper);
            base.LoadFloat(this.locationReflectivity, reflectivity);
        }
        
        public void LoadTransformationMatrix(Matrix4x4f value)
        {
            base.LoadMatrix(this.locationTransformationMatrix, value);
        }
        
        public void LoadProjectionMatrix(Matrix4x4f value)
        {
            base.LoadMatrix(this.locationProjectionMatrix, value);
        }
        
        public void LoadViewMatrix(Camera camera)
        {
            Matrix4x4f viewMatrix = Maths.CreateViewMatrix(camera);
            base.LoadMatrix(this.locationViewMatrix, viewMatrix);
        }
        
        public void LoadLight(Light light)
        {
            base.LoadVector(this.loccationLightPosition, light.Position);
            base.LoadVector(this.locationLightColor, light.Colour);
        }
    }
}