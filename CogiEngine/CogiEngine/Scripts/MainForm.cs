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
        private GUIRenderer guiRenderer;
        private Loader loader;
        private Camera camera;
        private List<Light> lgihtList;
        private List<Entity> entities;
        private List<GUITexture> guiList;
        private Terrain terrain;
        private Player player;
        
        public MainForm()
        {
            InitializeComponent();
        }

        public void InitializeComponent()
        {
            this.SuspendLayout();
            this.glControl = new GlControl();
            this.displayManager = new DisplayManager();
            this.inputManager = new InputManager();

            //glControl
            this.glControl.Animation = true;
            this.glControl.AnimationTimer = false;
            this.glControl.BackColor = Color.FromArgb(64, 64, 64);
            this.glControl.ColorBits = 24u;//Gl.COLOR_BUFFER_BIT;
            this.glControl.DepthBits = 24u;//Gl.DEPTH_BUFFER_BIT;
            this.glControl.Dock = DockStyle.Fill;
            this.glControl.Location = new Point(DisplayManager.WIDTH, DisplayManager.HEIGHT);
            this.glControl.MultisampleBits = 0u;
            this.glControl.Name = "glControl";
            this.glControl.Size = new Size(DisplayManager.WIDTH, DisplayManager.HEIGHT);
            this.glControl.StencilBits = 0u;//Gl.MULTISAMPLE_BIT;
            this.glControl.TabIndex = 0;

            this.glControl.ContextCreated += OnCreated_GlControl;
            this.glControl.ContextDestroying += OnDestroying_GlControl;
            this.glControl.Render += OnRender_GlControl;
            this.glControl.ContextUpdate += OnUpdate_GlControl;
            
            //input event
            this.glControl.MouseWheel += this.inputManager.OnMoueWheel;
            this.glControl.KeyDown += this.inputManager.OnKeyDown;
            this.glControl.KeyUp += this.inputManager.OnKeyUp;
            this.glControl.MouseDown += this.inputManager.OnMouseDown;
            this.glControl.MouseUp += this.inputManager.OnMouseUp;

            //winform
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(DisplayManager.WIDTH, DisplayManager.HEIGHT);
            this.Controls.Add(this.glControl);
            this.Name = "CogiEngine";
            this.Text = "[CogiEngine] Window";
            this.ResumeLayout(false);
            this.KeyPreview = true;
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
            this.guiRenderer = new GUIRenderer(this.loader);
            
            //Camera 
            this.camera = new Camera(new Vertex3f(0, 10, 0), 20f);
            this.inputManager.OnEventMouseWheel += this.camera.OnEventWheel;
            
            //Light
            this.lgihtList = new List<Light>();
            this.lgihtList.Add(new Light(new Vertex3f(0, 10000,-7000), new Vertex3f(1,1,1)));
            this.lgihtList.Add(new Light(new Vertex3f(-200, 10,-200), new Vertex3f(5,0,0)));
            this.lgihtList.Add(new Light(new Vertex3f(200, 10,200), new Vertex3f(0,0,5)));

            //Load Resources
            this.entities = new List<Entity>();
            this.terrain = LoadTerrain(this.loader);
            LoadEntities(this.terrain, this.entities, this.loader);
            LoadPlayer(this.loader);
            LoadGUI(this.loader);
        }

        private void LoadPlayer(Loader loader)
        {
            ModelTexture personTexture = new ModelTexture(loader.LoadTexture("playerTexture"));
            personTexture.ShineDamper = 30f;
            personTexture.Reflectivity = 0.3f;
            TextureModel personModel = new TextureModel(OBJLoader.LoadObjModelFromAssimp("person", loader), personTexture);
            this.player = new Player(personModel, new Vertex3f(256, 0, 256), 0, 0, 0, 1f);
        }
        
        private void LoadEntities(Terrain terrain, List<Entity> entities, Loader loader)
        {
            //Tree
            ModelTexture lowPolyTree = new ModelTexture(loader.LoadTexture("lowPolyTree"));
            lowPolyTree.ShineDamper = 30f;
            lowPolyTree.Reflectivity = 0.3f;
            TextureModel lowPolyTreeModel = new TextureModel(OBJLoader.LoadObjModelFromAssimp("lowPolyTree", loader), lowPolyTree);
            
            //Tree
            ModelTexture treeTexture = new ModelTexture(loader.LoadTexture("tree"));
            treeTexture.ShineDamper = 30f;
            treeTexture.Reflectivity = 0.3f;
            TextureModel treeModel = new TextureModel(OBJLoader.LoadObjModelFromAssimp("tree", loader), treeTexture);

            //Grass
            ModelTexture grassTexture = new ModelTexture(loader.LoadTexture("grassTexture"));
            grassTexture.HasTransparency = true;
            grassTexture.UseFakeLigihting = true;
            TextureModel grassModel = new TextureModel(OBJLoader.LoadObjModelFromAssimp("grassModel", this.loader), grassTexture);
            
            
            ModelTexture flowerTexture = new ModelTexture(loader.LoadTexture("flower"));
            flowerTexture.HasTransparency = true;
            flowerTexture.UseFakeLigihting = true;
            TextureModel flowerModel = new TextureModel(OBJLoader.LoadObjModelFromAssimp("grassModel", this.loader), flowerTexture);
            
            //Fern
            ModelTexture fernTextureAltas = new ModelTexture(loader.LoadTexture("fernAtlas"));
            fernTextureAltas.NumberOfRows = 2;
            TextureModel fernModel = new TextureModel(OBJLoader.LoadObjModelFromAssimp("fern", this.loader), fernTextureAltas);
     
            Random random = new Random();
            for (int i = 0; i < 200; i++)
            {
                if (i % 2 == 0)
                {
                    entities.Add(new Entity(fernModel, random.Next(0,3), GetRadomPosition(terrain, random, 400, 600), 0, 0, 0, 0.6f));
                }

                if (i % 5 == 0)
                {
                    entities.Add(new Entity(lowPolyTreeModel, GetRadomPosition(terrain, random, 400, 600), 0, 0, 0, 1f));
                }
                
                entities.Add(new Entity(treeModel, GetRadomPosition(terrain, random, 400, 600), 0, 0, 0, 5f));
                entities.Add(new Entity(grassModel, GetRadomPosition(terrain, random, 400, 600), 0, 0, 0, 1));
                entities.Add(new Entity(flowerModel, GetRadomPosition(terrain, random, 400, 600), 0, 0, 0, 1));
            }
        }

        private void LoadGUI(Loader loader)
        {
            this.guiList = new List<GUITexture>();
            GUITexture gui1 = new GUITexture(loader.LoadTexture("socuwan"), new Vertex2f(0.5f, 0.5f), new Vertex2f(0.25f, 0.25f));
            GUITexture gui2 = new GUITexture(loader.LoadTexture("thinmatrix"), new Vertex2f(0.3f, 0.74f), new Vertex2f(0.4f, 0.4f));
            this.guiList.Add(gui1);
            this.guiList.Add(gui2);
        }
        
        private Vertex3f GetRadomPosition(Terrain terrain, Random random, float randomX, float randomZ)
        {
            float x = (float)random.NextDouble() * (Terrain.SIZE - randomX);
            float z = (float)random.NextDouble() * + randomZ;
            float y = terrain.GetHeightOfTerrain(x, z);
            return  new Vertex3f(x, y, z);
        }

        private Terrain LoadTerrain(Loader loader)
        {
            TerrainTexture baseTexture = new TerrainTexture(loader.LoadRepeatTexture("grassy"));
            TerrainTexture redTexture = new TerrainTexture(loader.LoadRepeatTexture("dirt"));
            TerrainTexture greenTexture = new TerrainTexture(loader.LoadRepeatTexture("pinkFlowers"));
            TerrainTexture blueTexture = new TerrainTexture(loader.LoadRepeatTexture("path"));
            TerrainTexturePack texturePack = new TerrainTexturePack(baseTexture, redTexture, greenTexture, blueTexture);
            
            TerrainTexture blendMapTexture = new TerrainTexture(loader.LoadRepeatTexture("blendMap"));
            
            Bitmap heightMapImage = loader.LoadBitmap("heightmap");
            return new Terrain(0, 0, loader, texturePack, blendMapTexture, heightMapImage);;
        }
        

        private void OnDestroying_GlControl(object sender, GlControlEventArgs e)
        {
            this.displayManager.CloseDisplay();
            this.loader.CleanUp();
            this.renderer.CleanUp();
            this.guiRenderer.CleanUp();
        }
        
        private void OnUpdate_GlControl(object sender, GlControlEventArgs e)
        {   
            //this.models.ForEach(x => x.IncreaseRotation(0f,0.5f,0f));
            this.renderer.UpdateViewRect(this.glControl.ClientSize.Width, this.glControl.ClientSize.Height);
            this.inputManager.UpdateMousePosition();
            this.player.UpdateMove(this.inputManager, this.terrain, this.displayManager.GetFrameTimeSeconds());
            this.camera.UpdateMove(this.player.Position, this.player.RotationY, this.inputManager);
        }
        
        private void OnRender_GlControl(object sender, GlControlEventArgs e)
        {
            this.renderer.ProcessEntity(this.player);
            for (int i = 0; i < this.entities.Count; i++)
            {
                this.renderer.ProcessEntity(this.entities[i]);
            }
            this.renderer.ProcessTerrain(this.terrain);
            this.renderer.Render(this.lgihtList, this.camera);
            this.guiRenderer.Render(this.guiList);
            DrawAxis(0,0,0,1,1f);
            this.displayManager.UpdateDisplay();
        }
        
        public void DrawAxis(float px, float py, float pz, float dist, float thick)
        {
            Gl.Viewport(30,this.glControl.ClientSize.Height - 200, 150, 150);
            Gl.PushMatrix();
            Gl.LoadIdentity();
            
            Gl.Translate(0, 0, 0);
            Gl.Rotate(this.camera.Pitch, 1, 0, 0);
            Gl.Rotate(this.camera.Yaw, 0, 1, 0);
            Gl.Rotate(this.camera.Roll, 0, 0, 1);
            Gl.Scale(1,1,1);
            
            //x축은 빨간색
            Gl.Begin(PrimitiveType.Lines);
            Gl.LineWidth(thick);
            
            Gl.Color3(1f, 0f, 0f);
            Gl.Vertex3(px, py, pz);
            Gl.Vertex3(px + dist, py, pz);
            
            
            Gl.Color3(0, 0, 1f);
            Gl.Vertex3(px, py, pz);
            Gl.Vertex3(px, py + dist, pz);
            
            
            Gl.Color3(0f,1f,0f);
            Gl.Vertex3(px, py, pz);
            Gl.Vertex3(px, py, pz - dist);
            
            Gl.End();
            Gl.PopMatrix();
        }
    }
}