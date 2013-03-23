float4x4 World;
float4x4 View;
float4x4 Projection;
float3 LightPosition;
float DepthPrecision; //This is for modulating the Light's Depth Precision, may need to play with this value to see shadows correctly

struct VSI
{
	float4 Position : POSITION0;
};

struct VSO
{
	float4 Position : POSITION0;
	float4 WorldPosition : TEXCOORD0;
};

VSO VS(VSI input)
{
	//Initialize Output
	VSO output;

	//Transform Position
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	//Pass World Position
	output.WorldPosition = worldPosition;

	//Return Output
	return output;
}

float4 PS(VSO input) : COLOR0
{
	//Fix World Position
	input.WorldPosition /= input.WorldPosition.w;

	//Calculate Depth from Light
	float depth = max(0.01f, length(LightPosition - input.WorldPosition)) / DepthPrecision;

	//Return Exponential of Depth
	return exp((DepthPrecision * 0.5f) * depth);
}

technique Default
{
	pass p0
	{
		VertexShader = compile vs_3_0 VS();
		PixelShader = compile ps_3_0 PS();
	}
}