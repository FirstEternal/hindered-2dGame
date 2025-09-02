sampler2D textureA : register(s0);
float4x4 transformA;
float2 resolution; 


float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR
{
    float2 uv = coords / resolution;

    // Sample from TextureA at `uv`
    float4 sampleA = tex2D(textureA, uv); // Explicit use of TextureA

    // Use a component from TransformA to modify output color
    float transformComponent = transformA[0][0]; // Reference TransformA

    return float4(sampleA.rgb * transformComponent, 1); // Use both TextureA and TransformA
}



technique CollisionCheck
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
/*
sampler2D TextureA : register(s0);
sampler2D TextureB : register(s1);

float4x4 TransformA;
float4x4 TransformB;

float2 resolution;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR
{
    // Convert screen coordinates to [0,1] UV range
    float2 uvCoords = coords / resolution;

    // Apply transformations to obtain the UVs for each texture
    float2 uvA = mul(float4(uvCoords, 0, 1), TransformA).xy;
    float2 uvB = mul(float4(uvCoords, 0, 1), TransformB).xy;

    // Clamp UVs to the [0,1] range to avoid out-of-bounds sampling
    uvA = clamp(uvA, 0.0, 1.0);
    uvB = clamp(uvB, 0.0, 1.0);

    // Sample each texture to get the alpha values (binary mask)
    float alphaA = tex2D(TextureA, uvA).a;
    float alphaB = tex2D(TextureB, uvB).a;

    // Convert alpha to binary: 1 if solid, 0 if transparent
    float maskA = (alphaA > 0.5) ? 1.0 : 0.0;
    float maskB = (alphaB > 0.5) ? 1.0 : 0.0;

    // Check if both textures overlap at this coordinate and are solid
    if (maskA > 0.5 && maskB > 0.5)
    {
        // Collision detected - output a red color for debugging
        return float4(1, 0, 0, 1);
    }

    // No collision - output transparent color
    return float4(0, 0, 0, 0);
}

technique CollisionCheck
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}*/