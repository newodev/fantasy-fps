﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Resources;
using CSharp_ECS.ECSCore;

using OpenTK.Graphics.OpenGL4;

namespace Game.Rendering;

// Currently this is a naive implementation. Renders should be batched

class Renderer : JobSystem 
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

    public void Update()
    {
        // Swap dictionaries
        // Wipe entities (as it contains last frame)
    }
    public void Render()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // Render each renderable
        for (int i = 0; i < entities.Count; i++)
        {
            var entity = entities.ElementAt(i);
            int renderableID = entity.Value.RenderableID;
            Transform t = entity.Value.Transform;

            Renderable r = renderables[renderableID];

            r.UseWithTransform(t, );
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }
    }

    public void Init()
    {
        renderables.Add(999, new Renderable(new Shader("OpenGLTest/shader.vert", "OpenGLTest/shader.frag"), Resource.LoadMaterial(new Texture("Resources/pepe.jpg")), Resource.GenCube()));
    }
}
