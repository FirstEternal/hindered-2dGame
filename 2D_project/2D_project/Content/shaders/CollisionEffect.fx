sampler2D maskA;
sampler2D maskB;

float4 MainPS(float2 texCoord : TEXCOORD0) : COLOR0
{
    float a = tex2D(maskA, texCoord).r; 
    float b = tex2D(maskB, texCoord).r;

    // Check for a collision (both masks should be non-zero)
    
    float isColliding = (a > 0.5 && b > 0.5) ? 1.0 : 0.0;

    return float4(isColliding, isColliding, isColliding, 1.0); // Output white for collision
}

technique CollisionDetection
{
    pass P0
    {
        PixelShader = compile ps_3_0 MainPS();
    }
}

sampler2D SpriteTexture : register(s0);
sampler2D MaskTexture : register(s1);

