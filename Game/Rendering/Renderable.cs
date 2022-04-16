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

    private int VAO;
    private int VBO;

    public Renderable(Shader shader, Material material, Model model, int vao, int vbo)
    {
        Shader = shader;
        Shader.InitialiseAttribute("aPosition", 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
        Shader.InitialiseAttribute("aNormal", 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
        Shader.InitialiseAttribute("aTexCoord", 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));

        Material = material;
        Model = model;

        VAO = vao;
        VBO = vbo;
    }

    public void UseWithTransform(Transform t, Transform camTransform, Camera c)
    {
        Model.Use(VAO, VBO);
        Material.Use();
        Shader.Use();
        Shader.SetInt("material.diffuse", 0);
        Shader.SetInt("material.specular", 1);
        Shader.SetVec3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
        Shader.SetFloat("material.shininess", Material.Shininess);

        Matrix4 model = Mathm.Transform(t);
        Shader.SetMatrix4("model", model);
        Matrix4 view = Mathm.GetViewMatrix(camTransform);
        Shader.SetMatrix4("view", view);
        Matrix4 projection = Mathm.GetProjectionMatrix(c);
        Shader.SetMatrix4("projection", projection);
        Matrix4 normal = model;
        normal.Invert();
        normal.Transpose();
        Shader.SetMatrix3("normalMat", new Matrix3(normal));
        Shader.SetVec3("viewPos", camTransform.Position);
    }
}
