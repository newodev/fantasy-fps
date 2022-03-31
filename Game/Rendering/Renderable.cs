using Game.Resources;
using CSharp_ECS;

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
        Shader.SetMatrix4("model", Mathm.Transform(t));
        Shader.SetMatrix4("view", Mathm.GetViewMatrix(camTransform));
        Shader.SetMatrix4("projection", Mathm.GetProjectionMatrix(c));
        Model.Use();
        Material.Use();
    }
}
