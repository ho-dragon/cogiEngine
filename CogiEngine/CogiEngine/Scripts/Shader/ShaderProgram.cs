using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using OpenGL;

namespace CogiEngine
{
    public abstract class ShaderProgram
    {
        uint programID;
        uint vertexShaderID;
        uint fragmentShaderID;

        public ShaderProgram(string vertexFilePath, string fragmentFilePath)
        {
            this.vertexShaderID = LoadShader(vertexFilePath, ShaderType.VertexShader);
            this.fragmentShaderID = LoadShader(fragmentFilePath, ShaderType.FragmentShader);

            this.programID = Gl.CreateProgram();
            Gl.AttachShader(this.programID, this.vertexShaderID);
            Gl.AttachShader(this.programID, this.fragmentShaderID);

            BindAttributes();

            Gl.LinkProgram(this.programID);
            Gl.ValidateProgram(this.programID);
            
            GetAllUniformLocations();
        }

        protected abstract void BindAttributes();
        protected abstract void GetAllUniformLocations();

        protected int GetUniformLocation(string uniformName)
        {
            return Gl.GetUniformLocation(programID, uniformName);
        }
        
        protected void LoadFloat(int location, float value)
        {
            Gl.Uniform1f(location, 1, value);
        }
        
        protected void LoadInt(int location, int value)
        {
            Gl.Uniform1i(location, 1, value);
        }

        protected void LoadVector(int location, Vertex3f value)
        {
            Gl.Uniform3f(location, 1, value);
        }

        protected void LoadBoolean(int location, bool value)
        {
            Gl.Uniform1f(location, 1, value? 1.0f : 0.0f);
        }

        protected void LoadMatrix(int location,  Matrix4x4f matrix)
        {
            Gl.UniformMatrix4f<Matrix4x4f>(location, 1,false, matrix);
        }
        
        public void Start()
        {
            Gl.UseProgram(programID);
        }

        public void Stop()
        {
            Gl.UseProgram(0);
        }

        public void CleanUp()
        {
            Stop();
            Gl.DetachShader(programID, vertexShaderID);
            Gl.DetachShader(programID, fragmentShaderID);
            Gl.DeleteShader(vertexShaderID);
            Gl.DeleteShader(fragmentShaderID);
            Gl.DeleteProgram(programID);
        }

        protected void BindAttribute(uint attribute, string variableName)
        {
            Gl.BindAttribLocation(programID, attribute, variableName);
        }

        static uint LoadShader(string filePath, ShaderType type)
        {
            string[] codeText = ReadShaderCode(filePath);
            uint shaderID = Gl.CreateShader(type);
            Gl.ShaderSource(shaderID, codeText);
            Gl.CompileShader(shaderID);
            
            int compileResult = Gl.FALSE;
            Gl.GetShader(shaderID, ShaderParameterName.CompileStatus, out compileResult);

            if (compileResult == Gl.FALSE)
            {
                throw new InvalidDataException(OpenGLExtension.GetShaderInfoLog(shaderID));
            }
            return shaderID;
        }

        private static string[] ReadShaderCode(string file)
        {
            List<string> codes = new List<string>();
            foreach (string txt in File.ReadAllLines(file))
            {
                codes.Add(txt + Environment.NewLine);
            }
            return codes.ToArray();
        }
    }
}