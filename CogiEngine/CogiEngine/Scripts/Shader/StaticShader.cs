using OpenGL;

namespace CogiEngine
{
    public class StaticShader : ShaderProgram
    {
        private const string VERTEX_FILE_PATH = "./Resources/Shader/vertexShader.txt";
        private const string FRAGMENT_FILE_PATH = "./Resources/Shader/fragmentShader.txt";
        private int locationTransformationMatrix;
        private int locationProjectionMatrix;
        private int locationViewMatrix;
        private int loccationLightPosition;
        private int locationLightColor;
        private int locationShineDamper;
        private int locationReflectivity;
        private int locationUseFakeLighting;
        private int locationSkyColor;
        
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
            this.locationTransformationMatrix  = base.GetUniformLocation("_transformationMatrix");
            this.locationProjectionMatrix = base.GetUniformLocation("_projectionMatrix");
            this.locationViewMatrix = base.GetUniformLocation("_viewMatrix");
            this.loccationLightPosition = base.GetUniformLocation("_lightPosition");
            this.locationLightColor = base.GetUniformLocation("_lightColor");
            this.locationShineDamper = base.GetUniformLocation("_shineDamper");
            this.locationReflectivity = base.GetUniformLocation("_reflectivity");
            this.locationUseFakeLighting = base.GetUniformLocation("_useFakeLighting");
            this.locationSkyColor = base.GetUniformLocation("_skyColor");
        }

        public void LoadSkyColor(float r, float g, float b)
        {
            base.LoadVector(this.locationSkyColor, new Vertex3f(r, g, b));
        }
        
        public void LoadShineVariables(float damper, float reflectivity)
        {
            base.LoadFloat(this.locationShineDamper, damper);
            base.LoadFloat(this.locationReflectivity, reflectivity);
        }
        
        public void LoadTransformationMatrix(Matrix4x4f value)
        {
            base.LoadMatrix(this.locationTransformationMatrix, value);
        }
        
        public void LoadProjectionMatrix(Matrix4x4f value)
        {
            base.LoadMatrix(this.locationProjectionMatrix, value);
        }
        
        public void LoadViewMatrix(Camera camera)
        {
            Matrix4x4f viewMatrix = Maths.CreateViewMatrix(camera);
            base.LoadMatrix(this.locationViewMatrix, viewMatrix);
        }
        
        public void LoadLight(Light light)
        {
            base.LoadVector(this.loccationLightPosition, light.Position);
            base.LoadVector(this.locationLightColor, light.Colour);
        }

        public void LoadFakeLighting(bool useFakeLighting)
        {
            base.LoadBoolean(this.locationUseFakeLighting, useFakeLighting);
        }
    }
}