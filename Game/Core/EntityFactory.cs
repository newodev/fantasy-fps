using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharp_ECS;
using OpenTK.Mathematics;
using System.Drawing;

namespace Game;

internal class EntityFactory
{
    private IComponent[] entity = Array.Empty<IComponent>();
    private int pointer = 0;

    public static EntityFactory New(int size)
    {
        EntityFactory instance = new EntityFactory();
        instance.entity = new IComponent[size];
        return instance;
    }

    public IComponent[] End()
    {
        if (pointer != entity.Length)
            throw new Exception("Entity was not correctly constructed to length");

        return entity;
    }

    public EntityFactory Transform(Vector3 pos, Vector3 rot, Vector3 scale)
    {
        entity[pointer] = new Transform() { Position = pos, Rotation = rot, Scale = scale };

        pointer++;
        return this;
    }

    public EntityFactory Renderable(int id)
    {
        entity[pointer] = new Rendering.RenderableComponent() { RenderableID = id };

        pointer++;
        return this;
    }

    public EntityFactory PointLight(Color col)
    {
        entity[pointer] = new Lighting.PointLight() { LightColor = col };

        pointer++;
        return this;
    }

    public EntityFactory Input()
    {
        entity[pointer] = new InputDevices.InputComponent();

        pointer++;
        return this;
    }

    public EntityFactory Camera(float ratio, float near, float far, float fov)
    {
        entity[pointer] = new Rendering.Camera() { AspectRatio = ratio, FarPlane = far, NearPlane = near, FieldOfView = fov };

        pointer++;
        return this;
    }
}
