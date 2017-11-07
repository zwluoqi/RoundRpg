// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

//Version=1.41
Shader"UNOShader/_Library/UNLIT/UNOShaderUNLIT Color Diffuse VertexCol SH "
{
	Properties
	{
		_BRDF("BRDF Shading", 2D) = "white" {}
		_BRDFBrightness ("BRDF Brightness", Range (0, 0)) = 0
		_Color ("Color (A)Opacity", Color) = (1,1,1,1)
		_MainTex ("Diffuse Texture", 2D) = "white" {}
		_MainTexOpacity ("Diffuse Opacity", Range (0, 1)) = 1
		_LightmapTex ("Lightmap Texture", 2D) = "gray" {}
		_MasksTex ("Masks", 2D) = "white" {}
	}
	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
			"Queue" = "Geometry"
		}
		Pass
			{
			Name "ForwardBase"
			Tags
			{
				"RenderType" = "Opaque"
				"Queue" = "Geometry"
				"LightMode" = "ForwardBase"
			}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "UnityPBSLighting.cginc"
			#include "UnityStandardBRDF.cginc"
			#pragma multi_compile_fwdbase
			#pragma shader_feature lmUV1_ON lmUV1_OFF
			#pragma shader_feature maskTex_ON maskTex_OFF
			#pragma shader_feature mathPixel_ON mathPixel_OFF
			#pragma shader_feature BASE_UNLIT BASE_BRDF
			#pragma shader_feature AMBIENT_OFF AMBIENT_ON
			#pragma shader_feature LDIR_OFF LDIR_ON
			#pragma shader_feature CUSTOMLIGHTMAP_OFF CUSTOMLIGHTMAP_ON
			#pragma shader_feature CUSTOMSHADOW_OFF CUSTOMSHADOW_ON
			#pragma exclude_renderers d3d11_9x
			#pragma multi_compile_fog
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "AutoLight.cginc"
			#include "Lighting.cginc"

			#if maskTex_ON
			sampler2D _MasksTex;
			half4 _MasksTex_ST;
			#endif
			sampler2D _BRDF;
			half4 _BRDF_ST;
			fixed _BRDFBrightness;
			fixed4 _Color;

			sampler2D _MainTex;
			half4 _MainTex_ST;
			fixed _MainTexOpacity;
			half4x4 _MainTexMatrix;

			#ifdef CUSTOMLIGHTMAP_ON
				sampler2D _LightmapTex;
				half4 _LightmapTex_ST;
			#endif

			fixed4 _UNOShaderShadowColor;
			struct customData
			{
				half4 vertex : POSITION;
				half3 normal : NORMAL;
				half4 tangent : TANGENT;
				fixed2 texcoord : TEXCOORD0;
				fixed2 texcoord1 : TEXCOORD1;
				fixed4 color : COLOR;
			};
			struct v2f // = vertex to fragment ( pass vertex data to pixel pass )
			{
				half4 pos : SV_POSITION;
				fixed4 vc : COLOR;
				half4 Ndot : COLOR1;
				fixed4 uv : TEXCOORD0;
				fixed4 uv2 : TEXCOORD1;
				half4 posWorld : TEXCOORD2;//position of vertex in world;
				half4 normalDir : TEXCOORD3;//vertex Normal Direction in world space
				half4 viewRefDir : TEXCOORD4;
				UNITY_FOG_COORDS(5)
				LIGHTING_COORDS(6, 7)
			};
			v2f vert (customData v)
			{
				v2f o;
				o.normalDir = fixed4 (0,0,0,0);
				o.Ndot = fixed4(0,0,0,0);
				o.posWorld = fixed4 (0,0,0,0);
				o.normalDir.xyz = UnityObjectToWorldNormal(v.normal);

			//--- Vectors
				half3 normalDirection = normalize(half3( mul(half4(v.normal, 0.0), unity_WorldToObject).xyz ));
				half3 lightDirection = normalize(half3(_WorldSpaceLightPos0.xyz));
				half3 viewDirection = normalize(ObjSpaceViewDir(v.vertex));
				o.posWorld.xyz = mul(unity_ObjectToWorld, v.vertex);
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);//UNITY_MATRIX_MVP is a matrix that will convert a model's vertex position to the projection space
				o.vc = fixed4(1,1,1,1);;// Vertex Colors
				o.vc = v.color;// Vertex Colors
				o.uv = fixed4(0,0,0,0);
				o.uv.xy = TRANSFORM_TEX (v.texcoord, _MainTex); // this allows you to offset uvs and such	
				o.uv.xy = mul(_MainTexMatrix, fixed4(o.uv.xy,0,1)); // this allows you to rotate uvs and such with script help
				o.uv2 = fixed4(0,0,0,0);
				o.uv2.xy = v.texcoord1; //--- regular uv2
				o.uv2.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw; //Unity matrix lightmap uvs
				o.viewRefDir = fixed4(0,0,0,0);
				#ifdef BASE_UNLIT
					o.Ndot.x = max(0.0, dot(normalDirection, lightDirection));//NdotL  light falloff
				#endif
				#ifdef BASE_BRDF
					o.Ndot.x = dot(normalDirection, lightDirection)*.5 +.5;//NdotL  light falloff
				#endif
				o.Ndot.y = clamp((dot(viewDirection, v.normal)) * 1.2 -.2 ,0.01,.99);

			//============================= Lights ================================
				fixed3 vLights = fixed3 (0,0,0);

				//___________________________ LightProbes Shade SH9 Math  __________________________________________
				fixed3 ambience = fixed3(1,1,1);
						ambience = ShadeSH9 (half4(o.normalDir.xyz,1.0)).rgb;
				vLights += ambience;

				o.normalDir.w = vLights.r;
				o.viewRefDir.w = vLights.g;
				o.posWorld.w = vLights.b;
				TRANSFER_VERTEX_TO_FRAGMENT(o) // This sets up the vertex attributes required for lighting and passes them through to the fragment shader.
			//_________________________________________ FOG  __________________________________________
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			fixed4 frag (v2f i) : COLOR  // i = in gets info from the out of the v2f vert
			{
				fixed4 resultRGB = fixed4(1,1,1,0);
				fixed3 vLights = fixed3(i.normalDir.w,i.viewRefDir.w,i.posWorld.w);
			//__________________________________ Vectors _____________________________________
				half3 normalDirection = normalize(i.normalDir.xyz);
				half3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);//  half3 _WorldSpaceCameraPos.xyz built in gets camera Position
				#if maskTex_ON
			//__________________________________ Masks _____________________________________
				fixed4 T_Masks = tex2D(_MasksTex, i.uv.xy);
				#endif
			//__________________________________ Color Base _____________________________________
				resultRGB = _Color;
			//__________________________________ Diffuse _____________________________________
				fixed4 T_Diffuse = tex2D(_MainTex, i.uv.xy);
				resultRGB *= T_Diffuse;
				resultRGB = lerp(_Color,fixed4(resultRGB.rgb,1),(T_Diffuse.a * _MainTexOpacity));

			//__________________________________ Vertex Color _____________________________________
				resultRGB.rgb = resultRGB.rgb * i.vc.rgb;

			//__________________________________ Lightmap _____________________________________
			//--- lightmap unity ---
				#ifdef CUSTOMLIGHTMAP_OFF
					#ifdef LIGHTMAP_ON
						fixed4 Lightmap = fixed4(DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv2)),1);
						resultRGB.rgb *= resultRGB*Lightmap.rgb;
					#endif
				#endif

			//--- custom lightmap ---
				#ifdef CUSTOMLIGHTMAP_ON
					#if lmUV1_ON
						fixed4 Lightmap = tex2D(_LightmapTex, i.uv);
					#endif
					#if lmUV1_OFF
						fixed4 Lightmap = tex2D(_LightmapTex, i.uv2);
					#endif
					Lightmap.rgb = DecodeLightmap(Lightmap);
					resultRGB.rgb *= Lightmap.rgb;
				#endif

			//__________________________________ Lighting _____________________________________
				fixed atten = LIGHT_ATTENUATION(i); // This gets the shadow and attenuation values combined.
				fixed NdotL = i.Ndot.x;
				fixed NdotV = i.Ndot.y;
				fixed3 resultRGBflat = resultRGB.rgb;
				fixed4 T_BRDF = tex2D(_BRDF, fixed2(NdotV,NdotL));
				fixed3 Lights = vLights;
				vLights = lerp(vLights, 1, _BRDFBrightness);

				#ifdef BASE_UNLIT
					#ifdef LDIR_ON
						NdotL *= atten;
					#else
						NdotL = atten;
					#endif
				#endif

				#ifdef BASE_BRDF
					NdotL = T_BRDF.r;
					NdotL = clamp( (NdotL * atten) + T_BRDF.a ,0,1);
				#endif
				#ifdef LDIR_ON
					#ifdef AMBIENT_ON
						Lights = vLights + _LightColor0.rgb;
					#else
						Lights = 1 +  _LightColor0.rgb;
					#endif
				#endif
				#ifdef LDIR_OFF
					#ifdef AMBIENT_ON
						Lights = vLights;
					#else
						Lights = fixed3(1,1,1);
					#endif
				#endif

				#ifdef CUSTOMSHADOW_ON
					#ifdef LDIR_ON
						resultRGB.rgb = lerp(resultRGB.rgb * _UNOShaderShadowColor.rgb,resultRGB.rgb * Lights, NdotL);
					#endif
					#ifdef LDIR_OFF
						#ifdef AMBIENT_ON
							resultRGB.rgb = lerp(resultRGB.rgb * _UNOShaderShadowColor.rgb ,resultRGB.rgb * vLights, NdotL);
						#else
							resultRGB.rgb = lerp(resultRGB.rgb * _UNOShaderShadowColor.rgb ,resultRGB.rgb, NdotL);
						#endif
					#endif
				#endif

				#ifdef CUSTOMSHADOW_OFF
					#ifdef LDIR_ON
						resultRGB.rgb = lerp(resultRGB.rgb * vLights ,resultRGB.rgb * Lights, NdotL);
					#endif
					#ifdef LDIR_OFF
						#ifdef AMBIENT_ON
							resultRGB.rgb *= vLights;
						#else
							resultRGB.rgb = lerp(resultRGB.rgb * vLights ,resultRGB.rgb, NdotL);
						#endif
					#endif
				#endif

			//__________________________________ Mask Occlussion _____________________________________
				#if maskTex_ON
				//--- Oclussion from alpha
				resultRGB.rgb = resultRGB.rgb * T_Masks.g;
				#endif

			//__________________________________ Fog  _____________________________________
				UNITY_APPLY_FOG(i.fogCoord, resultRGB);

			//__________________________________ Vertex Alpha _____________________________________
				resultRGB.a *= i.vc.a;

			//__________________________________ result Final  _____________________________________
				return resultRGB;
			}
			ENDCG
		}//-------------------------------Pass-------------------------------
		UsePass "UNOShader/_Library/Helpers/Shadows/SHADOWCAST"
	} //-------------------------------SubShader-------------------------------
	Fallback "UNOShader/_Library/Helpers/VertexUNLIT"
	CustomEditor "UNOShader_UNLIT"
}