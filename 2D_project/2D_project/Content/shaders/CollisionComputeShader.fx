Texture2D<int> maskA : register(t0);
Texture2D<int> maskB : register(t1);

RWTexture2D<int> collisionBuffer : register(u0);

[numthreads(16, 16, 1)]
void CSMain(uint3 id : SV_DispatchThreadID)
{
    int2 coords = int2(id.xy);

    // Read the masks at the current coordinates
    int valueA = maskA[coords];
    int valueB = maskB[coords];

    // Check if both masks have collidable pixels at this position
    int isColliding = (valueA > 0) && (valueB > 0) ? 1 : 0;

    // Write the result to the collision buffer
    collisionBuffer[coords] = isColliding;
}
