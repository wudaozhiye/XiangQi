//警告！！！该shader完全未经过性能优化，请勿引入到项目中
//主要目的是方便动画师或者特效师制作个人demo，请勿用于任何商业用途
//个人知乎账号ID:shuang-miao-80 后续可能会有更新
//https://zhuanlan.zhihu.com/p/421146056
//技术谈论Q群:755239075
//最后,玩的开心!!!

Shader "PPX_shader/char/Toon_shader_code_ver1.0"
{
	Properties
	{
		_ASEOutlineWidth( "Outline Width", Range(0,1) ) = 0.002
		_ASEOutlineColor( "Outline Color", Color ) = (0.2358491,0.06021679,0,1)
		_ASEOutalpha( "_ASEOutalpha", Range(-1,0) ) = 0
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_shadow_clip("shadow_clip", Range( 0 , 1)) = 0.64
		_shadow_edge("shadow_edge", Range( 0 , 1)) = 0
		_normal_map("normal_map", 2D) = "bump" {}
		_BaseTex("BaseTex", 2D) = "white" {}
		_light("light", Color) = (1,1,1,0)
		_drak("drak", Color) = (0,0,0,0)
		[Toggle(_USE_SECOND_TEX_ON)] _use_second_tex("use_second_tex", Float) = 0
		_BuffTex("BuffTex", 2D) = "white" {}
		_BuffTex_switch("BuffTex_switch", 2D) = "white" {}
		_BuffTex_switch_edge_hardness("BuffTex_switch_edge_hardness", Range( 0 , 22)) = 1
		_BuffTex_switch_dissolve("BuffTex_switch_dissolve", Range( 0 , 1)) = 1
		[Toggle(_USE_MATCAT_ON)] _use_matcat("use_matcat", Float) = 0
		_matcap("matcap", 2D) = "white" {}
		_special_buff_switch("special_buff_switch", 2D) = "white" {}
		_special_buff_switch_edge_hardness("special_buff_switch_edge_hardness", Range( 0 , 22)) = 1
		_special_buff_dissolve("special_buff_dissolve", Range( 0 , 1)) = 1
		[Toggle(_USE_FRENSEL_ON)] _use_frensel("use_frensel", Float) = 0
		_frensel_range("frensel_range", Range( -1 , 1)) = 0
		_frensel_hard("frensel_hard", Range( 0 , 1)) = 0
		_frensel_power("frensel_power", Range( 0 , 3)) = 0
		[HDR]_frensel_color("frensel_color", Color) = (0,0,0,0)
		[Toggle(_USE_EMISSIVE_ON)] _use_emissive("use_emissive", Float) = 0
		_Emissive_Tex("Emissive_Tex", 2D) = "white" {}
		_Emissve_color("Emissve_color", Color) = (0,0,0,0)
		_Emissve_power("Emissve_power", Range( -1 , 3)) = 0
		[Toggle(_USE_DISSOLVE_ON)] _use_dissolve("use_dissolve", Float) = 0
		_dissolve("dissolve", 2D) = "white" {}
		_dissolve_edge_dissolve("dissolve_edge_dissolve", Range( 0 , 1)) = 1
		_edge_width("edge_width", Range( 0 , 1)) = 0
		_edge_clip("edge_clip", Range( 0 , 1)) = 0
		_dissolve_Emissve_color("dissolve_Emissve_color", Color) = (0,0,0,0)
		_dissolve_Emissve_power("dissolve_Emissve_power", Range( 0 , 3)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		
		float4 _ASEOutlineColor;
		float _ASEOutlineWidth;
		float _ASEOutalpha;
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += ( v.normal * _ASEOutlineWidth );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = _ASEOutlineColor.rgb;
			o.Alpha = 1;
			clip(_ASEOutalpha);
		}
		ENDCG
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _USE_DISSOLVE_ON
		#pragma shader_feature_local _USE_EMISSIVE_ON
		#pragma shader_feature_local _USE_FRENSEL_ON
		#pragma shader_feature_local _USE_MATCAT_ON
		#pragma shader_feature_local _USE_SECOND_TEX_ON
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform sampler2D _dissolve;
		uniform float4 _dissolve_ST;
		uniform float _dissolve_edge_dissolve;
		uniform sampler2D _BaseTex;
		uniform float4 _BaseTex_ST;
		uniform sampler2D _BuffTex;
		uniform float4 _BuffTex_ST;
		uniform sampler2D _BuffTex_switch;
		uniform float4 _BuffTex_switch_ST;
		uniform float _BuffTex_switch_edge_hardness;
		uniform float _BuffTex_switch_dissolve;
		uniform float4 _drak;
		uniform float4 _light;
		uniform float _shadow_clip;
		uniform float _shadow_edge;
		uniform sampler2D _normal_map;
		uniform float4 _normal_map_ST;
		uniform sampler2D _matcap;
		uniform sampler2D _special_buff_switch;
		uniform float4 _special_buff_switch_ST;
		uniform float _special_buff_switch_edge_hardness;
		uniform float _special_buff_dissolve;
		uniform float4 _frensel_color;
		uniform float _frensel_power;
		uniform float _frensel_range;
		uniform float _frensel_hard;
		uniform sampler2D _Emissive_Tex;
		uniform float4 _Emissive_Tex_ST;
		uniform float4 _Emissve_color;
		uniform float _Emissve_power;
		uniform float4 _dissolve_Emissve_color;
		uniform float _dissolve_Emissve_power;
		uniform float _edge_clip;
		uniform float _edge_width;
		uniform float _Cutoff = 0.5;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
				float ase_lightAtten = data.atten;
				if( _LightColor0.a == 0)
				ase_lightAtten = 0;
			#else
				float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
				float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
				half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
				float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
				float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
				ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float2 uv_dissolve = i.uv_texcoord * _dissolve_ST.xy + _dissolve_ST.zw;
			float temp_output_7_0_g47 = 2.0;
			float lerpResult2_g47 = lerp( temp_output_7_0_g47 , -1.0 , _dissolve_edge_dissolve);
			float temp_output_147_0 = saturate( ( ( tex2D( _dissolve, uv_dissolve ).r * temp_output_7_0_g47 ) - lerpResult2_g47 ) );
			float alpha175 = temp_output_147_0;
			float2 uv_BaseTex = i.uv_texcoord * _BaseTex_ST.xy + _BaseTex_ST.zw;
			float4 tex2DNode13 = tex2D( _BaseTex, uv_BaseTex );
			float2 uv_BuffTex = i.uv_texcoord * _BuffTex_ST.xy + _BuffTex_ST.zw;
			float2 uv_BuffTex_switch = i.uv_texcoord * _BuffTex_switch_ST.xy + _BuffTex_switch_ST.zw;
			float temp_output_7_0_g45 = _BuffTex_switch_edge_hardness;
			float lerpResult2_g45 = lerp( temp_output_7_0_g45 , -1.0 , _BuffTex_switch_dissolve);
			float4 lerpResult96 = lerp( tex2DNode13 , tex2D( _BuffTex, uv_BuffTex ) , saturate( ( ( tex2D( _BuffTex_switch, uv_BuffTex_switch ).r * temp_output_7_0_g45 ) - lerpResult2_g45 ) ));
			#ifdef _USE_SECOND_TEX_ON
				float4 staticSwitch185 = lerpResult96;
			#else
				float4 staticSwitch185 = tex2DNode13;
			#endif
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
				float3 ase_worldlightDir = 0;
			#else //aseld
				float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float2 uv_normal_map = i.uv_texcoord * _normal_map_ST.xy + _normal_map_ST.zw;
			float3 tex2DNode32 = UnpackNormal( tex2D( _normal_map, uv_normal_map ) );
			float3 newWorldNormal4 = (WorldNormalVector( i , tex2DNode32 ));
			float dotResult2 = dot( ase_worldlightDir , newWorldNormal4 );
			float smoothstepResult9 = smoothstep( _shadow_clip , ( _shadow_clip + _shadow_edge ) , ( ( ( dotResult2 * ase_lightAtten ) + 1.0 ) * 0.5 ));
			float4 lerpResult19 = lerp( _drak , _light , smoothstepResult9);
			float4 base_part169 = ( staticSwitch185 * lerpResult19 );
			float3 normalizeResult63 = normalize( ( ase_worldPos - _WorldSpaceCameraPos ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float4 appendResult55 = (float4(ase_worldTangent.x , ase_worldBitangent.x , ase_worldNormal.x , ase_worldPos.x));
			float3 normal_map67 = tex2DNode32;
			float dotResult66 = dot( (appendResult55).xyz , normal_map67 );
			float4 appendResult60 = (float4(ase_worldTangent.y , ase_worldBitangent.y , ase_worldNormal.y , ase_worldPos.y));
			float dotResult71 = dot( (appendResult60).xyz , normal_map67 );
			float4 appendResult61 = (float4(ase_worldTangent.z , ase_worldBitangent.z , ase_worldNormal.z , ase_worldPos.z));
			float dotResult74 = dot( (appendResult61).xyz , normal_map67 );
			float3 appendResult76 = (float3(dotResult66 , dotResult71 , dotResult74));
			float3 normalizeResult78 = normalize( appendResult76 );
			float3 temp_output_81_0 = mul( UNITY_MATRIX_V, float4( reflect( normalizeResult63 , normalizeResult78 ) , 0.0 ) ).xyz;
			float4 matcap94 = tex2D( _matcap, ( ( (temp_output_81_0).xy / ( 2.828427 * sqrt( ( (temp_output_81_0).z + 1.0 ) ) ) ) + 0.5 ) );
			float2 uv_special_buff_switch = i.uv_texcoord * _special_buff_switch_ST.xy + _special_buff_switch_ST.zw;
			float temp_output_7_0_g48 = _special_buff_switch_edge_hardness;
			float lerpResult2_g48 = lerp( temp_output_7_0_g48 , -1.0 , _special_buff_dissolve);
			float4 lerpResult92 = lerp( base_part169 , matcap94 , saturate( ( ( tex2D( _special_buff_switch, uv_special_buff_switch ).r * temp_output_7_0_g48 ) - lerpResult2_g48 ) ));
			#ifdef _USE_MATCAT_ON
				float4 staticSwitch181 = lerpResult92;
			#else
				float4 staticSwitch181 = base_part169;
			#endif
			float3 normal33 = newWorldNormal4;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float dotResult109 = dot( normal33 , ase_worldViewDir );
			float smoothstepResult124 = smoothstep( _frensel_range , ( _frensel_range + _frensel_hard ) , ( 1.0 - dotResult109 ));
			float4 frensel117 = ( ( _frensel_color * _frensel_power ) * saturate( smoothstepResult124 ) );
			#ifdef _USE_FRENSEL_ON
				float4 staticSwitch182 = ( lerpResult92 + frensel117 );
			#else
				float4 staticSwitch182 = staticSwitch181;
			#endif
			float2 uv_Emissive_Tex = i.uv_texcoord * _Emissive_Tex_ST.xy + _Emissive_Tex_ST.zw;
			float4 emissive141 = ( tex2D( _Emissive_Tex, uv_Emissive_Tex ).r * ( _Emissve_color * _Emissve_power ) );
			float4 temp_output_142_0 = ( staticSwitch182 + emissive141 );
			#ifdef _USE_EMISSIVE_ON
				float4 staticSwitch184 = temp_output_142_0;
			#else
				float4 staticSwitch184 = staticSwitch182;
			#endif
			float smoothstepResult152 = smoothstep( _edge_clip , ( _edge_clip + _edge_width ) , ( 1.0 - temp_output_147_0 ));
			float4 dissolve_edge165 = ( ( _dissolve_Emissve_color * _dissolve_Emissve_power ) * smoothstepResult152 );
			#ifdef _USE_DISSOLVE_ON
				float4 staticSwitch183 = ( temp_output_142_0 + dissolve_edge165 );
			#else
				float4 staticSwitch183 = staticSwitch184;
			#endif
			c.rgb = (staticSwitch183).rgb;
			c.a = 1;
			clip( alpha175 - _Cutoff );
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows exclude_path:deferred 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
				, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
				#if defined( CAN_SKIP_VPOS )
					float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "PPX_TOON_char_shader_GUI"
}
