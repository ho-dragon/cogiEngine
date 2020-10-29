using OpenGL;

namespace CogiEngine
{
    public class ShadowDepthShader : ShaderProgram
    {
        private const string VERTEX_FILE_PATH = "./Resources/Shader/shadowDepthVertexShader.txt";
        private const string FRAGMENT_FILE_PATH = "./Resources/Shader/shadowDepthFragmentShader.txt";
        public ShadowDepthShader() :base(VERTEX_FILE_PATH, FRAGMENT_FILE_PATH)
        {
            
        }
        
        protected override void BindAttributes()
        {
            
        }

        private int location_lightSpaceMatrix;
        private int location_transformationMatrix;
        protected override void GetAllUniformLocations()
        {
            this.location_lightSpaceMatrix = base.GetUniformLocation("_lightSpaceMatrix");
            this.location_transformationMatrix = base.GetUniformLocation("_transformationMatrix");
        }
        
        public void LoadLightSpaceMatrixMatrix(Matrix4x4f value)
        {
            base.LoadMatrix(this.location_lightSpaceMatrix, value);
        }
        
        public void LoadModelMatrix(Matrix4x4f value)
        {
            base.LoadMatrix(this.location_transformationMatrix, value);
        }
    }
}