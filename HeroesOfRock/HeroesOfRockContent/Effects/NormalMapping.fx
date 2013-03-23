float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;

// ***** Light properties *****
//float3 LightPosition;
float4 LightColor;
float4 AmbientLightColor;
float3 theLightDirection;
// ****************************

// ***** material properties *****

// output from phong specular will be scaled by this amount
float Shininess;

// specular exponent from phong lighting model.  controls the "tightness" of
// specular highlights.
float SpecularPower;
// *******************************

texture2D NormalMap;
sampler2D NormalMapSampler = sampler_state
{
    Texture = <NormalMap>;
    MinFilter = linear;
    MagFilter = linear;
    MipFilter = linear;
};

texture2D Texture;
sampler2D DiffuseTextureSampler = sampler_state
{
    Texture = <Texture>;
    MinFilter = linear;
    MagFilter = linear;
    MipFilter = linear;
};

struct VS_INPUT
{
    float4 position            : POSITION0;
    float2 texCoord            : TEXCOORD0;
    float3 normal            : NORMAL0;    
    float3 binormal            : BINORMAL0;
    float3 tangent            : TANGENT0;
};

struct VS_OUTPUT
{
    float4 position            : POSITION0;
    float2 texCoord            : TEXCOORD0;
    float3 Normal : TEXCOORD1;
    float3 Tangent    : TEXCOORD2;
    float3 Binormal    : TEXCOORD3;
	float3 viewDirection : TEXCOORD4;
};

VS_OUTPUT VertexShaderFunction( VS_INPUT input )
{
    VS_OUTPUT output;
    
    // transform the position into projection space
    float4 worldSpacePos = mul(input.position, World);
    output.position = mul(worldSpacePos, View);
    output.position = mul(output.position, Projection);

	float3 eyePosition = mul(-View._m30_m31_m32, transpose(View));    
    output.viewDirection = worldSpacePos - eyePosition;   
	
	output.Normal = normalize(mul(input.normal, WorldInverseTranspose));
	output.Tangent = normalize(mul(input.tangent, WorldInverseTranspose));
	output.Binormal = normalize(mul(input.binormal, WorldInverseTranspose));

	output.texCoord = input.texCoord;
	return output;
}

float4 PixelShaderFunction( VS_OUTPUT input ) : COLOR0
{
	// look up the normal from the normal map, and transform from tangent space
    // into world space using the matrix created above.  normalize the result
    // in case the matrix contains scaling.
    float3 normalFromMap = tex2D(NormalMapSampler, input.texCoord);	
	normalFromMap = input.Normal + (normalFromMap.x * input.Tangent + normalFromMap.y * input.Binormal);
    //normalFromMap = mul(normalFromMap, input.tangentToWorld);
    normalFromMap = normalize(normalFromMap);
    
    // clean up our inputs a bit
    input.viewDirection = normalize(input.viewDirection);
    
    // use the normal we looked up to do phong diffuse style lighting.    
    float nDotL = max(dot(normalFromMap, theLightDirection), 0);
    float4 diffuse = LightColor * nDotL;
    
    // use phong to calculate specular highlights: reflect the incoming light
    // vector off the normal, and use a dot product to see how "similar"
    // the reflected vector is to the view vector.    
    float3 reflectedLight = reflect(theLightDirection, normalFromMap);
    float rDotV = max(dot(reflectedLight, input.viewDirection), 0.00001f);
    float4 specular = Shininess * LightColor * pow(rDotV, SpecularPower);
    
    float4 diffuseTexture = tex2D(DiffuseTextureSampler, input.texCoord);
    
    // return the combined result.	
    return (diffuse + AmbientLightColor) * diffuseTexture + specular;	
}

Technique NormalMapping
{
    Pass Go
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}