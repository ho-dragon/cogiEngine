using System.Collections.Generic;
using System.Numerics;
using OpenGL;

namespace CogiEngine
{
    public class TerrainShader : ShaderProgram
    {
        private const int MAX_LIGHT_COUNT = 4;
        private const string VERTEX_FILE_PATH = "./Resources/Shader/terrainVertexShader.txt";
        private const string FRAGMENT_FILE_PATH = "./Resources/Shader/terrainFragmentShader.txt";
        private int location_transformationMatrix;
        private int location_projectionMatrix;
        private int location_viewMatrix;
        private int location_lightViewMatrix;
        private int[] loccation_lightPosition;
        private int[] location_lightColor;
        private int[] location_attenuation;
        private int location_shineDamper;
        private int location_reflectivity;
        private int location_skyColor;
        private int location_shadowMap;
        private int location_blendMapTexture;
        private int location_baseTexture;
        private int location_redTexture;
        private int location_greenTexture;
        private int location_blueTexture;
        private int location_waterClippingPlane;


        public TerrainShader() : base(VERTEX_FILE_PATH, FRAGMENT_FILE_PATH)
        {
        }

        protected override void BindAttributes()
        {
        }

        protected override void GetAllUniformLocations()
        {
            this.location_transformationMatrix = base.GetUniformLocation("_transformationMatrix");
            this.location_projectionMatrix = base.GetUniformLocation("_projectionMatrix");
            this.location_viewMatrix = base.GetUniformLocation("_viewMatrix");
            this.location_lightViewMatrix = base.GetUniformLocation("_lightSpaceMatrix");
            this.location_shineDamper = base.GetUniformLocation("_shineDamper");
            this.location_reflectivity = base.GetUniformLocation("_reflectivity");
            this.location_skyColor = base.GetUniformLocation("_skyColor");
            this.location_shadowMap = base.GetUniformLocation("_shadowMap");
            this.location_blendMapTexture = base.GetUniformLocation("_blendMap");
            this.location_baseTexture = base.GetUniformLocation("_baseTexture");
            this.location_redTexture = base.GetUniformLocation("_redTexture");
            this.location_greenTexture = base.GetUniformLocation("_greenTexture");
            this.location_blueTexture = base.GetUniformLocation("_blueTexture");
            this.location_waterClippingPlane = base.GetUniformLocation("_waterClippingPlane");

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

        public void LoadClipPlane(Vertex4f plane)
        {
            base.LoadVector4(this.location_waterClippingPlane, plane);
        }
        
        public void ConnetTextureUnits()
        {
            base.LoadInt(this.location_blendMapTexture, 0);
            base.LoadInt(this.location_baseTexture, 1);
            base.LoadInt(this.location_redTexture, 2);
            base.LoadInt(this.location_greenTexture, 3);
            base.LoadInt(this.location_blueTexture, 4);
            base.LoadInt(this.location_shadowMap, 5);
        }

        public void LoadSkyColor(float r, float g, float b)
        {
            base.LoadVector3(this.location_skyColor, new Vertex3f(r, g, b));
        }

        public void LoadShineVariables(float damper, float reflectivity)
        {
            base.LoadFloat(this.location_shineDamper, damper);
            base.LoadFloat(this.location_reflectivity, reflectivity);
        }

        public void LoadTransformationMatrix(Matrix4x4f value)
        {
            base.LoadMatrix(this.location_transformationMatrix, value);
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

        public void LoadLightViewMatrix(Matrix4x4f value)
        {
            base.LoadMatrix(this.location_lightViewMatrix, value);
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
                    base.LoadVector3(this.loccation_lightPosition[i], new Vertex3f(0, 0, 0));
                    base.LoadVector3(this.location_lightColor[i], new Vertex3f(0, 0, 0));
                    base.LoadVector3(this.location_attenuation[i], new Vertex3f(1, 0, 0));
                }
            }
        }
    }
}