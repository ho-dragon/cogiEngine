using System.ComponentModel;
using OpenGL;

namespace CogiEngine
{
    public class MousePicker
    {
        private const int RECURSION_COUNT = 200;
        private const float RAY_RANGE = 600;

        private Vertex3f currentRay = Vertex3f.Zero;

        private Matrix4x4f projectionMatrix;
        private Matrix4x4f viewMatrix;
        private Camera camera;

        private Terrain terrain;
        private Vertex3f currentTerrainPoint;
        private Vertex3f lastTerrainPoint;


        public MousePicker(Camera cam, Matrix4x4f projection, Terrain terrain)
        {
            this.camera = cam;
            this.projectionMatrix = projection;
            this.viewMatrix = Maths.CreateViewMatrix(camera);
            this.terrain = terrain;
        }

        public Vertex3f GetCurrentTerrainPoint()
        {
            return currentTerrainPoint;
        }
        
        public void Update(InputManager input, float screenWidth, float screenHight)
        {
            this.viewMatrix = Maths.CreateViewMatrix(this.camera);
            this.currentRay = CalculateMouseRay(input.MousePosX, input.MousePosY, screenWidth, screenHight);
            
            if (IsInRangeRay(0, RAY_RANGE, currentRay))
            {
                currentTerrainPoint = BinarySearch(0, 0, RAY_RANGE, currentRay);
            }
        }

        private Vertex3f CalculateMouseRay(float mouseX, float mouseY, float screenWidth, float screenHeight)
        {
            Vertex2f normalizedCoords = GetNormalisedDeviceCoordinates(mouseX, mouseY, screenWidth, screenHeight);
            Vertex4f clipCoords = new Vertex4f(normalizedCoords.x, normalizedCoords.y, -1.0f, 1.0f);//우리는 광선의 z가 앞으로 향하게하기를 원합니다. 이것은 일반적으로 OpenGL 스타일에서 음의 z 방향입니다. 우리는 4d 벡터를 갖도록 w 를 추가 할 수 있습니다 .
            Vertex4f eyeCoords = ToEyeCoords(clipCoords);
            Vertex3f worldRay = ToWorldCoords(eyeCoords);
            return worldRay;
        }
        
        private Vertex4f ToEyeCoords(Vertex4f clipCoords)
        {
            Vertex4f eyeCoords = projectionMatrix.Inverse * clipCoords;
            return new Vertex4f(eyeCoords.x, eyeCoords.y, -1f, 0f);// z, w 부분을 ​"지점이 아닌 앞으로"를 의미 하도록 수동으로 설정
        }
        
        private Vertex3f ToWorldCoords(Vertex4f eyeCoords)
        {
            Vertex4f rayWorld = viewMatrix.Inverse * eyeCoords;
            return new Vertex3f(rayWorld.x, rayWorld.y, rayWorld.z).Normalized;//z 구성 요소에 -1을 수동으로 지정 했으므로 광선이 정규화되지 않았기때문에 정규화 필요.
        }

        private Vertex2f GetNormalisedDeviceCoordinates(float mouseX, float mouseY, float screenWidth, float screenHieght)
        {
            float x = (2.0f * mouseX) / screenWidth - 1.0f;
            float y = 1.0f - (2.0f * mouseY) / screenHieght;
            return new Vertex2f(x, y);
        }

        private Vertex3f GetPointOnRay(Vertex3f ray, float distance)
        {
            return camera.Position + ray * distance;
        }

        private Vertex3f BinarySearch(int count, float start, float finish, Vertex3f ray)
        {
            float half = start + (finish - start) / 2f;
            if (count >= RECURSION_COUNT)
            {
                Vertex3f endPoint = GetPointOnRay(ray, half);
                Terrain terrain = GetTerrain(endPoint.x, endPoint.z);
                if (terrain != null)
                {
                    return endPoint;
                }
                else
                {
                    return Vertex3f.Zero;
                }
            }

            if (IsInRangeRay(start, half, ray))
            {
                return BinarySearch(count + 1, start, half, ray);
            }
            else
            {
                return BinarySearch(count + 1, half, finish, ray);
            }
        }

        private bool IsInRangeRay(float start, float finish, Vertex3f ray)
        {
            Vertex3f startPoint = GetPointOnRay(ray, start);
            Vertex3f endPoint = GetPointOnRay(ray, finish);
            if (IsUnderGround(startPoint) == false && IsUnderGround(endPoint))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsUnderGround(Vertex3f testPoint)
        {
            Terrain terrain = GetTerrain(testPoint.x, testPoint.z);
            float height = 0;
            if (terrain != null)
            {
                height = terrain.GetHeightOfTerrain(testPoint.x, testPoint.z);
            }
            
            if (testPoint.y < height)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Terrain GetTerrain(float worldX, float worldZ)
        {
            return terrain;
        }
    }
}