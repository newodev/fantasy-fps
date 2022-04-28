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
        Shader.InitialiseAttribute("aPosition", 3, VertexAttribPointerType.Float, false, 14 * sizeof(float), 0);
        Shader.InitialiseAttribute("aNormal", 3, VertexAttribPointerType.Float, false, 14 * sizeof(float), 3 * sizeof(float));
        Shader.InitialiseAttribute("aTexCoord", 2, VertexAttribPointerType.Float, false, 14 * sizeof(float), 6 * sizeof(float));
        Shader.InitialiseAttribute("aTangent", 3, VertexAttribPointerType.Float, false, 14 * sizeof(float), 8 * sizeof(float));
        Shader.InitialiseAttribute("aBitangent", 3, VertexAttribPointerType.Float, false, 14 * sizeof(float), 11 * sizeof(float));


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
        Shader.SetInt("material.albedoMap", 0);
        Shader.SetInt("material.roughnessMap", 1);
        Shader.SetInt("material.metallicMap", 2);
        Shader.SetInt("material.aoMap", 3);
        Shader.SetInt("material.normalMap", 4);

        Matrix4 model = Mathm.Transform(t);
        Shader.SetMatrix4("model", model);
        Matrix4 view = Mathm.GetViewMatrix(camTransform);
        Shader.SetMatrix4("view", view);
        Matrix4 projection = Mathm.GetProjectionMatrix(c);
        Shader.SetMatrix4("projection", projection);
        Matrix3 normal = new(Matrix4.Transpose(Matrix4.Invert(model)));
        Shader.SetMatrix3("normalMat", normal);
        Shader.SetVec3("viewPos", camTransform.Position);
    }
}
