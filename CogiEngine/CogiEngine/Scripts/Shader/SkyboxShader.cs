using System.Collections.Generic;
using System.Security.Permissions;
using OpenGL;

namespace CogiEngine
{
    public class SkyboxShader : ShaderProgram
    {
        private const string VERTEX_FILE_PATH = "./Resources/Shader/skyboxVertexShader.txt";
        private const string FRAGMENT_FILE_PATH = "./Resources/Shader/skyboxFragmentShader.txt";
        
        private const float ROTATE_SPEED = 1f;
        private float currentRotation = 0f;
        
        private int location_projectionMatrix;
        private int location_viewMatrix;
        private int location_fogColor;
        private int location_cubeMapDay;
        private int location_cubeMapNight;
        private int location_blendFactor;
        
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
            this.location_fogColor = base.GetUniformLocation("_fogColor");
            this.location_cubeMapDay = base.GetUniformLocation("_cubeMapDay");
            this.location_cubeMapNight = base.GetUniformLocation("_cubeMapNight");
            this.location_blendFactor = base.GetUniformLocation("_blendFactor");
        }

        public void ConnectTextureUnits()
        {
            base.LoadInt(this.location_cubeMapDay, 0);
            base.LoadInt(this.location_cubeMapNight, 1);
        }

        public void LoadBendFactor(float blendFactor)
        {
            base.LoadFloat(this.location_blendFactor, blendFactor);
        }
        
        public void LoadFogColor(Vertex3f fogColor)
        {
            base.LoadVector3(this.location_fogColor, new Vertex3f(fogColor.x, fogColor.y, fogColor.z));
        }

        public void LoadProjectionMatrix(Matrix4x4f value)
        {
            base.LoadMatrix(this.location_projectionMatrix, value);
        }

        public void LoadViewMatrix(Camera camera, float frameTimeSec)
        {
            Matrix4x4f viewMatrix = Maths.CreateViewMatrix(camera);
            viewMatrix[3, 0] = 0;
            viewMatrix[3, 1] = 0;
            viewMatrix[3, 2] = 0;
            this.currentRotation += ROTATE_SPEED * frameTimeSec;
            viewMatrix.RotateY(this.currentRotation);
            base.LoadMatrix(this.location_viewMatrix, viewMatrix);
        }
    }
}