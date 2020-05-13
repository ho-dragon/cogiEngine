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

        public WaterShader() :base(VERTEX_FILE, FRAGMENT_FILE)
        {
            
        }
        
        protected override void BindAttributes()
        {
            
        }

        protected override void GetAllUniformLocations()
        {
            location_projectionMatrix = GetUniformLocation("_projectionMatrix");
            location_viewMatrix = GetUniformLocation("_viewMatrix");
            location_modelMatrix = GetUniformLocation("_modelMatrix");
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