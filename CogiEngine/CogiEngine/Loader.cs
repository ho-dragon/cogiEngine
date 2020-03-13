using System;
using System.Collections.Generic;
using OpenGL;

namespace CogiEngine
{
    public class Loader
    {
        List<uint> vaoList = new List<uint>();
        List<uint> vboList = new List<uint>();

        public RawModel loadToVAO(float [] positions)
        {
            uint vaoID = CreateVAO();
            StoreDataInAttributeList(0, positions);
            UnbindVAO();
            return new RawModel(vaoID, positions.Length / 3);
        }

        public void CleanUp()
        {
            Gl.DeleteVertexArrays(vaoList.ToArray());
            Gl.DeleteBuffers(vboList.ToArray());
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

        void StoreDataInAttributeList(uint attributeNumber, float [] data)
        {
            uint vboID = Gl.GenBuffer();
            vboList.Add(vboID);

            Gl.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)(data.Length * sizeof(float)), data, BufferUsage.StaticDraw);//Bind 이후의 연산들은 이제 해당 VAO, VBO 문맥 하에서 이뤄집니다. 가령 VBO의 데이터를 CPU로부터 GPU에 복사할 때 사용하는 BindBuffer를 다음과 같이 사용할 수 있습니다.

            Gl.VertexAttribPointer(attributeNumber, 3, VertexAttribType.Float, false, 0, IntPtr.Zero);//현재 바인드 중인 VBO를 VAO에 연관시키는 VertexAttribPointer 함수도 마찬가지로 현재 바인드된 VAO를 대상으로 합니다.
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);//바인딩 후 더 이상 해당 GPU 메모리 조작이 필요 없다면 다음과 같이 바인딩 해제를 할 수 있습니다.
        }

        void UnbindVAO()
        {
            #region Desc
            //바인딩 후 더 이상 해당 GPU 메모리 조작이 필요 없다면 다음과 같이 바인딩 해제를 할 수 있습니다.
            //바인딩 해제는 문맥에서 제거한 것일 뿐 객체의 메모리가 제거된 것은 아닙니다. 해당 GPU 자원을 완전히 해제하려면 다음과 같이 삭제를 해야 합니다.
            // VAO의 경우
            //Gl.DeleteVertexArrays(vaoID);

            // VBO의 경우
            //Gl.DeleteBuffers(vboID);
            #endregion
            Gl.BindVertexArray(0);
        }
    }
}