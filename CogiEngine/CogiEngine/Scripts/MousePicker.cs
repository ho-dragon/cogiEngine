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
            Vertex4f clipCoords = new Vertex4f(normalizedCoords.x, normalizedCoords.y, -1.0f, 1.0f);
            Vertex4f eyeCoords = ToEyeCoords(clipCoords);
            Vertex3f worldRay = ToWorldCoords(eyeCoords);
            return worldRay;
        }

        private Vertex3f ToWorldCoords(Vertex4f eyeCoords)
        {
            Matrix4x4f invertedView = viewMatrix.Inverse;
            Vertex4f rayWorld = Maths.Transform(invertedView, eyeCoords);
            Vertex3f mouseRay = new Vertex3f(rayWorld.x, rayWorld.y, rayWorld.z);
            return mouseRay.Normalized;
        }

        private Vertex4f ToEyeCoords(Vertex4f clipCoords)
        {
            Matrix4x4f invertedProjection = projectionMatrix.Inverse;
            Vertex4f eyeCoords = Maths.Transform(invertedProjection, clipCoords);
            return new Vertex4f(eyeCoords.x, eyeCoords.y, -1f, 0f);
        }

        private Vertex2f GetNormalisedDeviceCoordinates(float mouseX, float mouseY, float screenWidth, float screenHieght)
        {
            float x = (2.0f * mouseX) / screenWidth - 1.0f;
            float y = 1.0f - (2.0f * mouseY) / screenHieght;
            return new Vertex2f(x, y);
        }

        //**********************************************************

        private Vertex3f GetPointOnRay(Vertex3f ray, float distance)
        {
            Vertex3f camPos = camera.Position;
            Vertex3f start = new Vertex3f(camPos.x, camPos.y, camPos.z);
            Vertex3f scaledRay = new Vertex3f(ray.x * distance, ray.y * distance, ray.z * distance);
            return start + scaledRay;
        }

        private Vertex3f BinarySearch(int count, float start, float finish, Vertex3f ray)
        {
            float half = start + ((finish - start) / 2f);
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