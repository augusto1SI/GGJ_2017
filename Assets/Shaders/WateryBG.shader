Shader "Custom/WateryBG"
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		_CloudTexture ("Cloud Texture", 2D) = "white" {}

		_WateryOffset ("Watery offset", float) = 0
	}
	SubShader
	{
		Tags
		{
			"RenderType"="Geometry"
		}
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _CloudTexture;

		half _WateryOffset;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_CloudTexture;
		};

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			half3 _main = tex2D (_MainTex,  IN.uv_MainTex).rgb;
			half3 _offMain = tex2D (_MainTex,  float2(IN.uv_MainTex.x + _WateryOffset, IN.uv_MainTex.y + _WateryOffset)).rgb;
			half _cloud = tex2D (_CloudTexture, IN.uv_CloudTexture).r;
			half _invCloud = 1 - _cloud;
			o.Emission = (_offMain * _invCloud) + (_main * _cloud);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
