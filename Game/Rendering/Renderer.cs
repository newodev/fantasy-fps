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

// TODOS ON ADVANCED RENDERING
/*
 * 
 * Profiler
 * - Profile allocs, time per system call, fps etc
 * 
 * Instanced/Batched rendering
 * - Create a RenderAllocator, and create different versions that use batching, instancing, etc
 * - Compare and see what works best
 * - PROFILE
 * 
 * Deferred rendering + z pass
 * - per final pixel lighting calc (constant, doesn't scale with obj count)
 * 
 * Dynamic Global Illumination
 * - Look into UE5 Lumen
 * 
 * Ambient Occlusion
 * 
 * Bloom
 * 
 * Cascaded Shadows
 * 
 * Virtual Streamed Textures? See UE5
 * 
 * https://advances.realtimerendering.com/s2021/Karis_Nanite_SIGGRAPH_Advances_2021_final.pdf
 */
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
        Light.DirectionalCount = 0;
        Light.PointCount = 0;

    }

    public void Render()
    {
        // TODO: FRUSTRUM CULLING
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // TODO: Render shadow maps from each light source
        // Render each renderable
        // TODO: Multiple render passes with diff shaders?

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
                UsePoint(j, r, Light.PointLights[j], Light.PointPositions[j]);
            }

            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }
    }

    private void UseDirectional(int i, Renderable r, DirectionalLight light, Transform transform)
    {
        r.Shader.SetVec3($"directionalLights[{i}].direction", Mathm.Front(transform));
        r.Shader.SetVec3($"directionalLights[{i}].color", new Vector3(light.LightColor.R, light.LightColor.G, light.LightColor.B));
    }

    private void UsePoint(int i, Renderable r, PointLight light, Transform transform)
    {
        r.Shader.SetVec3($"pointLights[{i}].position", transform.Position);
        r.Shader.SetVec3($"pointLights[{i}].color", new Vector3(light.LightColor.R, light.LightColor.G, light.LightColor.B));
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

        GL.BindVertexArray(VAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        // TODO: Make a configurable resource loader
        Shader s = new("Resources/Shaders/Standard/Opaque/shader.vert", "Resources/Shaders/Standard/Opaque/pbr.frag");
        Shader s2 = new("Resources/Shaders/Standard/Opaque/shader.vert", "Resources/Shaders/Standard/Opaque/shader.frag");
        Model cube = ModelLoader.LoadCube();
        renderables.Add(1, new Renderable(s2, Resource.LoadMaterial("Resources/Textures/floor_ao.tif", "Resources/Textures/stone_rough.tif", "Resources/Textures/floor_met.tif", "Resources/Textures/stone_ao.tif", "Resources/Textures/stone_normal.tif"), cube, VAO, VBO));
        renderables.Add(999, new Renderable(s, Resource.LoadMaterial("Resources/Textures/stone_alb.tif", "Resources/Textures/stone_rough.tif", "Resources/Textures/floor_met.tif", "Resources/Textures/stone_ao.tif", "Resources/Textures/stone_normal.tif"), cube, VAO, VBO));
        renderables.Add(998, new Renderable(s, Resource.LoadMaterial("Resources/Textures/floor_alb.tif", "Resources/Textures/floor_rough.tif", "Resources/Textures/floor_met.tif", "Resources/Textures/floor_ao.tif", "Resources/Textures/floor_normal.tif"), cube, VAO, VBO));
    }
}
