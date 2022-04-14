#version 330 core
// out vec4 FragColor;

out vec4 outputColor;

in vec2 texCoord;
in vec3 Normal;

struct Material
{
    sampler2D diffuse;
    sampler2D specular;
    float shininess;
}

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
        result += CalcPointLight(directionalLights[i], norm, viewDir);  

    for(int i = 0; i < numPointLight; i++)
        result += CalcPointLight(pointLights[i], norm, FragPos, viewDir);  

    outputColor = vec4(output, 1.0);

}

vec3 CalcDirLight(DirectionalLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDirection = normalize(-light.direction);

    float diffuse = max(dot(normal, lightDirection), 0.0);

    vec3 reflectDirection = reflect(-lightDirection, normal);
    float specular = pow(max(dot(viewDir, reflectDirection), 0.0), material.shininess);

    vec3 ambient  = light.color  * vec3(texture(material.diffuse, TexCoords));
    vec3 diffuse  = light.color  * diffuse * vec3(texture(material.diffuse, texCoord));
    vec3 specular = light.color * specular * vec3(texture(material.specular, texCoord));

    return (ambient + diffuse + specular);
}

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDirection = normalize(light.position - fragPos);

    float diffuse = max(dot(normal, lightDirection), 0.0);

    vec3 reflectDirection = reflect(-lightDirection, normal);
    float specular = pow(max(dot(viewDir, reflectDirection), 0.0), material.shininess);

    float distance = length(light.position - fragPos);
    float attenuation = 1.0 / (distance);

    vec3 ambient = light.ambient * vec3(texture(material.diffuse, texCoord));
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, texCoord));
    vec3 specular = light.specular * spec * vec3(texture(material.specular, texCoord));

    ambient  *= attenuation;
    diffuse  *= attenuation;
    specular *= attenuation;

    return (ambient + diffuse + specular);
}