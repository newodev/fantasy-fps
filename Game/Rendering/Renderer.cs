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

    private void NewMethod()
    {
        Renderable r = renderables[entities.ElementAt(0).Value.RenderableID];

        r.Model.Use(VAO, VBO);

        r.Material.Texture.Use(TextureUnit.Texture0);

        r.Shader.Use();
        r.Shader.InitialiseAttribute("aPosition", 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        r.Shader.InitialiseAttribute("aTexCoord", 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

        r.Shader.SetMatrix4("model", Mathm.Transform(entities.ElementAt(0).Value.Transform));
        r.Shader.SetMatrix4("view", Mathm.GetViewMatrix(CameraPos));
        r.Shader.SetMatrix4("projection", Mathm.GetProjectionMatrix(CurrentCamera));

        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }

    int VAO;
    int VBO;
    public void Init()
    {
        VAO = GL.GenVertexArray();
        VBO = GL.GenBuffer();
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
        renderables.Add(999, new Renderable(new Shader("OpenGLTest/shader.vert", "OpenGLTest/shader.frag"), Resource.LoadMaterial(new Texture("Resources/pepe.jpg")), Resource.GenCube(), VAO, VBO));
        renderables.Add(998, new Renderable(new Shader("OpenGLTest/shader.vert", "OpenGLTest/shader.frag"), Resource.LoadMaterial(new Texture("Resources/floor.png")), Resource.GenCube(), VAO, VBO));
    }
}
