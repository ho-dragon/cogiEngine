using OpenGL;

namespace CogiEngine
{
    public class GUIShader : ShaderProgram
    {
        private const string VERTEX_FILE_PATH = "./Resources/Shader/guiVertexShader.txt";
        private const string FRAGMENT_FILE_PATH = "./Resources/Shader/guiFragmentShader.txt";

        private int location_transformationMatrix;

        public GUIShader() : base(VERTEX_FILE_PATH, FRAGMENT_FILE_PATH)
        {

        }
	
        public void LoadTransformationMatrix(Matrix4x4f value)
        {
            base.LoadMatrix(this.location_transformationMatrix, value);
        }


        protected override void GetAllUniformLocations()
        {
            this.location_transformationMatrix = base.GetUniformLocation("_transformationMatrix");
        }

        protected override void BindAttributes()
        {
            
        }
    }
}