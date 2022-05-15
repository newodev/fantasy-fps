#version 330 core
// PBR Implementation from LearnOpenGL.com

out vec4 FragColor;

in vec2 texCoord;

uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D gAlbedo;
uniform sampler2D gDetail;

struct PointLight
{
    vec3 position;
    vec3 color;
    vec3 distance;
};

uniform int numPointLight;
uniform PointLight pointLights[20];

uniform vec3 viewPos;

const float PI = 3.14159265359;

vec3 CalcPointLight(PointLight light, vec3 N, vec3 fragPos, vec3 V, vec3 F0);

float DistributionGGX(vec3 N, vec3 H, float roughness);
float GeometrySchlickGGX(float NdotV, float roughness);
float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness);
vec3 fresnelSchlick(float cosTheta, vec3 F0);

void main()
{
    vec3 FragPos = texture(gPosition, texCoord).rgb;
    // Normal vector of surface
    vec3 N = normalize(texture(gNormal, texCoord).rgb);
    // Outgoing vector, from surface to camera
    vec3 V = normalize(viewPos - FragPos);

    vec3 albedo = pow(texture(gAlbedo, texCoord).rgb, vec3(2.2));
    float metallic = texture(gDetail, texCoord).r;
    float roughness = texture(gDetail, texCoord).g;
    float ao = texture(gDetail, texCoord).b;

    vec3 F0 = vec3(0.04);
    F0 = mix(F0, albedo, metallic);

    vec3 Lo = vec3(0.0);

    for(int i = 0; i < numPointLight; i++)
    {
        PointLight light = pointLights[i];
        vec3 L = normalize(light.position - FragPos);
        vec3 H = normalize(V + L);
        float distance    = length(light.position - FragPos);
        float attenuation = 1.0 / (distance * distance);
        vec3 radiance     = light.color * attenuation;

        // cook-torrance brdf
        float NDF = DistributionGGX(N, H, roughness);
        float G = GeometrySmith(N, V, L, roughness);
        vec3 F = fresnelSchlick(max(dot(H, V), 0.0), F0);

        vec3 kS = F;
        vec3 kD = vec3(1.0) - kS;
        kD *= 1.0 - metallic;

        vec3 numerator = NDF * G * F;
        float denominator = 4.0 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0) + 0.0001;
        vec3 specular = numerator / denominator;

        // add to outgoing radiance Lo
        float NdotL = max(dot(N, L), 0.0);
        Lo += (kD * albedo / PI + specular) * radiance * NdotL;
    }

    // Calculate resulting color
    vec3 ambient = vec3(0.03) * albedo * ao;
    vec3 color = ambient + Lo;

    // Clamp HDR and gamma correct
    color = color / (color + vec3(1.0));
    color = pow(color, vec3(1.0/2.2));

    FragColor = vec4(color, 1.0);
    //FragColor = vec4(abs(N), 1.0);
}

vec3 fresnelSchlick(float cosTheta, vec3 F0)
{
    return F0 + (1.0 - F0) * pow(clamp(1.0 - cosTheta, 0.0, 1.0), 5.0);
}

float DistributionGGX(vec3 N, vec3 H, float roughness)
{
    float a = roughness*roughness;
    float a2 = a*a;
    float NdotH = max(dot(N, H), 0.0);
    float NdotH2 = NdotH*NdotH;

    float num = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;

    return num / denom;
}

float GeometrySchlickGGX(float NdotV, float roughness)
{
    float r = (roughness + 1.0);
    float k = (r*r) / 8.0;

    float num = NdotV;
    float denom = NdotV * (1.0 - k) + k;

    return num / denom;
}
float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx1 = GeometrySchlickGGX(NdotL, roughness);
    float ggx2 = GeometrySchlickGGX(NdotV, roughness);

    return ggx1 * ggx2;
}
