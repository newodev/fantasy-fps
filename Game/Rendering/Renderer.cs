using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Resources;
using OpenTK.Mathematics;
using Game.Lighting;

using OpenTK.Graphics.OpenGL;


namespace Game.Rendering;

// Currently this is a naive implementation. Renders should be batched

class Renderer
{
    public Illumination Light = new();
    struct RenderObject
    {
        public Transform Transform;
        public int RenderableID;

        public RenderObject(Transform t, int id)
        {
            Transform = t;
            RenderableID = id;
        }
    }

    private Camera CurrentCamera;
    private Transform CameraPos;

    // Contains the renderable of each entity
    private Dictionary<int, RenderObject> entities = new();
    // Used for interpolation
    private Dictionary<int, RenderObject> lastFrame = new();

    // Renderables mapped by unique ID
    private Dictionary<int, Renderable> renderables = new();

    public void AddObject(int entityID, Transform t, int renderableID)
    {
        entities.Add(entityID, new RenderObject(t, renderableID));
    }

    public void UpdateCamera(Transform t, Camera c)
    {
        CameraPos = t;
        CurrentCamera = c;
    }

    public void Update()
    {
        // Swap dictionaries
        // Wipe entities (as it contains last frame)
        entities.Clear();
    }

    public void Render()
    {
        Console.WriteLine("Start " + GL.GetError());
        
        // TODO: FRUSTRUM CULLING
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // TODO: Render shadow maps from each light source
        // Render each renderable

        for (int i = 0; i < entities.Count; i++)
        {
            var entity = entities.ElementAt(i);
            int renderableID = entity.Value.RenderableID;
            Transform t = entity.Value.Transform;

            Renderable r = renderables[renderableID];
            r.UseWithTransform(t, CameraPos, CurrentCamera);

            r.Shader.SetInt("numDirLight", Light.DirectionalCount);

            for (int j = 0; j < Light.DirectionalCount; j++)
            {
                UseDirectional(j, r, Light.Directionals[j], Light.DirectionalDirections[j]);
            }

            r.Shader.SetInt("numPointLight", Light.PointCount);

            for (int j = 0; j < Light.PointCount; j++)
            {
                UseDirectional(j, r, Light.Directionals[j], Light.DirectionalDirections[j]);
            }

            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }

        Console.WriteLine("End " + GL.GetError());

    }

    private void UseDirectional(int i, Renderable r, DirectionalLight light, Transform transform)
    {
        r.Shader.SetVec3($"directionalLights[{i}].direction", Mathm.Front(transform));
        r.Shader.SetVec3($"directionalLights[{i}].color", new Vector3(light.LightColor.R, light.LightColor.G, light.LightColor.B));
    }

    public void CullFrustrum(Camera cam, Transform camPos)
    {
        // TODO: Loop through entities in spatial index
        // TODO: Ones that intersect frustrum are kept for this render
    }

    public void RenderShadowMap()
    {

    }

    int VAO;
    int VBO;
    public void Init()
    {
        VAO = GL.GenVertexArray();
        VBO = GL.GenBuffer();
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Enable(EnableCap.DepthTest);

        // TODO: Make a configurable resource loader
        Shader s = new("Resources/Shaders/Standard/Opaque/shader.vert", "Resources/Shaders/Standard/Opaque/shader.frag");
        Model cube = Resource.GenCube();
        renderables.Add(999, new Renderable(s, Resource.LoadMaterial("Resources/Textures/pepe.jpg", "Resources/Textures/specular.jpg", 1f), cube, VAO, VBO));
        renderables.Add(998, new Renderable(s, Resource.LoadMaterial("Resources/Textures/floor.png", "Resources/Textures/specular.jpg", 20.5f), cube, VAO, VBO));
    }
}
