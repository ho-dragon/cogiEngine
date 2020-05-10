using System.Linq.Expressions;
using OpenGL;

namespace CogiEngine
{
    public class SkyboxRenderer
    {
        private const float SIZE = 500f;

        private readonly float[] VERTICES =
        {
            -SIZE, SIZE, -SIZE,
            -SIZE, -SIZE, -SIZE,
            SIZE, -SIZE, -SIZE,
            SIZE, -SIZE, -SIZE,
            SIZE, SIZE, -SIZE,
            -SIZE, SIZE, -SIZE,

            -SIZE, -SIZE, SIZE,
            -SIZE, -SIZE, -SIZE,
            -SIZE, SIZE, -SIZE,
            -SIZE, SIZE, -SIZE,
            -SIZE, SIZE, SIZE,
            -SIZE, -SIZE, SIZE,

            SIZE, -SIZE, -SIZE,
            SIZE, -SIZE, SIZE,
            SIZE, SIZE, SIZE,
            SIZE, SIZE, SIZE,
            SIZE, SIZE, -SIZE,
            SIZE, -SIZE, -SIZE,

            -SIZE, -SIZE, SIZE,
            -SIZE, SIZE, SIZE,
            SIZE, SIZE, SIZE,
            SIZE, SIZE, SIZE,
            SIZE, -SIZE, SIZE,
            -SIZE, -SIZE, SIZE,

            -SIZE, SIZE, -SIZE,
            SIZE, SIZE, -SIZE,
            SIZE, SIZE, SIZE,
            SIZE, SIZE, SIZE,
            -SIZE, SIZE, SIZE,
            -SIZE, SIZE, -SIZE,

            -SIZE, -SIZE, -SIZE,
            -SIZE, -SIZE, SIZE,
            SIZE, -SIZE, -SIZE,
            SIZE, -SIZE, -SIZE,
            -SIZE, -SIZE, SIZE,
            SIZE, -SIZE, SIZE
        };

        private readonly string[] DAY_TEXTURE_FILES =
            {"skybox_right", "skybox_left", "skybox_top", "skybox_bottom", "skybox_back", "skybox_front"};

        private readonly string[] NIGHT_TEXTURE_FILES =
        {
            "skybox_night_right", "skybox_night_left", "skybox_night_top", "skybox_night_bottom", "skybox_night_back",
            "skybox_night_front"
        };

        private RawModel cube;
        private uint textureID_day;
        private uint textureID_night;
        private SkyboxShader shader;
        private float time = 0f;

        public SkyboxRenderer(Loader loader, Matrix4x4f projectionMatrix)
        {
            this.cube = loader.LoadVAO(VERTICES, 3);
            this.textureID_day = loader.LoadCubeMap(DAY_TEXTURE_FILES);
            this.textureID_night = loader.LoadCubeMap(NIGHT_TEXTURE_FILES);
            this.shader = new SkyboxShader();
            this.shader.Start();
            this.shader.ConnectTextureUnits();
            this.shader.LoadProjectionMatrix(projectionMatrix);
            this.shader.Stop();
        }

        public void Render(Camera camera, Vertex3f fogColor, float frameTimeSec)
        {
            this.shader.Start();
            this.shader.LoadViewMatrix(camera, frameTimeSec);
            this.shader.LoadFogColor(fogColor);
            Gl.BindVertexArray(this.cube.VaoID);
            Gl.EnableVertexAttribArray(0);
            BindTextures(frameTimeSec);
            Gl.DrawArrays(PrimitiveType.Triangles, 0, this.cube.VertexCount);
            Gl.DisableVertexAttribArray(0);
            Gl.BindVertexArray(0);
            this.shader.Stop();
        }


        private void BindTextures(float frameTimeSec)
        {
            time += frameTimeSec * 1000;
            time %= 24000;
            uint texture1;
            uint texture2;
            float blendFactor;

            float time1 = 5000;
            float time2 = 8000;
            float time3 = 21000;
            float time4 = 24000;

            if (time >= 0 && time < time1)
            {
                texture1 = textureID_night;
                texture2 = textureID_night;
                blendFactor = (time - 0) /  (time1 - 0);
            }
            else if (time >= time1 && time < time2)
            {
                texture1 = textureID_night;
                texture2 = textureID_day;
                blendFactor = (time - time1) / (time2 - time1);
            }
            else if (time >= time2 && time < time3)
            {
                texture1 = textureID_day;
                texture2 = textureID_day;
                blendFactor = (time - time2) / (time3 - time2);
            }
            else
            {
                texture1 = textureID_day;
                texture2 = textureID_night;
                blendFactor = (time - time3) / (time4 - time3);
            }

            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.TextureCubeMap, texture1);

            Gl.ActiveTexture(TextureUnit.Texture1);
            Gl.BindTexture(TextureTarget.TextureCubeMap, texture2);

            this.shader.LoadBendFactor(blendFactor);
        }
    }
}