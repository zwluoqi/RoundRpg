Shader "BloodBarbias"
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
		Cull off
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
		half2 texcoord1 : TEXCOORD1;
		fixed4 color : COLOR;
	};

	sampler2D _MainTex;
	sampler2D _AlphaTex;
	float4 _MainTex_ST;
	float4 _AlphaTex_ST;

	v2f vert(appdata_t v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
		o.texcoord1 = TRANSFORM_TEX(v.texcoord, _AlphaTex);
		o.color = v.color;
		return o;
	}

	fixed4 frag(v2f i) : COLOR
	{
		fixed4 col1 = tex2D(_MainTex, i.texcoord)  * i.color;
		fixed4 col2 = tex2D(_AlphaTex, i.texcoord1);
		col1.a = col1.a * col2.a;
		return col1;
	}
		ENDCG
	}
	}

		FallBack "Unlight/Texture"
}
