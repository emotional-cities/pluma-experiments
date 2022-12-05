#version 400
uniform samplerCube tex;
in vec3 texCoord;
out vec4 fragColor;

void main()
{
  vec4 texel = texture(tex, texCoord);
  fragColor = texel;
}
