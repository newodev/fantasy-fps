#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aTexCoord;
layout(location = 3) in vec3 aTangent;
layout(location = 4) in vec3 aBitangent;

out vec2 texCoord;
out vec3 Normal;
out vec3 FragPos;
out mat3 TBN;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform mat3 normalMat;

void main()
{

    texCoord = aTexCoord;

    vec3 VertexWorldPos = vec3(vec4(aPosition, 1.0) * model);

    Normal = aNormal * normalMat;

    vec3 T = normalize(vec3(vec4(aTangent,   0.0) * model));
    vec3 B = normalize(vec3(vec4(aBitangent, 0.0) * model));
    vec3 N = normalize(vec3(vec4(aNormal,    0.0) * model));
    TBN = mat3(T, B, N);

    FragPos = VertexWorldPos;
    gl_Position =  vec4(VertexWorldPos, 1.0) * view * projection;
}
