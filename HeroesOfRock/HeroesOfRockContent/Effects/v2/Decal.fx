float4x4 World;
float4x4 View;
float4x4 Projection;
float3x3 WorldInverseTranspose;

float4x4 DecalMatrix;

float DepthBias = 0.00262f;
float SMAP_SIZE = 1024.0f;
float texelSize;
float4 AmbientLightColor;

#define NUM_CASCADES 3
float4x4 LightViewProjs[NUM_CASCADES];
float2 ClipPlanes[NUM_CASCADES];

texture DecalTexture;
sampler DecalTextureSampler = sampler_state { Texture = (DecalTexture); AddressU = Clamp; AddressV = Clamp; };

texture2D ShadowMap;
sampler ShadowMapSampler : register(s1) = sampler_state { Texture = (ShadowMap); };

float ShadowMapLookup(float2 texCoord, float2 offset, float depth)
{
	return (tex2D(ShadowMapSampler, (texCoord + offset * texelSize)).r < depth) ? 0.5f : 1.0f;
}

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float3 Normal : NORMAL;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 projectedUV : TEXCOORD0;
	float3 Normal : TEXCOORD1;
	float4 Position2D : TEXCOORD2;
	float4 WorldPos : TEXCOORD3;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	output.WorldPos = mul(input.Position, World);
    float4 viewPosition = mul(output.WorldPos, View);
    output.Position = mul(viewPosition, Projection);

	output.projectedUV = mul(output.WorldPos, DecalMatrix);
	output.Normal = normalize(mul(input.Normal, WorldInverseTranspose));
	output.Position2D = output.Position;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{	
	// fix up tex coords so we draw centered
	input.projectedUV.x += 0.5f;
	input.projectedUV.y += 0.5f;
	
	if ( input.projectedUV.x >= 0.0f && input.projectedUV.y >= 0.0f && input.projectedUV.x <= 1.0f && input.projectedUV.y <= 1.0f )
	{
		float shadowCoeff = 1.0f;
		float viewdepth = 1.0f - (input.Position2D.z / input.Position2D.w);
		int shadowMapIndex = 0;
		for ( int i = 0; i < NUM_CASCADES; i++ )
		{
			float near = ClipPlanes[i].x;
			float far = ClipPlanes[i].y;
			if ( viewdepth >= near && viewdepth < far )
			{			
				shadowMapIndex = i;
				break;
			}
		}

		// Shadow stuff
		// Find the position of this pixel in light space
		float4 lightingPosition = mul(input.WorldPos, LightViewProjs[shadowMapIndex]);    

		// Calculate the current pixel depth
		// The bias is used to prevent floating point errors that occur when
		// the pixel of the occluder is being drawn
		float ourdepth = (lightingPosition.z / lightingPosition.w) - DepthBias;	

		// Find the position in the shadow map for this pixel
		float2 ShadowTexCoord = 0.5 * lightingPosition.xy / lightingPosition.w + float2( 0.5, 0.5 );
		ShadowTexCoord.y = 1.0f - ShadowTexCoord.y;
		ShadowTexCoord.x *= (1.0f / (float)NUM_CASCADES);
		ShadowTexCoord.x += ( (float)shadowMapIndex / (float)NUM_CASCADES );

		// Get the current depth stored in the shadow map
		float shadowdepth = tex2D(ShadowMapSampler, ShadowTexCoord).r;
	    
		float4 diffuseTexture = tex2D(DecalTextureSampler, input.projectedUV);

		// Check to see if this pixel is in front or behind the value in the shadow map
		if (shadowdepth < ourdepth)
		{
			shadowCoeff = ShadowMapLookup(ShadowTexCoord, float2(0.0f, 0.0f), ourdepth);
		}
		
		return diffuseTexture * shadowCoeff;
	}
	else
	{
		return float4(0.0f, 0.0f, 0.0f, 0.0f);
	}	
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
