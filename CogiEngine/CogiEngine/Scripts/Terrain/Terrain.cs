using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Security.Policy;
using OpenGL;

namespace CogiEngine
{
    public class Terrain
    {
        //https://www.youtube.com/watch?v=h-kZ8dEHIBg
        public const float SIZE = 800;
        private const float MAX_HEIGHT = 40;
        private const float MAX_PIXCEL_COLOR = 256 * 256 * 256; // R * G * B
        private float x;
        private float z;
        private RawModel model;
        private TerrainTexturePack texturePack;
        private TerrainTexture blendMap;
        private int hieghtMapWidth;
        private float[,] heights;

        public float X => x;
        public float Z => z;
        public RawModel Model => model;
        public TerrainTexturePack TexturePack => texturePack;
        public TerrainTexture BlendMap => blendMap;

        public Terrain(float gridX, float gridZ, Loader loader, TerrainTexturePack texturePack, TerrainTexture blendMap, Bitmap hegihtMap)
        {
            this.hieghtMapWidth = hegihtMap.Width;
            this.texturePack = texturePack;
            this.blendMap = blendMap;
            this.x = gridX * SIZE;
            this.z = gridZ * SIZE;
            this.model = GenerateTerrain(loader, hegihtMap);
        }

        public float GetHeightOfTerrain(float worldPositionX, float worldPositonZ)
        {
            float terrainX = worldPositionX - this.x;
            float terrainZ = worldPositonZ - this.z;
            float gridSquareSize = SIZE /  ((float)this.hieghtMapWidth - 1);

            int gridX = (int) Math.Floor(terrainX / gridSquareSize);
            int gridZ = (int) Math.Floor(terrainZ / gridSquareSize);

            if (gridX >= heights.Length - 1 || gridZ >= heights.Length - 1 || gridX < 0 || gridZ < 0)
            {
                return 0f;
            }

            float xCoord = (terrainX % gridSquareSize) / gridSquareSize;
            float zCoord = (terrainZ % gridSquareSize) / gridSquareSize;
            
            float hegiht = 0f;

            if (xCoord <= 1 - zCoord)
            {
                hegiht = Maths.BarryCentric(new Vertex3f(0, heights[gridX, gridZ], 0)
                    , new Vertex3f(1, heights[gridX + 1, gridZ], 0)
                    , new Vertex3f(0, heights[gridX, gridZ + 1], 1)
                    , new Vertex2f(xCoord, zCoord));
            }
            else
            {
                hegiht = Maths.BarryCentric(new Vertex3f(1, heights[gridX + 1, gridZ], 0)
                    , new Vertex3f(1, heights[gridX + 1, gridZ + 1], 1)
                    , new Vertex3f(0, heights[gridX, gridZ + 1], 1)
                    , new Vertex2f(xCoord, zCoord));
            }
            return hegiht;
        }

        private RawModel GenerateTerrain(Loader loader, Bitmap heightMap)
        {
            int vertexCount = heightMap.Height;
            this.heights = new float[vertexCount, vertexCount];

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
                    float height = GetHeight(j, i, heightMap);
                    this.heights[j, i] = height;
                    vertices[vertexPointer * 3] = (float) j / ((float) vertexCount - 1) * SIZE;
                    vertices[vertexPointer * 3 + 1] = height;
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

            return loader.LoadVAO(vertices, textureCoords, normals, indices);
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