using System.Drawing;
using OpenGL;

namespace CogiEngine
{
    public class Terrain
    {
        //https://www.youtube.com/watch?v=h-kZ8dEHIBg
        private const float SIZE = 800;
        private const float MAX_HEIGHT = 40;
        private const float MAX_PIXCEL_COLOR = 256 * 256 * 256; // R * G * B
        private float x;
        private float z;
        private RawModel model;
        private TerrainTexturePack texturePack;
        private TerrainTexture blendMap;

        public float X => x;
        public float Z => z;
        public RawModel Model => model;
        public TerrainTexturePack TexturePack => texturePack;
        public TerrainTexture BlendMap => blendMap;

        public Terrain(float gridX, float gridZ, Loader loader, TerrainTexturePack texturePack, TerrainTexture blendMap,
            Bitmap hegihtMapFileName)
        {
            this.texturePack = texturePack;
            this.blendMap = blendMap;
            this.x = gridX * SIZE;
            this.z = gridZ * SIZE;
            this.model = GenerateTerrain(loader, hegihtMapFileName);
        }

        private RawModel GenerateTerrain(Loader loader, Bitmap heightMap)
        {
            int vertexCount = heightMap.Height;
            int count = vertexCount * vertexCount;
            float[] vertices = new float[count * 3];
            float[] normals = new float[count * 3];
            float[] textureCoords = new float[count * 2];
            int[] indices = new int[6 * (vertexCount - 1) * (vertexCount - 1)];

            int vertexPointer = 0;
            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = 0; j < vertexCount; j++)
                {
                    vertices[vertexPointer * 3] = (float) j / ((float) vertexCount - 1) * SIZE;
                    vertices[vertexPointer * 3 + 1] = GetHeight(j, i, heightMap);
                    vertices[vertexPointer * 3 + 2] = (float) i / ((float) vertexCount - 1) * SIZE;

                    Vertex3f normal = GetNormal(j, i, heightMap);
                    normals[vertexPointer * 3] = normal.x;
                    normals[vertexPointer * 3 + 1] = normal.y;
                    normals[vertexPointer * 3 + 2] = normal.z;

                    textureCoords[vertexPointer * 2] = (float) j / ((float) vertexCount - 1);
                    textureCoords[vertexPointer * 2 + 1] = (float) i / ((float) vertexCount - 1);
                    vertexPointer++;
                }
            }

            int pointer = 0;
            for (int gz = 0; gz < vertexCount - 1; gz++)
            {
                for (int gx = 0; gx < vertexCount - 1; gx++)
                {
                    int topLeft = (gz * vertexCount) + gx;
                    int topRight = topLeft + 1;
                    int bottomLeft = ((gz + 1) * vertexCount) + gx;
                    int bottomRight = bottomLeft + 1;
                    indices[pointer++] = topLeft;
                    indices[pointer++] = bottomLeft;
                    indices[pointer++] = topRight;
                    indices[pointer++] = topRight;
                    indices[pointer++] = bottomLeft;
                    indices[pointer++] = bottomRight;
                }
            }

            return loader.LoadToVAO(vertices, textureCoords, normals, indices);
        }

        private Vertex3f GetNormal(int x, int z, Bitmap bitmap)
        {
            float heightLeft = GetHeight(x - 1, z, bitmap);
            float heightRight = GetHeight(x + 1, z, bitmap);
            float heightDown = GetHeight(x, z - 1, bitmap);
            float heightUp = GetHeight(x, z + 1, bitmap);

            Vertex3f normal = new Vertex3f(heightLeft - heightRight, 2f, heightDown - heightUp);
            return normal.Normalized;
        }

        private float GetHeight(int x, int z, Bitmap image)
        {
            if (x < 0 || x >= image.Width || z < 0 || z >= image.Height)
            {
                CogiLogger.Error("[Terrain] heightMap is invalid.");
                return 0f;
            }

            float heightColor = image.GetPixel(x, z).ToArgb();
            heightColor += MAX_PIXCEL_COLOR * 0.5f;
            heightColor /= MAX_PIXCEL_COLOR * 0.5f;
            heightColor *= MAX_HEIGHT;
            return heightColor;
        }
    }
}