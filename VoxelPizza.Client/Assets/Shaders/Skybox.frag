#version 320 es
precision highp float;
precision lowp sampler;

layout(set = 0, binding = 0) uniform textureCube CubeTexture;
layout(set = 0, binding = 1) uniform sampler CubeSampler;

layout(location = 0) in vec3 fsin_0;
layout(location = 0) out vec4 OutputColor;

void main()
{
    OutputColor = texture(samplerCube(CubeTexture, CubeSampler), fsin_0);
}
