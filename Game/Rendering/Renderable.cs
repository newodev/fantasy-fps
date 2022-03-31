using Game.Resources;
using CSharp_ECS;
using OpenTK.Mathematics;

namespace Game.Rendering;

class Renderable
{
    public Shader Shader { get; private set; }
    public Material Material { get; set; }
    public Model Model { get; set; }

    public Renderable(Shader shader, Material material, Model model)
    {
        Shader = shader;
        Material = material;
        Model = model;
    }

    public void UseWithTransform(Transform t, Transform camTransform, Camera c)
    {
        Model.Use();
        Material.Use();
        Shader.Use();

        Matrix4 model = Mathm.Transform(t);
        Shader.SetMatrix4("model", model);
        // Matrix4 view = Mathm.GetViewMatrix(camTransform);
        Matrix4 view = new Matrix4(1, 0, 0, 0, 0, 0.8f, 0.6f, 0f, 0f, -0.6f, 0.8f, 0f, 0f, 0.16f, -2.7f, 1f);
        Shader.SetMatrix4("view", view);
        Matrix4 projection = Mathm.GetProjectionMatrix(c);
        Shader.SetMatrix4("projection", projection);
    }
}
