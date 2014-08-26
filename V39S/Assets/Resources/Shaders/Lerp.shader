Shader "Transparent/Lerp" {

Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
    _Factor ("LerpFactor", Range (0,1)) = 0.5
    _MapA ("MapA", 2D) = "white" { }
    _MapB ("MapB", 2D) = "white" { }
  	_MapC ("MapС", 2D) = "white" { }
}

SubShader {

	Cull Off
	Tags { "RenderType"="Opaque" }
	LOD 300

    Pass {
        Fog { Mode Off }
CGPROGRAM

#pragma exclude_renderers d3d11 xbox360
#pragma vertex vert
#pragma fragment frag

uniform float _Factor;

sampler2D _MapA;
sampler2D _MapB;
sampler2D _MapC;

struct appdata {
    float4 vertex;
    float4 texcoord;
};

struct v2f {
    float4 pos   : POSITION;
    float2 uv    : TEXCOORD0;

};

v2f vert (appdata v) {
    v2f o;
    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
    o.uv = v.texcoord.xy;
    return o;
}
half4 frag( v2f i ) : COLOR {
     
    float3 TextureA = tex2D(_MapA, i.uv.xy) * cos(3.14f * _Factor) * 0.5f;
    float3 TextureB = tex2D(_MapB, i.uv.xy) * cos(3.14f * _Factor + 4.71f) * 0.5f;
	float3 TextureC = tex2D(_MapC, i.uv.xy) * cos(3.14f * _Factor + 3.14f) * 0.5f; 
	
	if(_Factor > 0.5f)
		TextureA = 0.0f;
	if(_Factor < 0.5)
		TextureC = 0.0f;

	float3 result = TextureA + TextureB + TextureC;

	return float4(result,1);
}
ENDCG
    }
}
}