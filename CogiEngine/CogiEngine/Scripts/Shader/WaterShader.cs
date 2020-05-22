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
        private int location_dudvMap;
        private int location_moveFactor;
        private int location_cameraPosition;

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
            this.location_dudvMap = GetUniformLocation("_dudvMap");
            this.location_moveFactor = GetUniformLocation("_moveFactor");
            this.location_cameraPosition = GetUniformLocation("_cameraPosition");
        }

        public void ConnectTextureUnits()
        {
            base.LoadInt(this.location_reflectionTexture, 0);
            base.LoadInt(this.location_refractionTexture, 1);
            base.LoadInt(this.location_dudvMap, 2);
        }

        public void LoadMoveFactor(float moveFactor)
        {
            base.LoadFloat(this.location_moveFactor, moveFactor);
        }

        public void LoadProjectionMatrix(Matrix4x4f value)
        {
            base.LoadMatrix(this.location_projectionMatrix, value);
        }

        public void LoadViewMatrix(Camera camera)
        {
            Matrix4x4f viewMatrix = Maths.CreateViewMatrix(camera);
            base.LoadMatrix(this.location_viewMatrix, viewMatrix);
            base.LoadVector3(this.location_cameraPosition, camera.Position);
        }

        public void loadModelMatrix(Matrix4x4f modelMatrix){
            base.LoadMatrix(this.location_modelMatrix, modelMatrix);
        }
    }
}