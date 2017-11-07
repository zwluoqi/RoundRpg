Shader "CustomClipIconColorless"
{
	Properties
	{
		_MainTex("Base (RGB,A)", 2D) = "white" {}
		_AlphaTex("Alpha (A)", 2D) = "white" {}
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f 
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float4 _MainTex_ST;

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = v.color;
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				fixed4 col1 = tex2D(_MainTex, i.texcoord)  * i.color;
				fixed x = col1.r / 3.0 + col1.g / 3.0 + col1.b / 3.0;
				fixed4 col2 = tex2D(_AlphaTex, i.texcoord);
				fixed4 col3 = fixed4(x, x, x, col1.a * col2.a);
				return col3;
			}
			ENDCG
		}
	}

	FallBack "Unlight/Texture"
}
