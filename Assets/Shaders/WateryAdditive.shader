Shader "Custom/WateryAdditive"
{
	Properties
	{
		_RadialGradient ("Radial Gradient", 2D) = "white" {}
		_CloudTexture ("Cloud Texture", 2D) = "white" {}

		_Color1 ("Color 1", color) = (1,1,1,1)
		_Color2 ("Color 2", color) = (1,1,1,1)
	}
	SubShader
	{
		Tags
		{
			"RenderType"="Transparent"
			"Queue"="Transparent"
		}
		LOD 200
		Blend One One
		
		CGPROGRAM
		#pragma surface surf Standard
		#pragma target 3.0

		sampler2D _RadialGradient;
		sampler2D _CloudTexture;

		struct Input
		{
			float2 uv_RadialGradient;
		};

		fixed4 _Color1;
		fixed4 _Color2;

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			half _radial = tex2D (_RadialGradient, IN.uv_RadialGradient).x;
			half _clouds = tex2D (_CloudTexture, IN.uv_RadialGradient).x;
			half _invClouds = 1 - _clouds;
			o.Emission = ((_clouds * _Color1.rgb) + (_invClouds * _Color2.rgb)) * _radial;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
