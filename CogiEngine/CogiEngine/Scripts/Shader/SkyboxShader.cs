using System.Collections.Generic;
using OpenGL;

namespace CogiEngine
{
    public class SkyboxShader : ShaderProgram
    {
        private const string VERTEX_FILE_PATH = "./Resources/Shader/skyboxVertexShader.txt";
        private const string FRAGMENT_FILE_PATH = "./Resources/Shader/skyboxFragmentShader.txt";

        private int location_projectionMatrix;
        private int location_viewMatrix;
        
        public SkyboxShader() : base(VERTEX_FILE_PATH, FRAGMENT_FILE_PATH)
        {

        }

        protected override void BindAttributes()
        {
            
        }
        
        protected override void GetAllUniformLocations()
        {
            this.location_projectionMatrix = base.GetUniformLocation("_projectionMatrix");
            this.location_viewMatrix = base.GetUniformLocation("_viewMatrix");
        }

        public void LoadProjectionMatrix(Matrix4x4f value)
        {
            base.LoadMatrix(this.location_projectionMatrix, value);
        }

        public void LoadViewMatrix(Camera camera)
        {
            Matrix4x4f viewMatrix = Maths.CreateViewMatrix(camera);
            viewMatrix[3, 0] = 0;
            viewMatrix[3, 1] = 0;
            viewMatrix[3, 2] = 0;
            base.LoadMatrix(this.location_viewMatrix, viewMatrix);
        }
    }
}