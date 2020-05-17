using System.Collections.Generic;
using System.Security.Permissions;
using OpenGL;

namespace CogiEngine
{
    public class StaticShader : ShaderProgram
    {
        private const int MAX_LIGHT_COUNT = 4;
        private const string VERTEX_FILE_PATH = "./Resources/Shader/vertexShader.txt";
        private const string FRAGMENT_FILE_PATH = "./Resources/Shader/fragmentShader.txt";
        private int location_transformationMatrix;
        private int location_projectionMatrix;
        private int location_viewMatrix;
        private int[] loccation_lightPosition;
        private int[] location_lightColor;
        private int[] location_attenuation;
        private int location_shineDamper;
        private int location_reflectivity;
        private int location_useFakeLighting;
        private int location_skyColor;
        private int location_numberOfRows;
        private int location_offset;
        private int location_waterClippingPlane;
        
        public StaticShader() : base(VERTEX_FILE_PATH, FRAGMENT_FILE_PATH)
        {

        }
        
        protected override void BindAttributes()
        {
            //base.BindAttribute(0, "_position");
            //base.BindAttribute(1, "_textureCoords");
            #region Desc
            /*
            바인딩은 0번 위치에 "position"이라는 이름으로 하고 있습니다. 이것은 vertexShader의 소스 코드를 보면 이해할 수 있습니다.
            위의 소스 코드에 보면 "_position" 이름이 나오는데 원래 저 변수는 다음과 같이 선언한 것을 줄인 것입니다.
            
            layout(location = 0) in vec3 _position;
            
            다시 말해, 이름은 달라도 되지만 location으로 바인딩한 숫자는 틀리면 안 됩니다.
            그렇긴 해도 이름 역시 맞춰주는 것이 일관성을 위해 좋을 것입니다.
            만약 이름을 기준으로 location 위치를 동적으로 구하고 싶다면 다음과 같은 식으로 Gl.GetAttribLocation 메서드를 이용할 수 있습니다.
            
            protected void bindAttribute(...)
            {
                uint id = (uint)Gl.GetAttribLocation(_programID, "_position"); // vertex shader 코드의 변수 중 "_position"에 대한 location 값을 반환
                Gl.BindAttribLocation(_programID, id, "_position");
            }
            */
            #endregion
        }

        protected override void GetAllUniformLocations()
        {
            this.location_transformationMatrix = base.GetUniformLocation("_transformationMatrix");
            this.location_projectionMatrix = base.GetUniformLocation("_projectionMatrix");
            this.location_viewMatrix = base.GetUniformLocation("_viewMatrix");
            this.location_shineDamper = base.GetUniformLocation("_shineDamper");
            this.location_reflectivity = base.GetUniformLocation("_reflectivity");
            this.location_useFakeLighting = base.GetUniformLocation("_useFakeLighting");
            this.location_skyColor = base.GetUniformLocation("_skyColor");
            this.location_numberOfRows = base.GetUniformLocation("_numberOfRows");
            this.location_offset = base.GetUniformLocation("_offset");
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

        public void LoadAtlasInfo(int numberOfRows, Vertex2f offset)
        {
            base.LoadInt(this.location_numberOfRows, numberOfRows);
            base.LoadVector2(this.location_offset, offset);
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

        public void LoadFakeLighting(bool useFakeLighting)
        {
            base.LoadBoolean(this.location_useFakeLighting, useFakeLighting);
        }
    }
}