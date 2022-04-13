using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Resources;
using OpenTK.Mathematics;

using OpenTK.Graphics.OpenGL;


namespace Game.Rendering;

// Currently this is a naive implementation. Renders should be batched

class Renderer
{
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
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }
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
        renderables.Add(999, new Renderable(s, Resource.LoadMaterial(new Texture("Resources/Textures/pepe.jpg")), cube, VAO, VBO));
        renderables.Add(998, new Renderable(s, Resource.LoadMaterial(new Texture("Resources/Textures/floor.png")), cube, VAO, VBO));
    }
}
