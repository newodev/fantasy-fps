#version 330 core
// out vec4 FragColor;

out vec4 FragColor;

in vec3 FragPos;
in vec2 texCoord;
in vec3 Normal;

struct Material
{
    sampler2D diffuse;
    sampler2D specular;
    float shininess;
};

uniform Material material;

struct DirectionalLight
{
    vec3 direction;
    vec3 color;
};

struct PointLight
{
    vec3 position;
    vec3 color;
    vec3 distance;
};

uniform int numDirLight;
uniform DirectionalLight directionalLights[5];
uniform int numPointLight;
uniform PointLight pointLights[20];

uniform vec3 viewPos;

vec3 CalcDirLight(DirectionalLight light, vec3 normal, vec3 viewDir);
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);

void main()
{
    vec3 norm = normalize(Normal);
    vec3 viewDir = normalize(viewPos - FragPos);

    vec3 result = vec3(0.0);

    for(int i = 0; i < numDirLight; i++)
        result += CalcDirLight(directionalLights[i], norm, viewDir);  

    for(int i = 0; i < numPointLight; i++)
        result += CalcPointLight(pointLights[i], norm, FragPos, viewDir);  

    FragColor = vec4(result, 1.0);

}

vec3 CalcDirLight(DirectionalLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDirection = normalize(-light.direction);

    float diffuse = max(dot(normal, lightDirection), 0.0);

    vec3 reflectDirection = reflect(-lightDirection, normal);
    float specular = pow(max(dot(viewDir, reflectDirection), 0.0), material.shininess);

    vec3 ambient  = light.color * 0.001 * vec3(texture(material.diffuse, texCoord));
    vec3 diffusev  = light.color * 0.009 * diffuse * vec3(texture(material.diffuse, texCoord));
    vec3 specularv = light.color * 0.01 * specular * vec3(texture(material.specular, texCoord));

    return (ambient + diffusev + specularv);
}

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDirection = normalize(light.position - fragPos);

    float diffuse = max(dot(normal, lightDirection), 0.0);

    vec3 reflectDirection = reflect(-lightDirection, normal);
    float specular = pow(max(dot(viewDir, reflectDirection), 0.0), material.shininess);

    float distance = length(light.position - fragPos);
    float attenuation = 1.0 / (distance);

    vec3 ambient = light.color * 0.3 * vec3(texture(material.diffuse, texCoord));
    vec3 diffusev = light.color * 0.3 * diffuse * vec3(texture(material.diffuse, texCoord));
    vec3 specularv = light.color * 0.1 * specular * vec3(texture(material.specular, texCoord));

    ambient  *= attenuation;
    diffusev  *= attenuation;
    specularv *= attenuation;

    return (ambient + diffusev + specularv);
}