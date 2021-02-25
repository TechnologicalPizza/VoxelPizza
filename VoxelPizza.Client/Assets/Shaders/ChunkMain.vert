#version 450

struct TextureAnimation
{
    uint Type;
    uint Count;
    float Rate;
};

layout(set = 0, binding = 0) uniform ProjectionMatrix
{
    mat4 Projection;
};

layout(set = 0, binding = 1) uniform ViewMatrix
{
    mat4 View;
};
layout(set = 0, binding = 2) uniform WorldInfo
{
    float GlobalTime;
    float GlobalTimeFraction;
};

layout(set = 1, binding = 0) uniform ChunkInfo
{
    mat4 World;
    mat4 InverseWorld;
};

layout(location = 0) in vec3 Position;
layout(location = 1) in uint Normal;
layout(location = 2) in uint TexAnimation0;
layout(location = 3) in uint TexRegion0;

layout(location = 0) out vec3 f_Normal;
layout(location = 1) flat out uint f_TexRegion0_0;
layout(location = 2) flat out uint f_TexRegion0_1;
layout(location = 3) out float f_TexFraction_0;

vec3 unpack3x10(uint packed) 
{
    vec3 v;
    v.x = (packed & 1023);
    v.y = (packed >> 10) & 1023;
    v.z = (packed >> 20) & 1023;
    v /= 1023.0 / 2.0;
    v -= 1.0;
    return v;
}

TextureAnimation unpackTexAnim(uint packed)
{
    TextureAnimation anim; 
    anim.Type = packed & 1;
    anim.Count = (packed >> 1) & 16383;
    anim.Rate = (packed >> 15) / 4096.0;
    return anim;
}

void main()
{
    vec4 worldPosition = World * vec4(Position, 1);
    vec4 outPosition = Projection * View * worldPosition;

    vec3 normal = unpack3x10(Normal);
    vec4 outNormal = InverseWorld * vec4(normal, 1);

    TextureAnimation texAnim0 = unpackTexAnim(TexAnimation0);
    float step0 = GlobalTime * texAnim0.Rate;
    float indexF0;
    float texFract0 = modf(step0 - texAnim0.Type * 0.5f, indexF0);
    uint indexOffset0 = uint(indexF0);
    uint indexOffset0_0 = indexOffset0 % texAnim0.Count;
    uint indexOffset0_1 = (indexOffset0 + texAnim0.Type) % texAnim0.Count;
    
    gl_Position = outPosition;

    f_Normal = normalize(outNormal.xyz);
    
    f_TexRegion0_0 = TexRegion0 + indexOffset0_0;
    f_TexRegion0_1 = TexRegion0 + indexOffset0_1;
    f_TexFraction_0 = texFract0;
}