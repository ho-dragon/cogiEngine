using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenGL;
using Soil.NET;

namespace CogiEngine
{
    public class MainForm : Form
    {
        private GlControl glControl;
        private DisplayManager displayManager;
        private InputManager inputManager;
        private MasterRanderer renderer;
        private Loader loader;
        private Camera camera;
        private Light lgiht;
        private List<Entity> entities;
        private Terrain terrain_01;
        private Terrain terrain_02;
        
        public MainForm()
        {
            //Terrain.Test();
            InitializeComponent();
            KeyPreview = true;

        }

        public void InitializeComponent()
        {
            this.glControl = new GlControl();
            this.displayManager = new DisplayManager();
            this.inputManager = new InputManager();
            
            KeyDown += this.inputManager.OnKeyDown;
            KeyUp += this.inputManager.OnKeyUp;
            
            this.SuspendLayout();

            //glControl
            this.glControl.Animation = true;
            this.glControl.AnimationTimer = false;
            this.glControl.BackColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.glControl.ColorBits = 24u;//Gl.COLOR_BUFFER_BIT;
            this.glControl.DepthBits = 24u;//Gl.DEPTH_BUFFER_BIT;
            this.glControl.Dock = DockStyle.Fill;
            this.glControl.Location = new Point(0, 0);
            this.glControl.MultisampleBits = 0u;
            this.glControl.Name = "glControl";
            this.glControl.Size = new Size(1280, 720);
            this.glControl.StencilBits = 0u;//Gl.MULTISAMPLE_BIT;
            this.glControl.TabIndex = 0;

            this.glControl.ContextCreated += new EventHandler<GlControlEventArgs>(this.OnCreated_GlControl);
            this.glControl.ContextDestroying += new EventHandler<GlControlEventArgs>(this.OnDestroying_GlControl);
            this.glControl.Render += new EventHandler<GlControlEventArgs>(this.OnRender_GlControl);
            this.glControl.ContextUpdate += new EventHandler<GlControlEventArgs>(this.OnUpdate_GlControl);
            
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1280, 720);
            this.Controls.Add(this.glControl);
            this.Name = "CogiEngine";
            this.Text = "[CogiEngine] Window";
            this.ResumeLayout(false);
        }

        private void OnCreated_GlControl(object sender, GlControlEventArgs e)
        {
            GlControl glControl = (GlControl)sender;
            this.displayManager.CreateDisplay(glControl);
            
            bool result = WrapSOIL.Initialize();
            if (result == false)
            {
                MessageBox.Show("SOIL: Failed initialize : " + WrapSOIL.GetSoilLastError());
                return;
            }
            
            this.loader = new Loader();
            this.renderer = new MasterRanderer(glControl.ClientSize.Width, glControl.ClientSize.Height);
            
            //Camera 
            this.camera = new Camera();
            this.lgiht = new Light(new Vertex3f(20000, 40000,20000), new Vertex3f(1,1,1));
            this.inputManager.OnEventKeyDown += this.camera.OnEventKeyDown;
            
            this.entities = new List<Entity>();
            LoadEntities(this.entities, this.loader);
            LoadTerrain(this.loader);
        }

        private void LoadEntities(List<Entity> entities, Loader loader)
        {
            //Tree
            ModelTexture treeTexture = new ModelTexture(loader.LoadTexture("tree"));
            treeTexture.ShineDamper = 30f;
            treeTexture.Reflectivity = 0.3f;
            TextureModel treeModel = new TextureModel(OBJLoader.LoadObjModelFromAssimp("tree", loader), treeTexture);
            //Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, TextureMagFilter.Linear);
            //Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, TextureMagFilter.Linear);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, Gl.REPEAT);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, Gl.REPEAT);
            
            //Grass
            ModelTexture grassTexture = new ModelTexture(loader.LoadTexture("grassTexture"));
            grassTexture.HasTransparency = true;
            grassTexture.UseFakeLigihting = true;
            TextureModel grassModel = new TextureModel(OBJLoader.LoadObjModelFromAssimp("grassModel", this.loader), grassTexture);

            //Fern
            ModelTexture fernTexture = new ModelTexture(loader.LoadTexture("fern"));
            fernTexture.HasTransparency = true;
            TextureModel fernModel = new TextureModel(OBJLoader.LoadObjModelFromAssimp("fern", this.loader), fernTexture);
     
            Random random = new Random();
            for (int i = 0; i < 100; i++)
            {
                entities.Add(new Entity(treeModel, new Vertex3f((float)random.NextDouble() * 800 - 400,0, (float)random.NextDouble() * - 600), 0, 0, 0, 3));
                entities.Add(new Entity(grassModel, new Vertex3f((float)random.NextDouble() * 800 - 400,0, (float)random.NextDouble() * - 600), 0, 0, 0, 1));
                entities.Add(new Entity(fernModel, new Vertex3f((float)random.NextDouble() * 800 - 400,0, (float)random.NextDouble() * - 600), 0, 0, 0, 0.6f));
            }
        }

        private void LoadTerrain(Loader loader)
        {
            this.terrain_01 = new Terrain(0, -0.5f, loader, new ModelTexture(this.loader.LoadTexture("grass")));
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, Gl.REPEAT);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, Gl.REPEAT);
            
            this.terrain_02 = new Terrain(-1, -0.5f, loader, new ModelTexture(this.loader.LoadTexture("grass")));
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, Gl.REPEAT);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, Gl.REPEAT);
        }
        

        private void OnDestroying_GlControl(object sender, GlControlEventArgs e)
        {
            this.displayManager.CloseDisplay();
            this.loader.CleanUp();
            this.renderer.CleanUp();
        }
        
        private void OnUpdate_GlControl(object sender, GlControlEventArgs e)
        {   
            //this.models.ForEach(x => x.IncreaseRotation(0f,0.5f,0f));
            this.renderer.UpdateViewRect(glControl.ClientSize.Width, glControl.ClientSize.Height);
            this.camera.UpdateMove(this.inputManager);
        }
        
        private void OnRender_GlControl(object sender, GlControlEventArgs e)
        {
            //Control senderControl = (Control) sender;
            for (int i = 0; i < this.entities.Count; i++)
            {
                renderer.ProcessEntity(this.entities[i]);
            }
            renderer.ProcessTerrain(this.terrain_01); 
            renderer.ProcessTerrain(this.terrain_02);
            
            renderer.Render(this.lgiht, this.camera);
            //DrawAxis(PrimitiveType.Lines,0,0,0,0.1f,1f);
            this.displayManager.UpdateDisplay();
        }
        
        public void DrawAxis(PrimitiveType mode, float px, float py, float pz, float dist, float thick)
        {
            //x축은 빨간색
            Gl.Begin(mode);
            Gl.Color3(1f, 0f, 0f);
            Gl.LineWidth(thick);
            Gl.Vertex3(px, py, pz);
            Gl.Vertex3(px + dist, py, pz);
            Gl.End();

            //y축은 파랑
            Gl.Begin(mode);
            Gl.LineWidth(thick);
            Gl.Color3(0, 0, 1f);
            Gl.Vertex3(px, py, pz);
            Gl.Vertex3(px, py + dist, pz);
            Gl.End();

            //z축은 녹색
            Gl.Begin(mode);
            Gl.LineWidth(thick);
            Gl.Color3(0f,1f,0f);
            Gl.Vertex3(px, py, pz);
            Gl.Vertex3(px, py, pz + dist);
            Gl.End();
        }

    }
}