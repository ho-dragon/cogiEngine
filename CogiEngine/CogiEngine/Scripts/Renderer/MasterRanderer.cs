﻿using System.Collections.Generic;
using OpenGL;

namespace CogiEngine
{
    public class MasterRanderer
    {
        private const float FOV = 70;
        private const float NEAR_PLANE = 0.1f;
        private const float FAR_PLANE = 1000f;
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
        
        public MasterRanderer(int width, int height)
        {
            Gl.Enable(EnableCap.DepthTest);
            Gl.Enable(EnableCap.CullFace);//Optimize -- issue. not showing terrain
            Gl.CullFace(CullFaceMode.Back);//Optimize
            
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
        
        public void Render(Light light, Camera camera)
        {
            Prepare();
            
            //Entities
            this.entityShader.Start();
            this.entityShader.LoadLight(light);
            this.entityShader.LoadViewMatrix(camera);
            this.entityRenderer.Render(this.entities);
            this.entityShader.Stop();
            
            //Terrain
            this.terrainShader.Start();
            this.terrainShader.LoadLight(light);
            this.terrainShader.LoadViewMatrix(camera);
            this.terrainRenderer.Render(this.terrainList);
            this.terrainShader.Stop();
            
            entities.Clear();
        }

        public void UpdateViewRect(int width, int height)
        {
            SetViewRect(width, height);
        }

        public void ProcessTerrain(Terrain terrain)
        {
            this.terrainList.Add(terrain);
        }

        public void ProcessEntity(Entity entity)
        {
            TextureModel entityModel = entity.Model;
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
        
        public void Prepare()
        {
            Gl.Viewport(0, 0, clientWidth, clientHeight);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Gl.ClearColor(0, 0.3f, 0, 1f);
        }

        public void CleanUp()
        {
            this.entityShader.CleanUp();
            this.terrainShader.CleanUp();
        }
    }
}