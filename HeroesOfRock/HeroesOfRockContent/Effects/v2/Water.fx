
#define SAMPLE_TEXTURE(Name, texCoord)  tex2D(Name##Sampler, texCoord)

// specular parameters
float SpecularPower;
float Shininess;

// general parameters
float4x4 World;
float4x4 View;
float4x4 Projection;
float3x3 WorldInverseTranspose;

// light parameters
float4 AmbientLightColor;
float3 theLightDirection;
float4 WaterColor;
float4 WaterBase = float4(0.6f,0.6f,0.6f,1.0f);

float Time;

texture2D NormalMap;
sampler NormalMapSampler : register(s0) = sampler_state { Texture = (NormalMap); AddressU = Wrap; AddressV = Wrap; };

struct PS_IN
{
    float4 Position			: POSITION0;
    float2 TexCoord			: TEXCOORD0;
    float3 Normal			: TEXCOORD1;
    float3 Tangent			: TEXCOORD2;
    float3 Binormal			: TEXCOORD3;
	float3 ViewDirection	: TEXCOORD4;
	float4 WorldPos			: TEXCOORD5;
};

struct VS_IN
{
	float4	Position	: POSITION0;
    float3	Normal		: NORMAL;
    float2	TexCoord	: TEXCOORD0;
	float3	Binormal	: BINORMAL0;
	float3	Tangent		: TANGENT0;
};

#define CALC_NORM_TAN_BINORM \
	output.Normal = normalize(mul(input.Normal, WorldInverseTranspose)); \
	output.Tangent = normalize(mul(input.Tangent, WorldInverseTranspose)); \
	output.Binormal = normalize(mul(input.Binormal, WorldInverseTranspose));

// world space pos = input.position * World
float3 CalculateViewDirection( in float4 worldSpacePos )
{
    return worldSpacePos - mul(-View._m30_m31_m32, transpose(View));
}

// basic vertex shader function, for unskinned models
PS_IN VertexShaderFunction( VS_IN input )
{
	PS_IN output;
    
    // transform the position into projection space
    output.WorldPos = mul(input.Position, World);
    output.Position = mul(output.WorldPos, View);
    output.Position = mul(output.Position, Projection);

	output.ViewDirection = CalculateViewDirection( output.WorldPos );
	
	CALC_NORM_TAN_BINORM

	output.TexCoord = input.TexCoord;
	return output;
}

float4 PixelShaderFunction( PS_IN input ) : COLOR0
{
	// look up the normal from the normal map, and transform from tangent space
    // into world space using the matrix created above.  normalize the result
    // in case the matrix contains scaling.
	
	float2 texCoord = input.TexCoord * 3.5f;
	texCoord.x = texCoord.x + Time * 0.02f;
	texCoord.y = texCoord.y + Time * 0.026f;
    float3 normalFromMap1 = SAMPLE_TEXTURE(NormalMap, texCoord);
	//normalFromMap1 *= 1.2f;	
	//normalFromMap1 = input.Normal + (normalFromMap1.x * input.Tangent + normalFromMap1.y * input.Binormal);

	float2 texCoord1 = input.TexCoord * 2.0f;
	texCoord1.x = texCoord1.x + Time * 0.023f;
	texCoord1.y = texCoord1.y + Time * 0.0299f;
	float3 normalFromMap2 = SAMPLE_TEXTURE(NormalMap, texCoord1);
	//normalFromMap2 *= 1.2f;	
	//normalFromMap2 = input.Normal + (normalFromMap2.x * input.Tangent + normalFromMap2.y * input.Binormal);

    float3 normalFromMap = normalize((normalFromMap1 + normalFromMap2) * 0.5f);
    
    // clean up our inputs a bit
    input.ViewDirection = normalize(input.ViewDirection);
    
    // use the normal we looked up to do phong diffuse style lighting.    
    float nDotL = max(dot(normalFromMap, theLightDirection), 0);
    float4 diffuse = nDotL;
    
    // use phong to calculate specular highlights: reflect the incoming light
    // vector off the normal, and use a dot product to see how "similar"
    // the reflected vector is to the view vector.    
    float3 reflectedLight = reflect(theLightDirection, normalFromMap);
    float rDotV = max(dot(reflectedLight, input.ViewDirection), 0.00001f);
    float4 specular = Shininess * pow(rDotV, SpecularPower);
	
	
    
    // return the combined result.
	
    return ((diffuse + WaterBase) * WaterColor) + specular;
}

Technique Draw
{
    Pass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader  = compile ps_2_0 PixelShaderFunction();
    }
}
