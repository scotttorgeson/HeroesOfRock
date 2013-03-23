#define SKINNED_EFFECT_MAX_BONES 75
float4x3 Bones[SKINNED_EFFECT_MAX_BONES];

#include "Common.fxh"

struct SKINNED_VS_IN
{
    float4	Position	: SV_Position;
    float3	Normal		: NORMAL;
    float2	TexCoord	: TEXCOORD0;
    int4	Indices		: BLENDINDICES0;
    float4	Weights		: BLENDWEIGHT0;
	float3	Binormal	: BINORMAL0;
	float3	Tangent		: TANGENT0;
};

struct SKINNED_SHADOW_DEPTH_VS_IN
{
	float4	Position	: SV_Position;
    int4	Indices		: BLENDINDICES0;
    float4	Weights		: BLENDWEIGHT0;
};

void Skin(inout SKINNED_VS_IN vin, uniform int boneCount)
{
    float4x3 skinning = 0;

    [unroll]
    for (int i = 0; i < boneCount; i++)
    {
        skinning += Bones[vin.Indices[i]] * vin.Weights[i];
    }

    vin.Position.xyz = mul(vin.Position, skinning);
    vin.Normal = mul(vin.Normal, (float3x3)skinning);
	vin.Binormal = mul(vin.Binormal, (float3x3)skinning);
	vin.Tangent = mul(vin.Tangent, (float3x3)skinning);
}

void Skin(inout SKINNED_SHADOW_DEPTH_VS_IN vin, uniform int boneCount)
{
	float4x3 skinning = 0;

    [unroll]
    for (int i = 0; i < boneCount; i++)
    {
        skinning += Bones[vin.Indices[i]] * vin.Weights[i];
    }

    vin.Position.xyz = mul(vin.Position, skinning);
}

// Transforms the model into light space
CreateShadowMap_PS_IN CreateShadowMap_Skinned_VertexShader(SKINNED_SHADOW_DEPTH_VS_IN input)
{
    CreateShadowMap_PS_IN Out;
	Skin( input, 4 );
    Out.Position = mul(input.Position, mul(World, LightViewProj)); 
	Out.Position2D = Out.Position; 
    return Out;
}

PS_IN CommonSkinVS(SKINNED_VS_IN input)
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

PS_IN VSOneBone(SKINNED_VS_IN vin)
{
    Skin(vin, 1);    
	return CommonSkinVS(vin);
}

PS_IN VSTwoBone(SKINNED_VS_IN vin)
{
    Skin(vin, 2);
    return CommonSkinVS(vin);
}

PS_IN VSFourBone(SKINNED_VS_IN vin)
{
    Skin(vin, 4);
    return CommonSkinVS(vin);
}

Technique Draw
{
    Pass
    {
        VertexShader = compile vs_3_0 VSFourBone();
        PixelShader  = compile ps_3_0 PixelShaderFunction();
    }
}

Technique CreateShadowMap
{
	Pass
	{
		VertexShader = compile vs_2_0 CreateShadowMap_Skinned_VertexShader();
        PixelShader  = compile ps_2_0 CreateShadowMap_PixelShader();
	}
}