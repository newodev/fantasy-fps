using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Lighting;
using OpenTK.Graphics.OpenGL;
using Game.Rendering;
using Game.Resources;

namespace Game.NewRendering;

class Renderer
{
    public Illumination Light = new();
    private GBufferStage gBuffer;
    private LightingStage lighting;

    private Dictionary<int, Model> models = new();
    private Dictionary<int, RenderObject> entities = new();

    public Renderer()
    {
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Enable(EnableCap.DepthTest);

        gBuffer = new(models, entities);
        lighting = new(Light);
        lighting.SetGBuffer(gBuffer.GetLayers());
    }

    public void Render()
    {
        gBuffer.Render();
        //lighting.Render();
    }

    public void UpdateCamera(Transform t, Camera c)
    {
        gBuffer.camTransform = t;
        gBuffer.camera = c;
    }

    public void AddObject(int entityID, Transform t, int modelID)
    {
        entities.Add(entityID, new RenderObject(modelID, t));
    }

    public void Update()
    {
        // Swap dictionaries
        // Wipe entities (as it contains last frame)
        entities.Clear();
        Light.DirectionalCount = 0;
        Light.PointCount = 0;
    }

    public void Init()
    {
        float[] cube = ModelLoader.CubeVerts();
        models.Add(999, new Model(cube, Resource.LoadMaterial("Resources/Textures/stone_alb.tif", "Resources/Textures/stone_rough.tif", "Resources/Textures/floor_met.tif", "Resources/Textures/stone_ao.tif", "Resources/Textures/stone_normal.tif")));
        models.Add(998, new Model(cube, Resource.LoadMaterial("Resources/Textures/floor_alb.tif", "Resources/Textures/floor_rough.tif", "Resources/Textures/floor_met.tif", "Resources/Textures/floor_ao.tif", "Resources/Textures/floor_normal.tif")));
    }
}
