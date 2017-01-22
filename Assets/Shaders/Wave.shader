Shader "Custom/Wave"
{
	Properties
	{
		_Color1 ("Wave Color Main", Color) = (1,1,1,1)
		_Color2 ("Wave Color Secondary", Color) = (1,1,1,1)
		_ColorGlow1 ("Glow Color 1", Color) = (1,1,1,1)
		_ColorGlow2 ("Glow Color 2", Color) = (1,1,1,1)
		_NoiseTex ("Noise Texture", 2D) = "white" {}
		_MainWave ("Main Wave Texture", 2D) = "white" {}
		_FalloffTex ("Falloff Texture", 2D) = "white" {}
		_GlowTex ("Glow Texture", 2D) = "white" {}

		_NoiseAmount ("Noise amount", float) = 0
		_NoiseTile ("Noise tile", float) = 1
		_NoiseOffset ("Noise offset", float) = 0
	}
	SubShader
	{
		Tags
		{
			"RenderType"="Transparent"
			"Queue"="Transparent"
		}
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma surface surf Standard vertex:vert alpha:blend
		#pragma target 3.0

		sampler2D _GlowTex;
		sampler2D _NoiseTex;
		fixed _NoiseAmount;
		fixed _NoiseOffset;
		half _NoiseTile;

		struct Input
		{
			float2 uv_GlowTex;
		};

		void vert (inout appdata_full v)
		{
			fixed surface = (fixed)tex2Dlod (_NoiseTex, float4((v.texcoord.x * _NoiseTile) - _NoiseOffset, v.texcoord.y, 0, 0)).x;
			surface *= 2;
			surface -= 1;
			v.vertex.xyz += v.normal * _NoiseAmount * surface;
		}

		fixed4 _ColorGlow1;
		fixed4 _ColorGlow2;

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			half3 _g = tex2D (_GlowTex, IN.uv_GlowTex);
			half _red = clamp(1,0,_g.r - _g.g);
			o.Emission = (_ColorGlow1.rgb*(1-_g.g)) + (_ColorGlow2 * _g.g);
			o.Alpha = clamp(1,0,_g.r+_g.g);
		}
		ENDCG

		Tags
		{
			"RenderType"="Transparent"
			"Queue"="Transparent"
		}
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma surface surf Standard vertex:vert alpha:blend
		#pragma target 3.0

		sampler2D _NoiseTex;
		sampler2D _MainWave;
		sampler2D _FalloffTex;
		fixed _NoiseAmount;
		fixed _NoiseOffset;
		half _NoiseTile;

		struct Input
		{
			float2 uv_NoiseTex;
			float2 uv_MainWave;
			float2 uv_FalloffTex;
		};

		void vert (inout appdata_full v)
		{
			fixed surface = (fixed)tex2Dlod (_NoiseTex, float4((v.texcoord.x * _NoiseTile) + _NoiseOffset, v.texcoord.y, 0, 0)).x;
			surface *= 2;
			surface -= 1;
			v.vertex.xyz += v.normal * _NoiseAmount * surface;
		}

		fixed4 _Color1;
		fixed4 _Color2;

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			half _main = tex2D (_MainWave, IN.uv_MainWave).x;
			half _a = tex2D (_FalloffTex, IN.uv_FalloffTex).x;
			half _invMain = 1 - _main;
			half3 _ColorMain = _Color1.rgb * _main;
			half3 _ColorSub = _Color2.rgb * _invMain;
			o.Emission = _ColorMain + _ColorSub;
			o.Alpha = _a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
