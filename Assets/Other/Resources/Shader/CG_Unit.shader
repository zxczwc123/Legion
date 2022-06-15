Shader "CG/Unit" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_Fresnel ("Fresnel", Range(0, 0.7)) = 0
	_Power ("Power", Range(0.4, 5)) = 5
	_FresnelColor ("Fresnel Color", Color) = (1,1,1,1)
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 200
	Cull Off
	
CGPROGRAM
#pragma surface surf Lambert

sampler2D _MainTex;
fixed4 _Color;
float _Fresnel;
float _Power;
fixed4 _FresnelColor;

struct Input {
	float3 worldPos;
	float3 worldNormal;
	float2 uv_MainTex;
	float2 uv_Illum;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 c = tex * _Color;
	float3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - IN.worldPos);
	half fresnel =_Fresnel * pow(1 - dot(worldViewDir, IN.worldNormal), _Power);
	c.rgb = lerp(c.rgb, _FresnelColor.rgb, saturate(fresnel));
	o.Albedo = c.rgb;
	o.Emission = c.rgb;
	o.Alpha = c.a;
}
ENDCG
} 
FallBack "CG/Unit_VertexLit"
}
