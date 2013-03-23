#define DO_NORMAL_MAP 1

#include "Common.fxh"

Technique Draw
{
    Pass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader  = compile ps_3_0 PixelShaderFunction();
    }
}

Technique CreateShadowMap
{
	Pass
	{
		VertexShader = compile vs_2_0 CreateShadowMap_VertexShader();
        PixelShader  = compile ps_2_0 CreateShadowMap_PixelShader();
	}
}