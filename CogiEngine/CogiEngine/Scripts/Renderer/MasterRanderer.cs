﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using CogiEngine.Water;
using OpenGL;

namespace CogiEngine
{
    public class MasterRanderer
    {
        private const float FOV = 70;
        private const float NEAR_PLANE = 0.1f;
        private const float FAR_PLANE = 1000f;
        
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
        
        public Matrix4x4f ProjectionMatrix => projectionMatrix;

        public MasterRanderer(Loader loader, int width, int height)
        {
            Gl.Enable(EnableCap.DepthTest);
            this.clientWidth = width;
            this.clientHeight = height;
            this.projectionMatrix  = Maths.CreateProjectionMatrix(FOV, AspectRatio, NEAR_PLANE, FAR_PLANE);
            
            //Entities
            this.entityShader = new StaticShader();
            this.entityRenderer = new EntityRenderer(entityShader, width, height, this.projectionMatrix);
            this.entities = new Dictionary<TextureModel, List<Entity>>();
            
            //Terrain
            this.terrainShader = new TerrainShader();
            this.terrainRenderer = new TerrainRenderer(this.terrainShader, this.projectionMatrix);
            this.terrainList = new List<Terrain>();
            
            //Skybox
            this.skyboxRenderer = new SkyboxRenderer(loader, this.projectionMatrix);
        }

        public static void EnableCulling()
        {
            Gl.Enable(EnableCap.CullFace);//Optimize
            Gl.CullFace(CullFaceMode.Back);//Optimize
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
        
        public void RenderScene(int width, int height, List<Entity> entities, Terrain terrain, List<Light> lights, Camera camera, Vertex4f clipPlane, float frameTimeSec) {
          
            for (int i = 0; i < entities.Count; i++)
            {
                ProcessEntity(entities[i]);
            }
            ProcessTerrain(terrain);
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
            this.terrainRenderer.Render(this.terrainList);
            this.terrainShader.Stop();
            
            //Skybox
            this.skyboxRenderer.Render(camera, new Vertex3f(SKY_COLOR_RED, SKY_COLOR_GREEN, SKY_COLOR_BLUE), frameTimeSec);
            this.entities.Clear();
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