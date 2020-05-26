using System.Collections.Generic;
using OpenGL;

namespace CogiEngine
{
    public class WaterShader : ShaderProgram
    {
        private const int MAX_LIGHT_COUNT = 4;
        
        private const string VERTEX_FILE = "./Resources/Shader/waterVertexShader.txt";
        private const string FRAGMENT_FILE = "./Resources/Shader/waterFragmentShader.txt";

        private int location_transformationMatrix;
        private int location_viewMatrix;
        private int location_projectionMatrix;
        private int location_reflectionTexture;
        private int location_refractionTexture;
        private int location_dudvMap;
        private int location_moveFactor;
        private int location_cameraPosition;
        private int location_normalMap;
        private int[] loccation_lightPosition;
        private int[] location_lightColor;
        private int[] location_attenuation;
        private int location_shineDamper;
        private int location_reflectivity;
        private int location_depthMap;
        
        public WaterShader() :base(VERTEX_FILE, FRAGMENT_FILE)
        {
            
        }
        
        protected override void BindAttributes()
        {
            
        }

        protected override void GetAllUniformLocations()
        {
            this.location_projectionMatrix = base.GetUniformLocation("_projectionMatrix");
            this.location_viewMatrix = base.GetUniformLocation("_viewMatrix");
            this.location_transformationMatrix = base.GetUniformLocation("_transformationMatrix");
            this.location_reflectionTexture = base.GetUniformLocation("_reflectionTexture");
            this.location_refractionTexture = base.GetUniformLocation("_refractionTexture");
            this.location_dudvMap = base.GetUniformLocation("_dudvMap");
            this.location_normalMap = base.GetUniformLocation("_normalMap");
            this.location_moveFactor = base.GetUniformLocation("_moveFactor");
            this.location_cameraPosition = base.GetUniformLocation("_cameraPosition");
            this.location_shineDamper = base.GetUniformLocation("_shineDamper");
            this.location_reflectivity = base.GetUniformLocation("_reflectivity");
            this.location_depthMap = base.GetUniformLocation("_depthMap");
            this.loccation_lightPosition = new int[MAX_LIGHT_COUNT];
            this.location_lightColor = new int[MAX_LIGHT_COUNT];
            this.location_attenuation = new int[MAX_LIGHT_COUNT];
            
            for (int i = 0; i < MAX_LIGHT_COUNT; i++)
            {
                this.loccation_lightPosition[i] = base.GetUniformLocation(string.Format("_lightPosition[{0}]", i));
                this.location_lightColor[i] = base.GetUniformLocation(string.Format("_lightColor[{0}]", i));
                this.location_attenuation[i] = base.GetUniformLocation(string.Format("_lightAttenuation[{0}]", i));
            }
        }

        public void ConnectTextureUnits()
        {
            base.LoadInt(this.location_reflectionTexture, 0);
            base.LoadInt(this.location_refractionTexture, 1);
            base.LoadInt(this.location_dudvMap, 2);
            base.LoadInt(this.location_normalMap, 3);
            base.LoadInt(this.location_depthMap, 4);
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

        public void LoadModelMatrix(Matrix4x4f modelMatrix){
            base.LoadMatrix(this.location_transformationMatrix, modelMatrix);
        }
        
        public void LoadLights(List<Light> lightList)
        {
            for (int i = 0; i < MAX_LIGHT_COUNT; i++)
            {
                if (i < lightList.Count)
                {
                    base.LoadVector3(this.loccation_lightPosition[i], lightList[i].Position);
                    base.LoadVector3(this.location_lightColor[i], lightList[i].Color);
                    base.LoadVector3(this.location_attenuation[i], lightList[i].Attenuation);
                }
                else
                {
                    base.LoadVector3(this.loccation_lightPosition[i], new Vertex3f(0,0,0));
                    base.LoadVector3(this.location_lightColor[i], new Vertex3f(0, 0, 0));
                    base.LoadVector3(this.location_attenuation[i], new Vertex3f(1, 0, 0));
                }
            }
        }
        
        public void LoadShineVariables(float damper, float reflectivity)
        {
            base.LoadFloat(this.location_shineDamper, damper);
            base.LoadFloat(this.location_reflectivity, reflectivity);
        }
    }
}