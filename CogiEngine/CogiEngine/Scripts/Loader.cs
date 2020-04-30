using System;
using System.Collections.Generic;
using System.Drawing;
using OpenGL;

namespace CogiEngine
{
    public class Loader
    {
        List<uint> vaoList = new List<uint>();
        List<uint> vboList = new List<uint>();
        List<uint> loadedTextureList = new List<uint>();
        List<Bitmap> loadedBitmap = new List<Bitmap>();
        
        public RawModel LoadVAO(float [] positions,float [] textureCoords, float [] normals, int[] indices)
        {
            uint vaoID = CreateVAO();
            BindIndicesBuffer(indices);//Bind Index Buffer
            StoreDataInAttributeList(0, 3, positions); // Position 데이터를 VAO의 0번 슬롯에 할당
            StoreDataInAttributeList(1, 2, textureCoords);  
            StoreDataInAttributeList(2,3, normals);
            UnbindVAO();
            return new RawModel(vaoID, positions.Length);//PrimitiveType.Triangles
        }
        
        public RawModel LoadVAO_GUI(float[] positions)
        {
            uint vaoID = CreateVAO();
            StoreDataInAttributeList(0, 2, positions);
            UnbindVAO();
            return new RawModel(vaoID, positions.Length / 2);//PrimitiveType.TriangleStrip
        }

        public Bitmap LoadBitmap(string filename)
        {
            Bitmap bitmap = new Bitmap(string.Format(@"Resources\Textures\{0}.png", filename));
            if (bitmap.Height > 0 == false)
            {
                CogiLogger.Error("not found file" + filename);
                return null;
            }
            loadedBitmap.Add(bitmap);
            return bitmap;
        }

        public uint LoadTexture(string fileName)
        {
            string filePath = $".\\Resources\\Textures\\{fileName}.png";
            uint tex2d_id = Soil.NET.WrapSOIL.load_OGL_texture(filePath, Soil.NET.WrapSOIL.SOIL_LOAD.AUTO, Soil.NET.WrapSOIL.SOIL_NEW.ID,
                Soil.NET.WrapSOIL.SOIL_FLAG.MIPMAPS | Soil.NET.WrapSOIL.SOIL_FLAG.NTSC_SAFE_RGB | Soil.NET.WrapSOIL.SOIL_FLAG.COMPRESS_TO_DXT);
            this.loadedTextureList.Add(tex2d_id);
            
            Gl.GenerateMipmap(TextureTarget.Texture2d);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureLodBias, 0f);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, TextureMinFilter.LinearMipmapLinear);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, Gl.LINEAR);

            #region Desc
            // Mipmap과 TexturFiter는 함께 쓸수있는가?
            // Mimap사용시 TexturFiter의 TextureMinFilter는 TextureMinFilter.LinearMipmapLinear이걸로 넣어야 작동함.
            // TextureMagFilter는 Gl.LINEAR과 Gl.NEAREST로 변경시 정삭작동함(ex 잔디를 큰화면으로 키워서 확인)

            //GL_NEAREST는 텍스처를 형성하는 픽셀들을 명확히 볼 수 있는 차단된 패턴을 생성하는 반면
            //GL_LINEAR는 개별 픽셀들이 덜 보이는 더 매끄러운 패턴을 생성합니다.
            //텍스처 필터링은 확대(magnifying) 및 축소(minifying) 작업(스케일 업 혹은 다운)에 대해 설정할 수 있으므로
            //예를 들어 텍스처가 축소될 때 nearest neighbor filtering을 사용하고 텍스처가 확대될 때는 linear filtering을 사용할 수 있습니다.
            //따라서 우리는 glTexParameter* 함수를 통해 두 업션 모두에 대한 필터링 방법을 지정해야 합니다.
            
            //TextureLodBias는 텍스처의 LOD를 계산할 때 mipmap 레벨에 더해지는 값입니다.
            //TextureLodBias를 낮추면 민맵기능을 낮추는것이므로 화면에 보이는 픽셀의 정보량이많아 반짝거림.
            //따라서 -0.1f정도로 적게 낮춰서 반짝거림만 없도록하면 텍스쳐의 품질을 높일 수있음.(뭉게짐이 덜함)
            // 1f 하면 민맵레벨을 너무 높여서 화면전체 텍스쳐에 민맵이 적용되어 텍스쳐가 다 뭉게짐
            #endregion
            return tex2d_id;
        }
        
        public uint LoadRepeatTexture(string fileName)
        {
            uint loadNumber = LoadTexture(fileName);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, Gl.REPEAT);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, Gl.REPEAT);
            #region Desc
            //첫 번째 파라미터는 텍스처 타겟을 지정합니다. 우리는 2D 텍스처를 사용하기 때문에 타겟을 GL_TEXTURE_2D로 설정하였습니다.
            //두 번째 파라미터는 우리가 설정할 옵션과 어떤 축에 적용할 것인지 지정합니다. WRAP 옵션을 설정하고 S, T 축 모두에 적용하려고 합니다.
            //마지막 파라미터는 텍스처 wrapping 모드를 설정해야하며 이 경우에는 GL_MIRRORED_REPEAT을 사용하여 현재 활성화된 텍스처의 wrapping 옵션을 설정합니다.
            #endregion
            return loadNumber;
        }
        
        public void CleanUp()
        {
            Gl.DeleteVertexArrays(vaoList.ToArray());
            Gl.DeleteBuffers(vboList.ToArray());
            Gl.DeleteTextures(loadedTextureList.ToArray());
        }

        uint CreateVAO()
        {
            #region Desc
            //OpenGL(뿐만 아니라 GPU 관련한 프로그램들)은 보통 CPU가 사용하는 RAM의 데이터를 GPU에 복사하는 절차가 필요합니다.
            //따라서 VAO든, VBO든 모두 GPU에서 연산할 때 반드시 GPU 메모리에 올라와 있어야 합니다.
            //하지만 GPU 메모리를 직접 제어하는 것은 아니고 코드에서는 Handle을 이용해 간접적으로 다루게 됩니다. 결국 VAO와 VBO는 우선 GPU 메모리에 대한 Handle을 반환받는 것으로 시작합니다.
            #endregion
            uint vaoID = Gl.GenVertexArray();
            vaoList.Add(vaoID);
            Gl.BindVertexArray(vaoID);//앞으로의 버퍼 관련 함수의 호출에 대해 해당 핸들이 가리키는 GPU 메모리를 대상으로 하겠다고 Bind를 합니다.
            return vaoID;
        }

        void StoreDataInAttributeList(uint attributeNumber,int coordinateSize, float [] data)
        {
            uint vboID = Gl.GenBuffer();
            vboList.Add(vboID);

            Gl.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)(data.Length * sizeof(float)), data, BufferUsage.StaticDraw);//Bind 이후의 연산들은 이제 해당 VAO, VBO 문맥 하에서 이뤄집니다. 가령 VBO의 데이터를 CPU로부터 GPU에 복사할 때 사용하는 BindBuffer를 다음과 같이 사용할 수 있습니다.

            Gl.VertexAttribPointer(attributeNumber, coordinateSize, VertexAttribType.Float, false, 0, IntPtr.Zero);//현재 바인드 중인 VBO를 VAO에 연관시키는 VertexAttribPointer 함수도 마찬가지로 현재 바인드된 VAO를 대상으로 합니다.
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);//바인딩 후 더 이상 해당 GPU 메모리 조작이 필요 없다면 다음과 같이 바인딩 해제를 할 수 있습니다.
        }
        
        void BindIndicesBuffer(int[] indices)
        {
            uint vboID = Gl.GenBuffer();
            vboList.Add(vboID);
            
            Gl.BindBuffer(BufferTarget.ElementArrayBuffer, vboID);//BufferTarget.ElementArrayBuffer에 대해서는 언바인드를 하지는 않습니다.
            Gl.BufferData(BufferTarget.ElementArrayBuffer, (uint)(indices.Length * sizeof(int)), indices, BufferUsage.StaticDraw);
            #region Desc
            /*
            GL_ELEMENT_ARRAY_BUFFER(BufferTarget.ElementArrayBuffer)의 경우 VAO 상태의 일부로 바인딩이 되기 때문에 
            VAO를 Unbind 하면 GL_ELEMENT_ARRAY_BUFFER 역시 자동으로 Unbind 시킨다고 합니다.
            반면 GL_ARRAY_BUFFER(BufferTarget.ArrayBuffer)는 VAO의 일부가 아니기 때문에 명시적으로 Unbind 호출이 필요하다고.
            (실제로, unbind 유무에 상관없이 메모리 누수 같은 현상은 발생하지 않습니다.)
            심지어 VAO 조차도 Unbind를 할 필요는 없다고 합니다.
            어차피 다른 VAO를 Bind 하면 기존 바인드 되었던 VAO가 Unbind되므로 오히려 명시적인 Unbind는 성능상 좋지 않다고 합니다.
            단지, Unbind를 명시적으로 하는 것이 어떤 식으로든 지나칠 수 있는 실수를 미연에 방지할 수 있기 때문에
            그 정도 성능은 감수하고 맞춰 주는 것이 좋다는 언급도 합니다.
            */
            #endregion
        }

        void UnbindVAO()
        {
            Gl.BindVertexArray(0);
            #region Desc
            //바인딩 후 더 이상 해당 GPU 메모리 조작이 필요 없다면 다음과 같이 바인딩 해제를 할 수 있습니다.
            //바인딩 해제는 문맥에서 제거한 것일 뿐 객체의 메모리가 제거된 것은 아닙니다. 해당 GPU 자원을 완전히 해제하려면 다음과 같이 삭제를 해야 합니다.
            // VAO의 경우
            //Gl.DeleteVertexArrays(vaoID);

            // VBO의 경우
            //Gl.DeleteBuffers(vboID);
            #endregion
        }
    }
}