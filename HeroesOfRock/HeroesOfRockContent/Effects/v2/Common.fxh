// macros for easy texture sampling
#define DECLARE_TEXTURE(Name, index) \
    texture2D Name; \
    sampler Name##Sampler : register(s##index) = sampler_state { Texture = (Name); };

#define DECLARE_CUBEMAP(Name, index) \
    textureCUBE Name; \
    sampler Name##Sampler : register(s##index) = sampler_state { Texture = (Name); };

#define SAMPLE_TEXTURE(Name, texCoord)  tex2D(Name##Sampler, texCoord)
#define SAMPLE_CUBEMAP(Name, texCoord)  texCUBE(Name##Sampler, texCoord)

// specular parameters
float SpecularPower;
float Shininess;

// general parameters
float4x4 World;
float4x4 View;
float4x4 Projection;
float3x3 WorldInverseTranspose;

// light parameters
bool receivesShadows = true;
float4 LightColor;
float4 AmbientLightColor;
float3 theLightDirection;
float DepthBias = 0.00166996f;
float SMAP_SIZE = 1024.0f;
float texelSize;
float4x4 LightViewProj;

#define NUM_CASCADES 3
float4x4 LightViewProjs[NUM_CASCADES];
float2 ClipPlanes[NUM_CASCADES];

// diffuse is texture index 0, shadow map is index 1, normal map is index 2, specular map is index 3
texture2D Diffuse;
sampler DiffuseSampler : register(s0) = sampler_state { Texture = (Diffuse); AddressU = Wrap; AddressV = Wrap; };
//DECLARE_TEXTURE(Diffuse, 0)
DECLARE_TEXTURE(ShadowMap, 1)

#if DO_NORMAL_MAP
//DECLARE_TEXTURE(NormalMap, 2)
texture2D NormalMap;
sampler NormalMapSampler : register(s2) = sampler_state { Texture = (NormalMap); AddressU = Wrap; AddressV = Wrap; };
#endif

#if DO_SPECULAR_MAP
DECLARE_TEXTURE(SpecularMap, 3)
#endif

struct PS_IN
{
    float4 Position			: POSITION0;
    float2 TexCoord			: TEXCOORD0;
    float3 Normal			: TEXCOORD1;
    float3 Tangent			: TEXCOORD2;
    float3 Binormal			: TEXCOORD3;
	float3 ViewDirection	: TEXCOORD4;
	float4 WorldPos			: TEXCOORD5;
	float4 Position2D		: TEXCOORD6;
};

struct VS_IN
{
	float4	Position	: POSITION0;
    float3	Normal		: NORMAL;
    float2	TexCoord	: TEXCOORD0;
	float3	Binormal	: BINORMAL0;
	float3	Tangent		: TANGENT0;
};

struct CreateShadowMap_PS_IN
{
    float4 Position : POSITION;
    float4 Position2D : TEXCOORD0;
};

// Transforms the model into light space
CreateShadowMap_PS_IN CreateShadowMap_VertexShader(float4 Position: POSITION) // not skinned
{
    CreateShadowMap_PS_IN Out;
    Out.Position = mul(Position, mul(World, LightViewProj)); 
	Out.Position2D = Out.Position; 
    return Out;
}

// Saves the depth value out to the 32bit floating point texture
float4 CreateShadowMap_PixelShader(CreateShadowMap_PS_IN input) : COLOR0
{
	float depth = input.Position2D.z / input.Position2D.w;
	return float4( depth, depth, depth, 1.0f );
}

#define CALC_NORM_TAN_BINORM \
	output.Normal = normalize(mul(input.Normal, WorldInverseTranspose)); \
	output.Tangent = normalize(mul(input.Tangent, WorldInverseTranspose)); \
	output.Binormal = normalize(mul(input.Binormal, WorldInverseTranspose));

// world space pos = input.position * World
float3 CalculateViewDirection( in float4 worldSpacePos )
{
    return worldSpacePos - mul(-View._m30_m31_m32, transpose(View));
}

float ShadowMapLookup(float2 texCoord, float2 offset, float depth)
{
	return (SAMPLE_TEXTURE(ShadowMap, (texCoord + offset * texelSize)).r < depth) ? 0.0f : 1.0f;
}

// basic vertex shader function, for unskinned models
PS_IN VertexShaderFunction( VS_IN input )
{
	PS_IN output;
    
    // transform the position into projection space
    output.WorldPos = mul(input.Position, World);
    output.Position = mul(output.WorldPos, View);
    output.Position = mul(output.Position, Projection);
	output.Position2D = output.Position;

	output.ViewDirection = CalculateViewDirection( output.WorldPos );
	
	CALC_NORM_TAN_BINORM

	output.TexCoord = input.TexCoord;
	return output;
}

float4 PixelShaderFunction( PS_IN input ) : COLOR0
{
#if DO_NORMAL_MAP
	// look up the normal from the normal map, and transform from tangent space
    // into world space using the matrix created above.  normalize the result
    // in case the matrix contains scaling.
	
    float3 normalFromMap = SAMPLE_TEXTURE(NormalMap, input.TexCoord);	
	normalFromMap = input.Normal + (normalFromMap.x * input.Tangent + normalFromMap.y * input.Binormal);
    normalFromMap = normalize(normalFromMap);
#else
	float3 normalFromMap = input.Normal;
#endif // DO_NORMAL_MAP
    
    // clean up our inputs a bit
    input.ViewDirection = normalize(input.ViewDirection);
    
    // use the normal we looked up to do phong diffuse style lighting.    
    float nDotL = max(dot(normalFromMap, theLightDirection), 0);
    float4 diffuse = LightColor * nDotL;
    
    // use phong to calculate specular highlights: reflect the incoming light
    // vector off the normal, and use a dot product to see how "similar"
    // the reflected vector is to the view vector.    
    float3 reflectedLight = reflect(theLightDirection, normalFromMap);
    float rDotV = max(dot(reflectedLight, input.ViewDirection), 0.00001f);
    float4 specular = Shininess * LightColor * pow(rDotV, SpecularPower);

	#if DO_SPECULAR_MAP
		specular = specular * SAMPLE_TEXTURE(SpecularMap, input.TexCoord);
	#endif
    
	float4 diffuseTexture = SAMPLE_TEXTURE(Diffuse, input.TexCoord);
	float shadowCoeff = 1.0f;

	if ( receivesShadows )
	{
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
		float shadowdepth = SAMPLE_TEXTURE(ShadowMap, ShadowTexCoord).r;
	    
		// Check to see if this pixel is in front or behind the value in the shadow map
		if (shadowdepth < ourdepth)
		{
			//float2 texelpos = SMAP_SIZE * ShadowTexCoord.xy;
			//float2 lerps = frac( texelpos );
		
			// 3x3 pcf
			/*
			shadowCoeff += ShadowMapLookup(ShadowTexCoord, float2(0.0f, 0.0f), ourdepth);// * 9.0f;
			shadowCoeff += ShadowMapLookup(ShadowTexCoord, float2(0.0f, 1.0f), ourdepth);
			shadowCoeff += ShadowMapLookup(ShadowTexCoord, float2(0.0f, -1.0f), ourdepth);

			shadowCoeff += ShadowMapLookup(ShadowTexCoord, float2(1.0f, 0.0f), ourdepth);
			shadowCoeff += ShadowMapLookup(ShadowTexCoord, float2(1.0f, 1.0f), ourdepth);
			shadowCoeff += ShadowMapLookup(ShadowTexCoord, float2(1.0f, -1.0f), ourdepth);

			shadowCoeff += ShadowMapLookup(ShadowTexCoord, float2(-1.0f, 0.0f), ourdepth);
			shadowCoeff += ShadowMapLookup(ShadowTexCoord, float2(-1.0f, 1.0f), ourdepth);
			shadowCoeff += ShadowMapLookup(ShadowTexCoord, float2(-1.0f, -1.0f), ourdepth);
			shadowCoeff /= 9.0f;
			*/
			//shadowCoeff = min( 1.0f, shadowCoeff );		

			// plain sample
			shadowCoeff = ShadowMapLookup(ShadowTexCoord, float2(0.0f, 0.0f), ourdepth);
		}
	}
    
    // return the combined result.
	return ( AmbientLightColor * diffuseTexture ) + ( ( ( diffuse * diffuseTexture ) + specular ) * shadowCoeff );
}