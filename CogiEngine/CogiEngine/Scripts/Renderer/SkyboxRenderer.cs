using OpenGL;

namespace CogiEngine
{
    public class SkyboxRenderer
    {
        private const float SIZE = 500f;
        private readonly float[] VERTICES = {        
            -SIZE,  SIZE, -SIZE,
            -SIZE, -SIZE, -SIZE,
            SIZE, -SIZE, -SIZE,
            SIZE, -SIZE, -SIZE,
            SIZE,  SIZE, -SIZE,
            -SIZE,  SIZE, -SIZE,

            -SIZE, -SIZE,  SIZE,
            -SIZE, -SIZE, -SIZE,
            -SIZE,  SIZE, -SIZE,
            -SIZE,  SIZE, -SIZE,
            -SIZE,  SIZE,  SIZE,
            -SIZE, -SIZE,  SIZE,

            SIZE, -SIZE, -SIZE,
            SIZE, -SIZE,  SIZE,
            SIZE,  SIZE,  SIZE,
            SIZE,  SIZE,  SIZE,
            SIZE,  SIZE, -SIZE,
            SIZE, -SIZE, -SIZE,

            -SIZE, -SIZE,  SIZE,
            -SIZE,  SIZE,  SIZE,
            SIZE,  SIZE,  SIZE,
            SIZE,  SIZE,  SIZE,
            SIZE, -SIZE,  SIZE,
            -SIZE, -SIZE,  SIZE,

            -SIZE,  SIZE, -SIZE,
            SIZE,  SIZE, -SIZE,
            SIZE,  SIZE,  SIZE,
            SIZE,  SIZE,  SIZE,
            -SIZE,  SIZE,  SIZE,
            -SIZE,  SIZE, -SIZE,

            -SIZE, -SIZE, -SIZE,
            -SIZE, -SIZE,  SIZE,
            SIZE, -SIZE, -SIZE,
            SIZE, -SIZE, -SIZE,
            -SIZE, -SIZE,  SIZE,
            SIZE, -SIZE,  SIZE
        };

        private readonly string[] TEXTURE_FILES = {"skybox_right", "skybox_left", "skybox_top", "skybox_bottom", "skybox_back", "skybox_front"};

        private RawModel cube;
        private uint textureID;
        private SkyboxShader shader;

        public SkyboxRenderer(Loader loader, Matrix4x4f projectionMatrix)
        {
            this.cube = loader.LoadVAO(VERTICES, 3);
            this.textureID = loader.LoadCubeMap(TEXTURE_FILES);
            this.shader = new SkyboxShader();
            this.shader.Start();
            this.shader.LoadProjectionMatrix(projectionMatrix);
            this.shader.Stop();
        }

        public void Render(Camera camera)
        {
            this.shader.Start();
            this.shader.LoadViewMatrix(camera);
            Gl.BindVertexArray(this.cube.VaoID);
            Gl.EnableVertexAttribArray(0);
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.TextureCubeMap, this.textureID);
            Gl.DrawArrays(PrimitiveType.Triangles, 0, this.cube.VertexCount);
            Gl.DisableVertexAttribArray(0);
            Gl.BindVertexArray(0);
            this.shader.Stop();
        }

    }
}