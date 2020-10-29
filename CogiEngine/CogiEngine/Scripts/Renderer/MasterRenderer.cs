using System.Collections.Generic;
using OpenGL;

namespace CogiEngine
{
    public class MasterRenderer
    {
        public const float FOV = 70;
        public const float NEAR_PLANE = 0.1f;
        public const float FAR_PLANE = 1000f;

        private const float SKY_COLOR_RED = 0.5444f;
        private const float SKY_COLOR_GREEN = 0.62f;
        private const float SKY_COLOR_BLUE = 0.69f;

        private int clientWidth;
        private int clientHeight;
        private Matrix4x4f projectionMatrix = Matrix4x4f.Identity;

        private float AspectRatio => clientWidth / clientHeight;

        //Entities
        private StaticShader entityShader;
        private EntityRenderer entityRenderer;
        private Dictionary<TextureModel, List<Entity>> entities;

        //Terrain
        private TerrainRenderer terrainRenderer;
        private TerrainShader terrainShader;
        private List<Terrain> terrainList;

        //Skybox
        private SkyboxRenderer skyboxRenderer;
        
        //Shadow
        public ShadowRenderer shadowRenderer;
        private ShadowDepthShader shadowDepthShader;
        private ShadowDepthFrameBuffer shadowDepthFrameBuffer;

        public Matrix4x4f ProjectionMatrix => projectionMatrix;

        public MasterRenderer(Camera camera, Loader loader, DirectionalLight sun, int width, int height)
        {
            Gl.Enable(EnableCap.DepthTest);
            this.clientWidth = width;
            this.clientHeight = height;
            this.projectionMatrix = Maths.CreateProjectionMatrix(FOV, AspectRatio, NEAR_PLANE, FAR_PLANE);

            //Entities
            this.entityShader = new StaticShader();
            this.entityRenderer = new EntityRenderer(entityShader, width, height, this.projectionMatrix);
            this.entities = new Dictionary<TextureModel, List<Entity>>();

            //Skybox
            this.skyboxRenderer = new SkyboxRenderer(loader, this.projectionMatrix);
            
            //Shadow
            this.shadowDepthShader = new ShadowDepthShader();
            this.shadowDepthFrameBuffer = new ShadowDepthFrameBuffer();
            this.shadowRenderer = new ShadowRenderer(sun, shadowDepthShader, shadowDepthFrameBuffer);
            
            //Terrain
            this.terrainShader = new TerrainShader();
            this.terrainRenderer = new TerrainRenderer(this.terrainShader, this.projectionMatrix, this.shadowRenderer);
            this.terrainList = new List<Terrain>();
        }

        public static void EnableCulling()
        {
            Gl.Enable(EnableCap.CullFace); //Optimize
            Gl.CullFace(CullFaceMode.Back); //Optimize
        }

        public static void DisableCulling()
        {
            Gl.Disable(EnableCap.CullFace);
        }

        public void SetViewRect(int width, int height)
        {
            if (clientWidth == width && clientHeight == height)
            {
                return;
            }

            this.clientWidth = width;
            this.clientHeight = height;
        }

        public void ProcessStart(List<Entity> entityList, Terrain terrain)
        {
            for (int i = 0; i < entityList.Count; i++)
            {
                ProcessEntity(entityList[i]);
            }

            ProcessTerrain(terrain);
        }

        public void ProcessEnd()
        {
            this.entities.Clear();
        }

        public void RenderScene(int width, int height, List<Light> lights, Camera camera, Vertex4f clipPlane, float frameTimeSec)
        {
            Render(width, height, lights, camera, clipPlane, frameTimeSec);
        }

        private void Render(int width, int height, List<Light> lightList, Camera camera, Vertex4f clipPlane, float frameTimeSec)
        {
            Prepare(width, height);

            //Entities
            this.entityShader.Start();
            this.entityShader.LoadClipPlane(clipPlane);
            this.entityShader.LoadSkyColor(SKY_COLOR_RED, SKY_COLOR_GREEN, SKY_COLOR_BLUE);
            this.entityShader.LoadLights(lightList);
            this.entityShader.LoadViewMatrix(camera);
            this.entityRenderer.Render(this.entities);
            this.entityShader.Stop();

            //Terrain
            this.terrainShader.Start();
            this.terrainShader.LoadClipPlane(clipPlane);
            this.terrainShader.LoadSkyColor(SKY_COLOR_RED, SKY_COLOR_GREEN, SKY_COLOR_BLUE);
            this.terrainShader.LoadLights(lightList);
            this.terrainShader.LoadViewMatrix(camera);
            this.terrainShader.LoadLightViewMatrix(this.shadowRenderer.LightSpaceMatrix);
            this.terrainRenderer.Render(this.terrainList);
            this.terrainShader.Stop();

            //Skybox
            this.skyboxRenderer.Render(camera, new Vertex3f(SKY_COLOR_RED, SKY_COLOR_GREEN, SKY_COLOR_BLUE), frameTimeSec);
        }
        public uint DepthMap => this.shadowRenderer.DepthMap;
        public void RenderShadowMap()
        {
            this.shadowRenderer.Render(this.entities, this.terrainList);

        }

        public void UpdateViewRect(int width, int height)
        {
            SetViewRect(width, height);
        }

        private void ProcessTerrain(Terrain terrain)
        {
            this.terrainList.Add(terrain);
        }

        public void ProcessEntity(Entity entity)
        {
            TextureModel entityModel = entity.TextureModel;
            if (this.entities.ContainsKey(entityModel))
            {
                this.entities[entityModel].Add(entity);
            }
            else
            {
                List<Entity> newBatch = new List<Entity>();
                newBatch.Add(entity);
                this.entities.Add(entityModel, newBatch);
            }
        }

        public void Prepare(int width, int height)
        {
            Gl.Viewport(0, 0, width, height);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Gl.ClearColor(SKY_COLOR_RED, SKY_COLOR_GREEN, SKY_COLOR_BLUE, 1f);
        }

        public void CleanUp()
        {
            this.entityShader.CleanUp();
            this.terrainShader.CleanUp();
        }
    }
}