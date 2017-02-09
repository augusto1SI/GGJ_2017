Shader "Custom/WateryFragBG"
{
	Properties
	{
		[NoScaleOffset]_CloudTexture ("Cloud Texture", 2D) = "white" {}
		[NoScaleOffset]_FalloffTexture ("Falloff Texture", 2D) = "white" {}
		
		_ColorFalloff ("Color falloff", color) = (1,1,1,1)
		
		_Color1 ("Main Color 1", color) = (1,1,1,1)
		_Color2 ("Main Color 2", color) = (1,1,1,1)
		
		_CloudTexXTile1 ("Cloud Texture 1 X Tiling", float) = 1
		_CloudTexYTile1 ("Cloud Texture 1 Y Tiling", float) = 1
		
		_CloudTexXTile2 ("Cloud Texture 2 X Tiling", float) = 1
		_CloudTexYTile2 ("Cloud Texture 2 Y Tiling", float) = 1
		
		_CloudTexXOffset1 ("Cloud Texture 1 X Offset", float) = 1
		_CloudTexYOffset1 ("Cloud Texture 1 Y Offset", float) = 1
		
		_CloudTexXOffset2 ("Cloud Texture 2 X Offset", float) = 1
		_CloudTexYOffset2 ("Cloud Texture 2 Y Offset", float) = 1

		_WateryOffset ("Watery offset", float) = 0
		_AlphaMultiplier ("Alpha", range(0,1)) = 0.5
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Cull Off Lighting Off Fog { Color (0,0,0,0) }
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION; // vertex position
                float2 uv : TEXCOORD0; // texture coordinate
            };

            struct v2f
            {
                float2 uv : TEXCOORD0; // texture coordinate
                float4 vertex : SV_POSITION; // clip space position
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv = v.uv;
                return o;
            }

			sampler2D _CloudTexture;
			sampler2D _FalloffTexture;
			
			fixed4 _ColorFalloff;
			fixed4 _Color1;
			fixed4 _Color2;
		
			fixed _CloudTexXTile1;
			fixed _CloudTexYTile1;
			
			fixed _CloudTexXTile2;
			fixed _CloudTexYTile2;
		
			fixed _CloudTexXOffset1;
			fixed _CloudTexYOffset1;
			
			fixed _CloudTexXOffset2;
			fixed _CloudTexYOffset2;
			
			fixed _WateryOffset;
			fixed _AlphaMultiplier;

			fixed4 frag (v2f i) : SV_Target
            {
				half2 _cloudUV1;
				half2 _cloudUV2;
            	fixed4 _res;
            	_res.z = 1;
            	
            	_cloudUV1.x = (i.uv.x * _CloudTexXTile1) + _CloudTexXOffset1;
            	_cloudUV1.y = (i.uv.y * _CloudTexYTile1) + _CloudTexYOffset1;
            	
            	_cloudUV2.x = (i.uv.x * _CloudTexXTile2) + _CloudTexXOffset2;
            	_cloudUV2.y = (i.uv.y * _CloudTexYTile2) + _CloudTexYOffset2;
            	
                fixed  _falloff = tex2D (_FalloffTexture,     i.uv).x;
				fixed  _cloud1  = tex2D (_CloudTexture,  _cloudUV1).x;
				fixed  _cloud2  = tex2D (_CloudTexture,  _cloudUV2).x;
				
				fixed _cloud = _cloud1 * _cloud2;
				
				fixed  _invCloud = 1 - _cloud;
				fixed _invFalloff = 1 - _falloff;
				
				fixed _outerFalloff = ceil (_falloff);
				fixed _innerFalloff = floor(_falloff);
				fixed _rim = (1-_innerFalloff) * _outerFalloff * _falloff;
				
				_res.a = _invCloud * _falloff * _falloff * _falloff;
				_res.xyz = ((_invFalloff * _ColorFalloff) * _outerFalloff * _rim) + (_Color1 * _cloud * _falloff) + (_Color2 * _res.a);
				_res.a = 1 - (_res.a * _AlphaMultiplier);
				//_res.xyz = (_Color1 * _cloud * _falloff) + (_Color2 * _invCloud * _falloff * _falloff * _falloff);
                
                return _res;
            }
			ENDCG
		}
	}
	FallBack "Diffuse"
}
