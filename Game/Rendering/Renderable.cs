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
        Shader.Use();
        Model.Use();
        Material.Use();

        Matrix4 model = Mathm.Transform(t);
        Shader.SetMatrix4("model", model);
        Matrix4 view = Mathm.GetViewMatrix(camTransform);
        Shader.SetMatrix4("view", view);
        Matrix4 projection = Mathm.GetProjectionMatrix(c);
        Shader.SetMatrix4("projection", projection);
    }
}
