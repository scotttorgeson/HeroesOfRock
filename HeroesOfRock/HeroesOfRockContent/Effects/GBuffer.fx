float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldViewIT;

texture Texture;
texture NormalMap;
texture SpecularMap;

sampler AlbedoSampler = sampler_state
{
	texture = <Texture>;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
};

sampler NormalSampler = sampler_state
{
	texture = <NormalMap>;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
};

sampler SpecularSampler = sampler_state
{
	texture = <SpecularMap>;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
};

// vertex shader input
struct VSI
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 UV : TEXCOORD0;
	float3 Tangent : TANGENT0;
	float3 BiTangent : BINORMAL0;
};

// vertex shader output (pixel shader input)
struct VSO
{
	float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
	float3 Depth : TEXCOORD1;
	float3x3 TBN : TEXCOORD2; // tanget bitangent normal
};

// pixel shader output (pixel shader to render target)
struct PSO
{
	float4 Albedo : COLOR0; // stores color, W stores specular intensity
	float4 Normals : COLOR1; // view space normals, W stores specular power
	float4 Depth : COLOR2; // screen space depth for lighting and view space depth for SSAO
};

VSO VS(VSI input)
{
	//Initialize Output
	VSO output;

	//Transform Position
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	
	//Pass Depth
	output.Depth.x = output.Position.z;
	output.Depth.y = output.Position.w;
	output.Depth.z = viewPosition.z;

	//Build TBN Matrix
	output.TBN[0] = normalize(mul(input.Tangent, (float3x3)WorldViewIT));
	output.TBN[1] = normalize(mul(input.BiTangent, (float3x3)WorldViewIT));
	output.TBN[2] = normalize(mul(input.Normal, (float3x3)WorldViewIT));

	//Pass UV
	output.UV = input.UV;

	//Return Output
	return output;
}

//Normal Encoding Function
half3 encode(half3 n)
{
	n = normalize(n);

	n.xyz = 0.5f * (n.xyz + 1.0f);

	return n;
}

//Normal Decoding Function
half3 decode(half4 enc)
{
	return (2.0f * enc.xyz - 1.0f);
}

PSO PS(VSO input)
{
	//Initialize Output
	PSO output;

	//Pass Albedo from Texture
	output.Albedo = tex2D(AlbedoSampler, input.UV);
	
	//Pass Extra - Can be whatever you want, in this case will be a Specular Value
	output.Albedo.w = tex2D(SpecularSampler, input.UV).x;

	//Read Normal From Texture
	half3 normal = tex2D(NormalSampler, input.UV).xyz * 2.0f - 1.0f;

	//Transform Normal to WorldViewSpace from TangentSpace
	normal = normalize(mul(normal, input.TBN));

	//Pass Encoded Normal
	//output.Normals.xyz = encode(normal);
	
	//Pass this instead to disable normal mapping
	output.Normals.xyz = encode(normalize(input.TBN[2]));

	//Pass Extra - Can be whatever you want, in this case will be a Specular Value
	output.Normals.w = tex2D(SpecularSampler, input.UV).y;

	//Pass Depth(Screen Space, for lighting)
	output.Depth = input.Depth.x / input.Depth.y;

	//Pass Depth(View Space, for SSAO)
	output.Depth.g = input.Depth.z;

	//Return Output
	return output;
}

technique Default
{
	pass p0
	{
		VertexShader = compile vs_3_0 VS();
		PixelShader = compile ps_3_0 PS();
	}
}