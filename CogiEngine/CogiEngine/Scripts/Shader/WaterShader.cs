using OpenGL;

namespace CogiEngine
{
    public class WaterShader : ShaderProgram
    {
        private const string VERTEX_FILE = "./Resources/Shader/waterVertexShader.txt";
        private const string FRAGMENT_FILE = "./Resources/Shader/waterFragmentShader.txt";

        private int location_modelMatrix;
        private int location_viewMatrix;
        private int location_projectionMatrix;
        private int location_reflectionTexture;
        private int location_refractionTexture;

        public WaterShader() :base(VERTEX_FILE, FRAGMENT_FILE)
        {
            
        }
        
        protected override void BindAttributes()
        {
            
        }

        protected override void GetAllUniformLocations()
        {
            this.location_projectionMatrix = GetUniformLocation("_projectionMatrix");
            this.location_viewMatrix = GetUniformLocation("_viewMatrix");
            this.location_modelMatrix = GetUniformLocation("_modelMatrix");
            this.location_reflectionTexture = GetUniformLocation("_reflectionTexture");
            this.location_refractionTexture = GetUniformLocation("_refractionTexture");
        }

        public void ConnectTextureUnits()
        {
            base.LoadInt(this.location_reflectionTexture, 0);
            base.LoadInt(this.location_refractionTexture, 1);
        }

        public void LoadProjectionMatrix(Matrix4x4f value)
        {
            base.LoadMatrix(this.location_projectionMatrix, value);
        }

        public void LoadViewMatrix(Camera camera)
        {
            Matrix4x4f viewMatrix = Maths.CreateViewMatrix(camera);
            base.LoadMatrix(this.location_viewMatrix, viewMatrix);
        }

        public void loadModelMatrix(Matrix4x4f modelMatrix){
            base.LoadMatrix(this.location_modelMatrix, modelMatrix);
        }
    }
}