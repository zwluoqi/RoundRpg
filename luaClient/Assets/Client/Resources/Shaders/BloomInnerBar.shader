Shader "Unlit/BloomInnerBar"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_BloomValue("Bloom Value",Range(0,0.5))=0
		_ColorTint("MainColor",Color)=(1,1,1,1)
		
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _BloomValue;
			float4 _ColorTint;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv+float2(_BloomValue,0)) *  _ColorTint;// float4( (1-2*_BloomValue),(2*_BloomValue),0,1);			
				return col;
			}
			ENDCG
		}
	}
}
