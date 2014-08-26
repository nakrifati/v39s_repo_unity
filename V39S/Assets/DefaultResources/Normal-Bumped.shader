Shader "Bumped Diffuse" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)

	_FactorA ("LerpFactorA", float) = 0.5
	_FactorB ("LerpFactorB", float) = 0.5
	_FactorC ("LerpFactorC", float) = 0.5

	_MapA ("MapA (RGB)", 2D) = "white" {}
	_MapB ("MapB (RGB)", 2D) = "white" {}
	_MapC ("MapC (RGB)", 2D) = "white" {}

	_BumpMapA ("NormalmapA", 2D) = "bump" {}
	_BumpMapB ("NormalmapB", 2D) = "bump" {}
	_BumpMapC ("NormalmapC", 2D) = "bump" {}
}

SubShader {
	Cull Off
	Tags { "RenderType"="Opaque" }
	LOD 300

CGPROGRAM
#pragma surface surf Lambert

uniform float _FactorA;
uniform float _FactorB;
uniform float _FactorC;

sampler2D _MapA;
sampler2D _MapB;
sampler2D _MapC;

sampler2D _BumpMapA;
sampler2D _BumpMapB;
sampler2D _BumpMapC;

fixed4 _Color;

struct Input {
	float2 uv_MapA;
	float2 uv_MapB;
	float2 uv_MapC;
	float2 uv_BumpMapA;
	float2 uv_BumpMapB;
	float2 uv_BumpMapC;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c;
	
	c = tex2D(_MapA, IN.uv_MapA);
	o.Albedo = c.rgb * _FactorA * _Color;

	c = tex2D(_MapB, IN.uv_MapB);
	o.Albedo += c.rgb * _FactorB * _Color;

	c = tex2D(_MapC, IN.uv_MapC);
	o.Albedo += c.rgb * _FactorC * _Color;

	o.Alpha = c.a;

	o.Normal = UnpackNormal(tex2D(_BumpMapA, IN.uv_BumpMapA)) * _FactorA;
	o.Normal += UnpackNormal(tex2D(_BumpMapB, IN.uv_BumpMapB)) * _FactorB;
	o.Normal += UnpackNormal(tex2D(_BumpMapC, IN.uv_BumpMapC)) * _FactorC;
}
ENDCG  
}

FallBack "Diffuse"
}
