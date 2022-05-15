#version 330 core

layout (location = 0) out vec3 gPosition;
layout (location = 1) out vec3 gNormal;
layout (location = 2) out vec4 gAlbedo;
layout (location = 3) out vec4 gDetail;

in vec3 FragPos;
in vec2 texCoord;
in vec3 Normal;
in mat3 TBN;

struct Material
{
    sampler2D albedoMap;
    sampler2D metallicMap;
    sampler2D roughnessMap;
    sampler2D aoMap;
    sampler2D normalMap;
};

// This could be a material array, and could be indexed by a vertex attribute
uniform Material material;

vec3 sampleNormalMap()
{
    vec3 norm = texture(material.normalMap, texCoord).rgb * 2.0 - 1.0;

    return normalize(TBN * norm);
}

void main()
{
    gPosition = FragPos;
    gNormal = sampleNormalMap();
    gAlbedo = texture(material.albedoMap, texCoord);
    gDetail.r = texture(material.metallicMap, texCoord).r;
    gDetail.g = texture(material.roughnessMap, texCoord).r;
    gDetail.b = texture(material.aoMap, texCoord).r;

}
