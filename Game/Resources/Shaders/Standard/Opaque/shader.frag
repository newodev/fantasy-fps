#version 330 core
// out vec4 FragColor;

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;
// uniform sampler2D texture2;

void main()
{
    // FragColor = mix(texture(texture1, texCoord), texture(texture2, texCoord), 0.5);
    outputColor = texture(texture0, texCoord);
    //outputColor = vec4(texCoord.x, texCoord.y, 0, 1);
}