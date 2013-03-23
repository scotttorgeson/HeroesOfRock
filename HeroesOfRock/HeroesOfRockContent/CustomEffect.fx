float4x4 World;
float4x4 View;
float4x4 Projection;

//#if SHADOWS
float4x4 LightViewProj;

texture Texture;
sampler TextureSampler = sampler_state
{
    Texture = (Texture);
};

float4 AmbientColor = float4(0.15, 0.15, 0.15, 0);
float DepthBias = 0.0005f;
float3 LightPosition;

float SMAP_SIZE = 1024.0f;
float texelSize;

texture ShadowMap;
sampler ShadowMapSampler = sampler_state
{
    Texture = <ShadowMap>;
};

struct DrawWithShadowMap_VSIn
{
    float4 Position : POSITION0;
    float3 Normal   : NORMAL0;
    float2 TexCoord : TEXCOORD0;
};

struct DrawWithShadowMap_VSOut
{
    float4 Position : POSITION0;
    float3 Normal   : TEXCOORD0;
    float2 TexCoord : TEXCOORD1;
    float4 WorldPos : TEXCOORD2;
};

struct CreateShadowMap_VSOut
{
    float4 Position : POSITION;
    float4 Position2D : TEXCOORD0;
};

// Transforms the model into light space an renders out the depth of the object
CreateShadowMap_VSOut CreateShadowMap_VertexShader(float4 Position: POSITION)
{
    CreateShadowMap_VSOut Out;
    Out.Position = mul(Position, mul(World, LightViewProj)); 
	Out.Position2D = Out.Position;
    //Out.Depth = Out.Position.z / Out.Position.w;    
    return Out;
}

// Saves the depth value out to the 32bit floating point texture
float4 CreateShadowMap_PixelShader(CreateShadowMap_VSOut input) : COLOR
{ 
	//float depth = input.Position2D.z / input.Position2D.w;
	//return float4( depth, depth, depth, depth);
    //return float4(input.Depth, input.Depth, input.Depth, 1.0f);
	return input.Position2D.z / input.Position2D.w;
}

// Draws the model with shadows
DrawWithShadowMap_VSOut DrawWithShadowMap_VertexShader(DrawWithShadowMap_VSIn input)
{
    DrawWithShadowMap_VSOut Output;

    float4x4 WorldViewProj = mul(mul(World, View), Projection);
    
    // Transform the models verticies and normal
    Output.Position = mul(input.Position, WorldViewProj);
    Output.Normal =  normalize(mul(input.Normal, World));
    Output.TexCoord = input.TexCoord;
    
    // Save the vertices postion in world space
    Output.WorldPos = mul(input.Position, World);
    
    return Output;
}

float ShadowMapLookup(sampler shadowMap, float2 texCoord, float2 offset, float depth)
{
	return (tex2D(shadowMap, texCoord + offset * texelSize).r + DepthBias < depth) ? 0.0f : 1.0f;
}

// Determines the depth of the pixel for the model and checks to see 
// if it is in shadow or not
float4 DrawWithShadowMap_PixelShader(DrawWithShadowMap_VSOut input) : COLOR
{ 
    // Color of the model
    float4 diffuseColor = tex2D(TextureSampler, input.TexCoord);
    // Intensity based on the direction of the light
	float diffuseIntensity = saturate(dot(-normalize(input.WorldPos - LightPosition), input.Normal));

    // Final diffuse color with ambient color added
    
    
    // Find the position of this pixel in light space
    float4 lightingPosition = mul(input.WorldPos, LightViewProj);
    
    // Find the position in the shadow map for this pixel
    float2 ShadowTexCoord = 0.5 * lightingPosition.xy / 
                            lightingPosition.w + float2( 0.5, 0.5 );
    ShadowTexCoord.y = 1.0f - ShadowTexCoord.y;

    // Get the current depth stored in the shadow map
    float shadowdepth = tex2D(ShadowMapSampler, ShadowTexCoord).r;    
    
    // Calculate the current pixel depth
    // The bias is used to prevent folating point errors that occur when
    // the pixel of the occluder is being drawn
    float ourdepth = (lightingPosition.z / lightingPosition.w) - DepthBias;	
	
	float shadowCoeff = 1.0f;
	    
    // Check to see if this pixel is in front or behind the value in the shadow map
    if (shadowdepth < ourdepth)
    {
		float2 texelpos = SMAP_SIZE * ShadowTexCoord.xy;
		float2 lerps = frac( texelpos );

		// 2x2 percentage closest filter.
		// more samples for better looking blur		
		/*float s0 = (tex2D(ShadowMapSampler, ShadowTexCoord).r + DepthBias < ourdepth) ? 0.0f : 1.0f;
		float s1 = (tex2D(ShadowMapSampler, ShadowTexCoord + float2(texelSize, 0.0f)).r + DepthBias < ourdepth) ? 0.0f : 1.0f;
		float s2 = (tex2D(ShadowMapSampler, ShadowTexCoord + float2(0.0f, texelSize)).r + DepthBias < ourdepth) ? 0.0f : 1.0f;
		float s3 = (tex2D(ShadowMapSampler, ShadowTexCoord + float2(texelSize, texelSize)).r + DepthBias < ourdepth) ? 0.0f : 1.0f;
		shadowCoeff = lerp( lerp( s0, s1, lerps.x ), lerp( s2, s3, lerps.x ), lerps.y );*/

		/*shadowCoeff += ShadowMapLookup(ShadowMapSampler, ShadowTexCoord, float2(0.0f, 0.0f), ourdepth);
		shadowCoeff += ShadowMapLookup(ShadowMapSampler, ShadowTexCoord, float2(1.0f, 0.0f), ourdepth);
		shadowCoeff += ShadowMapLookup(ShadowMapSampler, ShadowTexCoord, float2(2.0f, 0.0f), ourdepth);
	
		shadowCoeff += ShadowMapLookup(ShadowMapSampler, ShadowTexCoord, float2(0.0f, -1.0f), ourdepth);
		shadowCoeff += ShadowMapLookup(ShadowMapSampler, ShadowTexCoord, float2(1.0f, -1.0f), ourdepth);
		shadowCoeff += ShadowMapLookup(ShadowMapSampler, ShadowTexCoord, float2(2.0f, -1.0f), ourdepth);
	
		shadowCoeff += ShadowMapLookup(ShadowMapSampler, ShadowTexCoord, float2(0.0f, 1.0f), ourdepth);
		shadowCoeff += ShadowMapLookup(ShadowMapSampler, ShadowTexCoord, float2(1.0f, 1.0f), ourdepth);
		shadowCoeff += ShadowMapLookup(ShadowMapSampler, ShadowTexCoord, float2(2.0f, 1.0f), ourdepth);
	
		shadowCoeff /= 9.0f;*/

		float sum = 0;
		float x, y;
		for ( y = -1.5; y <= 1.5; y += 1.0 )
			for ( x = -1.5; x <= 1.5; x += 1.0 )
				sum += ShadowMapLookup(ShadowMapSampler, ShadowTexCoord, float2(x, y), ourdepth);
		shadowCoeff = sum / 16.0;		
		

		/*
		float s4 = (tex2D(ShadowMapSampler, ShadowTexCoord).r + DepthBias < ourdepth) ? 0.0f : 1.0f;					//0,0
		float s5 = (tex2D(ShadowMapSampler, ShadowTexCoord + float2(texelSize, 0.0f)).r + DepthBias < ourdepth) ? 0.0f : lerps.x; //1,0
		float s1 = (tex2D(ShadowMapSampler, ShadowTexCoord + float2(0.0f, texelSize)).r + DepthBias < ourdepth) ? 0.0f : 1 - lerps.y; //0,1
		float s2 = (tex2D(ShadowMapSampler, ShadowTexCoord + float2(texelSize, texelSize)).r + DepthBias < ourdepth) ? 0.0f : ( lerps.x + 1 - lerps.y ) / 2.0f;	//1,1
		float s3 = (tex2D(ShadowMapSampler, ShadowTexCoord + float2(-texelSize, 0.0f)).r + DepthBias < ourdepth) ? 0.0f : 1 - lerps.x;//-1,0
		float s7 = (tex2D(ShadowMapSampler, ShadowTexCoord + float2(0.0f, -texelSize)).r + DepthBias < ourdepth) ? 0.0f : lerps.y;//0,-1
		float s6 = (tex2D(ShadowMapSampler, ShadowTexCoord + float2(-texelSize, -texelSize)).r + DepthBias < ourdepth) ? 0.0f : 1 - lerps.x + lerps.y; //-1,-1
		float s0 = (tex2D(ShadowMapSampler, ShadowTexCoord + float2(-texelSize, texelSize)).r + DepthBias < ourdepth) ? 0.0f : 1 - lerps.x + 1 - lerps.y;  //-1,1
		float s8 = (tex2D(ShadowMapSampler, ShadowTexCoord + float2(texelSize, -texelSize)).r + DepthBias < ourdepth) ? 0.0f : lerps.x + lerps.y;	//1,-1		
		shadowCoeff = ( s0 + s1 + s2 + s3 + s4 + s5 + s6 +s7 +s8 ) / 18.0f;
		*/
		
		/*
		shadowCoeff = tex2Dproj(ShadowMapSampler, ShadowTexCoord);
		shadowCoeff += tex2Dproj(ShadowMapSampler, ShadowTexCoord + float2(0.0, texelSize));
		shadowCoeff += tex2Dproj(ShadowMapSampler, ShadowTexCoord + float2(texelSize, 0));
		shadowCoeff += tex2Dproj(ShadowMapSampler, ShadowTexCoord + float2(texelSize, texelSize));
		shadwoCoeff /= 4.0f;
		*/
    };

	float4 diffuse = diffuseIntensity * diffuseColor * shadowCoeff + AmbientColor * diffuseColor;
    
    return diffuse;
}

float4 Editor_PixelShader(DrawWithShadowMap_VSOut input) : COLOR
{    
    return float4(0.0,1.0,0.0,0.0);
}

// Technique for creating the shadow map
technique CreateShadowMap
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 CreateShadowMap_VertexShader();
        PixelShader = compile ps_2_0 CreateShadowMap_PixelShader();
    }
}

// Technique for drawing with the shadow map
technique DrawWithShadowMap
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 DrawWithShadowMap_VertexShader();
        PixelShader = compile ps_3_0 DrawWithShadowMap_PixelShader();
    }
}

// Technique for drawing a selected item in the editor
technique Editor
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 DrawWithShadowMap_VertexShader();
        PixelShader = compile ps_3_0 Editor_PixelShader();
    }
}
//#endif

/*
struct Default_VSIn
{
    float4 Position : POSITION0;
    float3 Normal   : NORMAL0;
    float2 TexCoord : TEXCOORD0;
};

struct Default_VSOut
{
    float4 Position : POSITION0;
    float3 Normal   : TEXCOORD0;
    float2 TexCoord : TEXCOORD1;
    float4 WorldPos : TEXCOORD2;
};

// Draws the model with shadows
Default_VSOut DrawWithShadowMap_VertexShader(Default_VSIn input)
{
    Default_VSOut Output;

    float4x4 WorldViewProj = mul(mul(World, View), Projection);
    
    // Transform the models verticies and normal
    Output.Position = mul(input.Position, WorldViewProj);
    Output.Normal =  normalize(mul(input.Normal, World));
    Output.TexCoord = input.TexCoord;
    
    // Save the vertices postion in world space
    Output.WorldPos = mul(input.Position, World);
    
    return OUTPUT:
}*/