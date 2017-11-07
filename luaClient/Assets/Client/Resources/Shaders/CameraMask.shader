Shader "Unlit/CameraMask"
{
	Properties
	{
		//_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		//ZWrite off

		Pass
		{
			Tags 
			{ 
				"RenderType"="Transparent" 
				"Queue" = "Transparent-100"
				"IngoreProjector" = "True"
			}
			
			

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				//float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				//float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			//sampler2D _MainTex;
			//float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{			
				return fixed4(0,0,0,0.4);
			}
			ENDCG
		}
	}
}
