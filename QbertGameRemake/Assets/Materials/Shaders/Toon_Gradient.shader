Shader "Toon/Gradient"
{
	Properties
	{
		_Color("Color", Color) = (0.5, 0.65, 1, 1)
		_MainTex("Main Texture", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_EmissionMap("Emission Map", 2D) = "black" {}
		[HDR]
		_AmbientColor("Ambient Color", Color) = (0.4,0.4,0.4,1)
		_SpecularColor("Specular Color", Color) = (0.9,0.9,0.9,1)
		_Glossiness("Glossiness", Float) = 32
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimAmount("Rim Amount", Range(0, 1)) = 0.716
		_RimThreshold("Rim Threshold", Range(0, 1)) = 0.1
	}
		SubShader
		{
			Pass
			{
				Tags
				{
					"LightMode" = "ForwardBase"
					"PassFlags" = "OnlyDirectional"
				}
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fwdbase
				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float4 uv : TEXCOORD0;
					float3 normal : NORMAL;
					float4 tangent : TANGENT;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float3 worldNormal : NORMAL;
					float3 viewDir : TEXCOORD1;

					float3 worldPos : TEXCOORD2;
					half3 tspace0 : TEXCOORD3;
					half3 tspace1 : TEXCOORD4;
					half3 tspace2 : TEXCOORD5;

					SHADOW_COORDS(6)
				};

				sampler2D _MainTex;
				sampler2D _EmissionMap;
				sampler2D _BumpMap;
				float4 _MainTex_ST;
				float4 _EmissionMap_ST;

				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.worldNormal = UnityObjectToWorldNormal(v.normal);
					o.viewDir = WorldSpaceViewDir(v.vertex);

					o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					half3 worldNormalTemp = UnityObjectToWorldNormal(v.normal);
					half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
					half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
					half3 wBitangent = cross(worldNormalTemp, wTangent) * tangentSign;
					o.tspace0 = half3(wTangent.x, wBitangent.x, worldNormalTemp.x);
					o.tspace1 = half3(wTangent.y, wBitangent.y, worldNormalTemp.y);
					o.tspace2 = half3(wTangent.z, wBitangent.z, worldNormalTemp.z);

					TRANSFER_SHADOW(o)
					return o;
				}

				float4 _Color;
				float4 _AmbientColor;
				float _Glossiness;
				float4 _SpecularColor;
				float4 _RimColor;
				float _RimAmount;
				float _RimThreshold;

				float4 frag(v2f i) : SV_Target
				{
					half3 tnormal = UnpackNormal(tex2D(_BumpMap, i.uv));
					i.worldNormal.x = dot(i.tspace0, tnormal);
					i.worldNormal.y = dot(i.tspace1, tnormal);
					i.worldNormal.z = dot(i.tspace2, tnormal);

					float4 sample_albedo = tex2D(_MainTex, i.uv);
					float4 sample_emission = tex2D(_EmissionMap, i.uv);

					float3 normal = normalize(i.worldNormal);
					float NdotL = dot(_WorldSpaceLightPos0, normal);
					float shadow = SHADOW_ATTENUATION(i);
					float lightIntensity = smoothstep(0, 0.01, NdotL * shadow);
					float4 light = lightIntensity * _LightColor0;
					float3 viewDir = normalize(i.viewDir);
					float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
					float NdotH = dot(normal, halfVector);
					float specularIntensity = pow(NdotH * lightIntensity, _Glossiness * _Glossiness);
					float specularIntensitySmooth = smoothstep(0.005, 0.01, specularIntensity);
					float4 specular = specularIntensitySmooth * _SpecularColor;
					float4 rimDot = 1 - dot(viewDir, normal);
					float rimIntensity = rimDot * pow(NdotL, _RimThreshold);
					rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);
					float4 rim = rimIntensity * _RimColor;

					return max(_Color * sample_albedo * (_AmbientColor + light + specular + rim), sample_emission);
				}
				ENDCG
			}
			UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
		}
}