using System.Collections.Generic;

namespace CogiEngine
{
    public class MasterRanderer
    {
        private StaticShader shader;
        private Renderer renderer;
        private Dictionary<TextureModel, List<Entity>> entities;

        public MasterRanderer(int width, int height)
        {
            this.shader = new StaticShader();
            this.renderer = new Renderer(shader, width, height);
            this.entities = new Dictionary<TextureModel, List<Entity>>();
        }

        public void Render(Light light, Camera camera)
        {
            //Gl.Viewport(0, 0, senderControl.ClientSize.Width, senderControl.ClientSize.Height);
            renderer.Prepare();
            shader.Start();
            shader.LoadLight(light);
            shader.LoadViewMatrix(camera);
            renderer.Render(entities);
            shader.Stop();
            entities.Clear();
        }

        public void UpdateViewRect(int width, int height)
        {
            this.renderer.SetViewRect(width, height);
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

        public void CleanUp()
        {
            shader.CleanUp();
        }
    }
}