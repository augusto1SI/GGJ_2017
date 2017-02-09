Shader "Custom/WateryFragMultiplicative"
{
	Properties
	{
		[NoScaleOffset]_RadialGradient ("Radial Gradient", 2D) = "white" {}
		[NoScaleOffset]_CloudTexture ("Cloud Texture", 2D) = "white" {}

		_Color1 ("Color 1", color) = (1,1,1,1)
		_Color2 ("Color 2", color) = (1,1,1,1)
	}
    SubShader
    {
    	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend DstColor Zero
		Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
    
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
            
            // texture we will sample
            sampler2D _RadialGradient;
            sampler2D _CloudTexture;
            fixed4 _Color1;
            fixed4 _Color2;

            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed _radial = tex2D(_RadialGradient, i.uv).x;
                fixed _clouds = tex2D(_CloudTexture, i.uv).x;
                fixed _invClouds = 1 - _clouds;
                return (((_clouds * _Color1) + (_invClouds * _Color2)) * _radial) + (1-_radial);
            }
            ENDCG
        }
    }
}
