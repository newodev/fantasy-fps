using Game.Resources;
using CSharp_ECS;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace Game.Rendering;

class Renderable
{
    public Shader Shader { get; private set; }
    public Material Material { get; set; }
    public Model Model { get; set; }

    public Renderable(Shader shader, Material material, Model model)
    {
        Shader = shader;
        shader.InitialiseAttribute("aPosition", 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        shader.InitialiseAttribute("aTexCoord", 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3);

        Material = material;
        Model = model;
    }

    public void UseWithTransform(Transform t, Transform camTransform, Camera c)
    {
        Model.Use(0,0);
        Material.Use();
        Shader.Use();

        Matrix4 model = Mathm.Transform(t);
        Shader.SetMatrix4("model", model);
        Matrix4 view = Mathm.GetViewMatrix(camTransform);
        Shader.SetMatrix4("view", view);
        Matrix4 projection = Mathm.GetProjectionMatrix(c);
        Shader.SetMatrix4("projection", projection);
    }
}
