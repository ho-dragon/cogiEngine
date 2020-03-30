using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Assimp;
using Assimp.Configs;
using OpenGL;

namespace CogiEngine
{
    /// <summary>
    /// OBJ파일을 Read한 후 vectex, uv, normal리스트를 순서대로 재정렬하기
    /// </summary>
    public class OBJLoader
    {

        public static RawModel LoadObjModelFromAssimp(string fileName, Loader loader)
        {
            string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources", fileName + ".obj");
            AssimpContext importer = new AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            Scene scene = importer.ImportFile(filePath, PostProcessPreset.TargetRealTimeQuality | PostProcessSteps.FlipWindingOrder);
            if (scene == null || scene.HasMeshes == false)
            {
                return null;
            }
            float[] verticesArray = VerticesFromMesh(scene.Meshes[0]);
            float[] textureArray = TextureFromMesh(scene.Meshes[0]);
            int[] indicesArray = IndicesFromMesh(scene.Meshes[0]);
            float[] normalsArray = NormalsFromMesh(scene.Meshes[0]);
            return loader.LoadToVAO(verticesArray, textureArray, normalsArray, indicesArray);
        }
        
        static float[] TextureFromMesh(Mesh mesh)
        {
            List<float> list = new List<float>();
            foreach (Vector3D vertex in mesh.TextureCoordinateChannels[0])
            {
                list.Add(vertex.X);
                list.Add(1 - vertex.Y);
            }
            return list.ToArray();
        }

        static float[] VerticesFromMesh(Mesh mesh)
        {
            List<float> list = new List<float>();
            foreach (Vector3D vertex in mesh.Vertices)
            {
                list.Add(vertex.X);
                list.Add(vertex.Y);
                list.Add(vertex.Z);
            }
            return list.ToArray();
        }

        static int[] IndicesFromMesh(Mesh mesh)
        {
            List<int> list = new List<int>();
            foreach (Face face in mesh.Faces)
            {
                list.AddRange(face.Indices);
            }
            return list.ToArray();
        }
        
        static float[] NormalsFromMesh(Mesh mesh)
        {
            List<float> list = new List<float>();
            foreach (Vector3D vector in mesh.Normals)
            {
                list.Add(vector.X);
                list.Add(vector.Y);
                list.Add(vector.Z);
            }
            return list.ToArray();
        }
        
        public static RawModel LoadObjModel(string fileName, Loader loader)
        {
            List<Vertex3f> vertexList = new List<Vertex3f>();
            List<Vertex2f> uvList = new List<Vertex2f>();
            List<Vertex3f> normalList = new List<Vertex3f>();
            List<int> indexList = new List<int>();
            
            float[] uvArray = null;
            float[] normalArray = null;
            
            string filePath = $".\\Resources\\{fileName}.obj";
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string line;
                    while (true)
                    {
                        line = sr.ReadLine();
                        string[] currentLine = line.Split(' ');

                        if (line.StartsWith("v "))
                        {
                            vertexList.Add(new Vertex3f(float.Parse(currentLine[1]), float.Parse(currentLine[2]), float.Parse(currentLine[3])));
                        }
                        else if (line.StartsWith("vt "))
                        {
                            uvList.Add(new Vertex2f(float.Parse(currentLine[1]), float.Parse(currentLine[2])));
                        }
                        else if (line.StartsWith("vn "))
                        {
                            normalList.Add(new Vertex3f(float.Parse(currentLine[1]), float.Parse(currentLine[2]), float.Parse(currentLine[3])));
                        }
                        else if (line.StartsWith("f "))
                        {
                            uvArray = new float[vertexList.Count * 2];
                            normalArray = new float[vertexList.Count * 3];
                            break;
                        }
                    }

                    while (line != null)
                    {
                        if (line.StartsWith("f ") == false)
                        {
                            line = sr.ReadLine();
                            continue;
                        }

                        string[] currentLine = line.Split(' ');
                        string[] vertex1 = currentLine[1].Split('/');
                        string[] vertex2 = currentLine[2].Split('/');
                        string[] vertex3 = currentLine[3].Split('/');

                        ProcessVertex(vertex1, indexList, uvList, normalList, uvArray, normalArray);
                        ProcessVertex(vertex2, indexList, uvList, normalList, uvArray, normalArray);
                        ProcessVertex(vertex3, indexList, uvList, normalList, uvArray, normalArray);

                        line = sr.ReadLine();
                    }
                }
            }
            return loader.LoadToVAO(ConvertToArray(vertexList), uvArray,  ConvertToArray(normalList), indexList.ToArray());
        }

        static float[] ConvertToArray(List<Vertex3f> vecterList)
        {
            float[] array = new float[vecterList.Count * 3];
            int index = 0;
            foreach (Vertex3f i in vecterList)
            {
                array[index++] = i.x;
                array[index++] = i.y;
                array[index++] = i.z;
             }
            return array;
        }
        
        /* 
         v: vertex
        vt: UV texture Coordinates
        vn: Normal vector
         f: 연관이 있는 v, vt, vn에 대한 index 조합
         (f position/texture-coordinates/normal position/texture-coordinates/normal position/texture-coordinates/normal)
         */
        static void ProcessVertex(string[] vertexData
            , List<int> indexList
            , List<Vertex2f> uvList
            , List<Vertex3f> normalList
            , float[] uvArray
            , float[] normalArray)
        {
            int i = int.Parse(vertexData[0]) - 1;//position
            indexList.Add(i);

            Vertex2f uv = uvList[int.Parse(vertexData[1]) - 1];//texture-coordinates
            uvArray[i * 2] = uv.x;
            uvArray[i * 2 + 1] = 1 - uv.y;

            Vertex3f normal = normalList[int.Parse(vertexData[2]) - 1];//normal
            normalArray[i * 3] = normal.x;
            normalArray[i * 3 + 1] = normal.y;
            normalArray[i * 3 + 2] = normal.z;
        }
    }
}