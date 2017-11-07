// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_LightmapInd', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "UNOShader/_Library/Helpers/VertexUNLIT Transparent" 
{
	Properties
	{		
		_Color ("Color (A)Opacity", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags
		{
		}
		Offset -1.0,0
		Blend SrcAlpha OneMinusSrcAlpha // --- not needed when doing cutout
		Pass
			{
			Tags
			{
				"RenderType" = "Transparent"
				"Queue" = "Transparent"
				"LightMode" = "Vertex"
			}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			//#pragma multi_compile_fwdbase

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4x4 _MatrixDiffuse;

			struct customData
			{
				float4 vertex : POSITION;
				half3 normal : NORMAL;
				fixed2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};
			struct v2f // = vertex to fragment ( pass vertex data to pixel pass )
			{
				float4 pos : SV_POSITION;
				fixed4 vc : COLOR;
				fixed4 uv : TEXCOORD0;
			};
			v2f vert (customData v)
			{
				v2f o;
				o.pos = 	mul (UNITY_MATRIX_MVP, v.vertex);
				o.vc = v.color;
				o.uv = fixed4(0,0,0,0);
				o.uv.xy =		TRANSFORM_TEX (v.texcoord, _MainTex); // this allows you to offset uvs and such
				o.uv.xy = 	mul(_MatrixDiffuse, fixed4(o.uv.xy,0,1)); // this allows you to rotate uvs and such with script help
				return o;
			}

			fixed4 frag (v2f i) : COLOR  // i = in gets info from the out of the v2f vert
			{
				fixed4 result = fixed4(1,1,1,0);				
				fixed4 T_Diffuse = tex2D(_MainTex, i.uv.xy);
				result = _Color;
				fixed4 DiffResult = _Color * T_Diffuse;
				result = lerp(result,fixed4(DiffResult.rgb,1),T_Diffuse.a*_Color.a);
				result = fixed4(result.rgb * i.vc.rgb, result.a);
				result = fixed4(result.rgb,result.a * i.vc.a);
				return result;
			}
			ENDCG
		}//-------------------------------Pass-------------------------------
	
		Offset -1.0,0
		Blend SrcAlpha OneMinusSrcAlpha // --- not needed when doing cutout
		Pass
			{
			Tags
			{
				"RenderType" = "Transparent"
				"Queue" = "Transparent"
				"LightMode" = "VertexLM"
			}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			//#pragma multi_compile_fwdbase			

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4x4 _MatrixDiffuse;

			// sampler2D unity_Lightmap; //Far lightmap.
			// float4 unity_LightmapST; //Lightmap atlasing data.
			// sampler2D unity_LightmapInd; //Near lightmap (indirect lighting only).
			fixed _UNOShaderLightmapOpacity;
			struct customData
			{
				float4 vertex : POSITION;
				half3 normal : NORMAL;
				fixed2 texcoord : TEXCOORD0;
				fixed4 texcoord1 : TEXCOORD1;
				fixed4 color : COLOR;
			};
			struct v2f // = vertex to fragment ( pass vertex data to pixel pass )
			{
				float4 pos : SV_POSITION;
				fixed4 vc : COLOR;
				fixed4 uv : TEXCOORD0;
				fixed2 uv2 : TEXCOORD1;
			};
			v2f vert (customData v)
			{
				v2f o;
				o.pos = 	mul (UNITY_MATRIX_MVP, v.vertex);
				o.vc = v.color;
				o.uv = fixed4(0,0,0,0);
				o.uv.xy =		TRANSFORM_TEX (v.texcoord, _MainTex); // this allows you to offset uvs and such
				o.uv.xy = 	mul(_MatrixDiffuse, fixed4(o.uv.xy,0,1)); // this allows you to rotate uvs and such with script help
				o.uv2 = 	v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw; //Unity matrix lightmap uvs
				return o;
			}

			fixed4 frag (v2f i) : COLOR  // i = in gets info from the out of the v2f vert
			{
				fixed4 result = fixed4(1,1,1,0);				
				fixed4 T_Diffuse = tex2D(_MainTex, i.uv.xy);
				fixed4 Lightmap = fixed4(DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv2)),1);
				result = _Color;
				fixed4 DiffResult = _Color * T_Diffuse;
				result = lerp(result,fixed4(DiffResult.rgb,1),T_Diffuse.a*_Color.a);
				result = fixed4(result.rgb * i.vc.rgb, result.a);
				result = lerp(result,result * Lightmap, _UNOShaderLightmapOpacity);
				result = fixed4(result.rgb,result.a * i.vc.a);
				return result;
			}
			ENDCG
		}//--- Pass ---
		
		Offset -1.0,0
		Blend SrcAlpha OneMinusSrcAlpha // --- not needed when doing cutout
		Pass
			{
			Tags
			{
				"RenderType" = "Transparent"
				"Queue" = "Transparent"
				"LightMode" = "VertexLMRGBM"
			}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			//#pragma multi_compile_fwdbase			

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4x4 _MatrixDiffuse;

			// sampler2D unity_Lightmap; //Far lightmap.
			// float4 unity_LightmapST; //Lightmap atlasing data.
			// sampler2D unity_LightmapInd; //Near lightmap (indirect lighting only).
			fixed _UNOShaderLightmapOpacity;
			struct customData
			{
				float4 vertex : POSITION;
				half3 normal : NORMAL;
				fixed2 texcoord : TEXCOORD0;
				fixed4 texcoord1 : TEXCOORD1;
				fixed4 color : COLOR;
			};
			struct v2f // = vertex to fragment ( pass vertex data to pixel pass )
			{
				float4 pos : SV_POSITION;
				fixed4 vc : COLOR;
				fixed4 uv : TEXCOORD0;
				fixed2 uv2 : TEXCOORD1;
			};
			v2f vert (customData v)
			{
				v2f o;
				o.pos = 	mul (UNITY_MATRIX_MVP, v.vertex);
				o.vc = v.color;
				o.uv = fixed4(0,0,0,0);
				o.uv.xy =		TRANSFORM_TEX (v.texcoord, _MainTex); // this allows you to offset uvs and such
				o.uv.xy = 	mul(_MatrixDiffuse, fixed4(o.uv.xy,0,1)); // this allows you to rotate uvs and such with script help
				o.uv2 = 	v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw; //Unity matrix lightmap uvs
				return o;
			}

			fixed4 frag (v2f i) : COLOR  // i = in gets info from the out of the v2f vert
			{
				fixed4 result = fixed4(1,1,1,0);				
				fixed4 T_Diffuse = tex2D(_MainTex, i.uv.xy);
				fixed4 Lightmap = fixed4(DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv2)),1);
				result = _Color;
				fixed4 DiffResult = _Color * T_Diffuse;
				result = lerp(result,fixed4(DiffResult.rgb,1),T_Diffuse.a*_Color.a);
				result = fixed4(result.rgb * i.vc.rgb, result.a);
				result = lerp(result,result * Lightmap, _UNOShaderLightmapOpacity);
				result = fixed4(result.rgb,result.a * i.vc.a);
				return result;
			}
			ENDCG
		}//--- Pass ---
		
		// =========================== Pass to render object as a shadow caster ================================
//		Pass 
//		{
//			Name "ShadowCaster"
//			Tags { "LightMode" = "ShadowCaster" }
//			
//			ZWrite On ZTest LEqual Cull Off
//
//			CGPROGRAM
//			#pragma vertex vert
//			#pragma fragment frag
//			#pragma multi_compile_shadowcaster
//			#include "UnityCG.cginc"
//
//			struct v2f { 
//				V2F_SHADOW_CASTER;
//			};
//
//			v2f vert( appdata_base v )
//			{
//				v2f o;
//				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
//				return o;
//			}
//
//			float4 frag( v2f i ) : SV_Target
//			{
//				SHADOW_CASTER_FRAGMENT(i)
//			}
//			ENDCG
//		}

		
		
	} //-------------------------------SubShader-------------------------------
	

}
