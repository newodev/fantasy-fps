using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Game.Rendering;

public class Shader : IDisposable
{
    public int Handle;

    public Shader(string vertexPath, string fragmentPath)
    {
        int VertexShader, FragmentShader;

        string VertexShaderSource;

        using (StreamReader reader = new StreamReader(vertexPath, Encoding.UTF8))
        {
            VertexShaderSource = reader.ReadToEnd();
        }

        string FragmentShaderSource;

        using (StreamReader reader = new StreamReader(fragmentPath, Encoding.UTF8))
        {
            FragmentShaderSource = reader.ReadToEnd();
        }

        VertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(VertexShader, VertexShaderSource);

        FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(FragmentShader, FragmentShaderSource);

        GL.CompileShader(VertexShader);

        string infoLogVert = GL.GetShaderInfoLog(VertexShader);
        if (infoLogVert != string.Empty)
            Console.WriteLine(infoLogVert);

        GL.CompileShader(FragmentShader);

        string infoLogFrag = GL.GetShaderInfoLog(FragmentShader);

        if (infoLogFrag != string.Empty)
            Console.WriteLine(infoLogFrag);

        Handle = GL.CreateProgram();

        GL.AttachShader(Handle, VertexShader);
        GL.AttachShader(Handle, FragmentShader);

        GL.LinkProgram(Handle);

        GL.DetachShader(Handle, VertexShader);
        GL.DetachShader(Handle, FragmentShader);
        GL.DeleteShader(FragmentShader);
        GL.DeleteShader(VertexShader);
    }

    public void InitialiseAttribute(string attribName, int size, VertexAttribPointerType type, bool normalized, int stride, int offset)
    {
        var loc = GL.GetAttribLocation(Handle, attribName);
        GL.EnableVertexAttribArray(loc);
        GL.VertexAttribPointer(loc, size, type, normalized, stride, offset);
    }

    public void Use()
    {
        GL.UseProgram(Handle);
    }

    private bool disposedValue = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            GL.DeleteProgram(Handle);

            disposedValue = true;
        }
    }

    ~Shader()
    {
        GL.DeleteProgram(Handle);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void SetInt(string name, int value)
    {
        int location = GL.GetUniformLocation(Handle, name);

        GL.Uniform1(location, value);
    }
    public void SetFloat(string name, float value)
    {
        int location = GL.GetUniformLocation(Handle, name);

        GL.Uniform1(location, value);
    }

    public void SetMatrix4(string name, Matrix4 mat)
    {
        int loc = GL.GetUniformLocation(Handle, name);

        GL.UniformMatrix4(loc, true, ref mat);
    }

    public void SetMatrix3(string name, Matrix3 mat)
    {
        int loc = GL.GetUniformLocation(Handle, name);

        GL.UniformMatrix3(loc, true, ref mat);
    }

    public void SetVec3(string name, Vector3 vec)
    {
        int loc = GL.GetUniformLocation(Handle, name);

        GL.Uniform3(loc, vec);
    }

    /*
    public void SetUniform(string name, params float[] values)
    {
        int location = GL.GetUniformLocation(Handle, name);

        if (values.Count() > 4 || values.Count() == 0)
            throw new ArgumentException($"Uniform must have between 1 and 4 values");
        if (location == -1)
            throw new ArgumentException($"Uniform of name {name} does not exist in this shader");

        // TODO: Find alternative, or make work for diff values
        GL.Uniform4(location, values[0], values[1], values[2], values[3]);
    }
    */
}