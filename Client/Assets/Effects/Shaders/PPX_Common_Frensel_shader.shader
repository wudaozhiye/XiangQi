//警告！！！该shader完全未经过性能优化，请勿引入到项目中
//主要目的是方便动画师或者特效师制作个人demo，请勿用于任何商业用途
//个人知乎账号ID:shuang-miao-80 后续可能会有更新
//https://zhuanlan.zhihu.com/p/421146056
//技术谈论Q群:453762906
//最后,玩的开心!!!
Shader "Effect/PPX_FX/PPX_Common_Frensel_shader "
{
	Properties
	{
		[Enum(UnityEngine.Rendering.CompareFunction)]_Ztest("Ztest", Float) = 4
		[Enum(Off,0,On,1)]_Zwrite("Zwrite", Float) = 1
		[Enum(UnityEngine.Rendering.CullMode)]_Cull("Cull", Float) = 2
		_add_or_blend("add_or_blend", Range( 0 , 1)) =1
		[NoScaleOffset]_base_tex("base_tex", 2D) = "white" {}
		_base_uv("base_uv", Vector) = (1,1,0,0)
		_base_speed("base_speed", Vector) = (0,0,0,0)
		_reduce_color("reduce_color", Range( 0 , 1)) = 0
		_Base_color("Base_color", Color) = (0,0,0,0)
		_Base_color_power("Base_color_power", Range( 0 , 3)) = 1
		_Base_alpha("Base_alpha", Range( 0 , 3)) = 1
		[Toggle(_USE_NORMAL_ON)] _use_normal("use_normal", Float) = 0
		[NoScaleOffset]_NormalMap_tex("NormalMap_tex", 2D) = "bump" {}
		_normal_uv("normal_uv", Vector) = (1,1,0,0)
		_normal_speed("normal_speed", Vector) = (0,0,0,0)
		[Toggle(_USE_FRENSEL_ON)] _use_frensel("use_frensel", Float) = 1
		_frensel_range("frensel_range", Range( 0 , 1)) = 0
		_frensel_hard("frensel_hard", Range( 0 , 1)) = 0.5
		[Toggle(_FRENSEL_FLIP_ON)] _Frensel_flip("Frensel_flip", Float) = 0
		[Toggle(_USE_DEPTH_ON)] _use_depth("use_depth", Float) = 0
		_Fade_distance("Fade_distance", Range( 0 , 11)) = 1
		_edge_hard("edge_hard", Range( 0 , 1)) = 0.6
		_edge_range("edge_range", Range( 0 , 1)) = 0.1
		[Toggle(_EDGE_KILL_OR_ADD_ON)] _edge_kill_or_add("edge_kill_or_add", Float) = 0
		[Toggle(_USE_DISSOLVE_ON)] _use_dissolve("use_dissolve", Float) = 0
		_dissolve_tex("dissolve_tex", 2D) = "white" {}
		_disslove_uv("disslove_uv", Vector) = (1,1,0,0)
		_disslove_speed("disslove_speed", Vector) = (0,0,0,0)
		_edge_hardness("edge_hardness", Range( 0 , 22)) = 16
		_dissolve("dissolve", Range( 0 , 1)) = 1
		[Toggle(_USE_CUSTOM1_X_DISSOLVE_ON)] _use_custom1_x_dissolve("use_custom1_x_dissolve", Float) = 0
		[Toggle(_USE_DISPLACE_ON)] _use_displace("use_displace", Float) = 0
		[NoScaleOffset]_displace_tex("displace_tex", 2D) = "white" {}
		_displace_uv("displace_uv", Vector) = (1,1,0,0)
		_displace_speed("displace_speed", Vector) = (0,0,0,0)
		_displace_power("displace_power", Range( -4 , 4)) = 0
		[NoScaleOffset]_displace_mask_tex("displace_mask_tex", 2D) = "white" {}
		_displace_mask_uv("displace_mask_uv", Vector) = (1,1,0,0)
		_displace_mask_speed("displace_mask_speed", Vector) = (0,0,0,0)
		[Toggle(_USE_CUSTOM1_Y_DISPLACE_ON)] _use_custom1_y_displace("use_custom1_y_displace", Float) = 0

	}
	
	SubShader
	{
		
		
		 Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend One OneMinusSrcAlpha
		AlphaToMask Off
		Cull [_Cull]
		ColorMask RGBA
		ZWrite [_Zwrite]
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
				//only defining to not throw compilation error over Unity 5.5
				#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_VERT_POSITION
			#pragma shader_feature_local _USE_DISPLACE_ON
			#pragma shader_feature_local _USE_CUSTOM1_Y_DISPLACE_ON
			#pragma shader_feature_local _USE_DEPTH_ON
			#pragma shader_feature_local _USE_FRENSEL_ON
			#pragma shader_feature_local _FRENSEL_FLIP_ON
			#pragma shader_feature_local _USE_NORMAL_ON
			#pragma shader_feature_local _EDGE_KILL_OR_ADD_ON
			#pragma shader_feature_local _USE_DISSOLVE_ON
			#pragma shader_feature_local _USE_CUSTOM1_X_DISSOLVE_ON


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				half3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				half4 ase_tangent : TANGENT;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
					float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform half _Cull;
			uniform half _Ztest;
			uniform half _Zwrite;
			uniform sampler2D _displace_tex;
			SamplerState sampler_displace_tex;
			uniform half4 _displace_speed;
			uniform half4 _displace_uv;
			uniform sampler2D _displace_mask_tex;
			SamplerState sampler_displace_mask_tex;
			uniform half4 _displace_mask_speed;
			uniform half4 _displace_mask_uv;
			uniform half _displace_power;
			uniform half4 _Base_color;
			uniform half _Base_color_power;
			uniform sampler2D _base_tex;
			uniform half4 _base_speed;
			uniform half4 _base_uv;
			uniform half _reduce_color;
			uniform half _frensel_range;
			uniform half _frensel_hard;
			uniform sampler2D _NormalMap_tex;
			uniform half4 _normal_speed;
			uniform half4 _normal_uv;
			uniform half _edge_range;
			uniform half _edge_hard;
			UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
			uniform float4 _CameraDepthTexture_TexelSize;
			uniform half _Fade_distance;
			uniform sampler2D _dissolve_tex;
			SamplerState sampler_dissolve_tex;
			uniform half4 _disslove_speed;
			uniform half4 _disslove_uv;
			uniform half _edge_hardness;
			uniform half _dissolve;
			uniform half _Base_alpha;
			uniform half _add_or_blend;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				half3 temp_cast_0 = (0.0).xxx;
				half4 break11_g126 = _displace_speed;
				half mulTime5_g126 = _Time.y * break11_g126.z;
				half2 appendResult6_g126 = (half2(break11_g126.x , break11_g126.y));
				half4 break10_g126 = _displace_uv;
				half2 appendResult2_g126 = (half2(break10_g126.x , break10_g126.y));
				half2 appendResult3_g126 = (half2(break10_g126.z , break10_g126.w));
				half2 texCoord4_g126 = v.ase_texcoord * appendResult2_g126 + appendResult3_g126;
				half2 panner1_g126 = ( mulTime5_g126 * appendResult6_g126 + texCoord4_g126);
				half4 break11_g125 = _displace_mask_speed;
				half mulTime5_g125 = _Time.y * break11_g125.z;
				half2 appendResult6_g125 = (half2(break11_g125.x , break11_g125.y));
				half4 break10_g125 = _displace_mask_uv;
				half2 appendResult2_g125 = (half2(break10_g125.x , break10_g125.y));
				half2 appendResult3_g125 = (half2(break10_g125.z , break10_g125.w));
				half2 texCoord4_g125 = v.ase_texcoord.xy * appendResult2_g125 + appendResult3_g125;
				half2 panner1_g125 = ( mulTime5_g125 * appendResult6_g125 + texCoord4_g125);
				half4 texCoord514 = v.ase_texcoord;
				texCoord514.xy = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _USE_CUSTOM1_Y_DISPLACE_ON
					half staticSwitch515 = texCoord514.w;
				#else
					half staticSwitch515 = _displace_power;
				#endif
				half3 displace516 = ( ( v.ase_normal * ( tex2Dlod( _displace_tex, float4( panner1_g126, 0, 0.0) ).r * tex2Dlod( _displace_mask_tex, float4( panner1_g125, 0, 0.0) ).r ) ) * staticSwitch515 );
				#ifdef _USE_DISPLACE_ON
					half3 staticSwitch518 = displace516;
				#else
					half3 staticSwitch518 = temp_cast_0;
				#endif
				
				half3 ase_worldNormal = UnityObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord2.xyz = ase_worldNormal;
				half3 ase_worldTangent = UnityObjectToWorldDir(v.ase_tangent);
				o.ase_texcoord3.xyz = ase_worldTangent;
				half ase_vertexTangentSign = v.ase_tangent.w * unity_WorldTransformParams.w;
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				o.ase_texcoord4.xyz = ase_worldBitangent;
				half3 vertexPos500 = v.vertex.xyz;
				float4 ase_clipPos500 = UnityObjectToClipPos(vertexPos500);
				float4 screenPos500 = ComputeScreenPos(ase_clipPos500);
				o.ase_texcoord5 = screenPos500;
				
				o.ase_texcoord1 = v.ase_texcoord;
				o.ase_color = v.color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.w = 0;
				o.ase_texcoord3.w = 0;
				o.ase_texcoord4.w = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
					vertexValue = v.vertex.xyz;
				#endif
				vertexValue = staticSwitch518;
				#if ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
					o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
					float3 WorldPosition = i.worldPos;
				#endif
				half4 break11_g121 = _base_speed;
				half mulTime5_g121 = _Time.y * break11_g121.z;
				half2 appendResult6_g121 = (half2(break11_g121.x , break11_g121.y));
				half4 break10_g121 = _base_uv;
				half2 appendResult2_g121 = (half2(break10_g121.x , break10_g121.y));
				half2 appendResult3_g121 = (half2(break10_g121.z , break10_g121.w));
				half2 texCoord4_g121 = i.ase_texcoord1.xy * appendResult2_g121 + appendResult3_g121;
				half2 panner1_g121 = ( mulTime5_g121 * appendResult6_g121 + texCoord4_g121);
				half4 tex2DNode480 = tex2D( _base_tex, panner1_g121 );
				half3 desaturateInitialColor547 = tex2DNode480.rgb;
				half desaturateDot547 = dot( desaturateInitialColor547, float3( 0.299, 0.587, 0.114 ));
				half3 desaturateVar547 = lerp( desaturateInitialColor547, desaturateDot547.xxx, _reduce_color );
				half3 ase_worldNormal = i.ase_texcoord2.xyz;
				half4 break11_g117 = _normal_speed;
				half mulTime5_g117 = _Time.y * break11_g117.z;
				half2 appendResult6_g117 = (half2(break11_g117.x , break11_g117.y));
				half4 break10_g117 = _normal_uv;
				half2 appendResult2_g117 = (half2(break10_g117.x , break10_g117.y));
				half2 appendResult3_g117 = (half2(break10_g117.z , break10_g117.w));
				half2 texCoord4_g117 = i.ase_texcoord1.xy * appendResult2_g117 + appendResult3_g117;
				half2 panner1_g117 = ( mulTime5_g117 * appendResult6_g117 + texCoord4_g117);
				half3 ase_worldTangent = i.ase_texcoord3.xyz;
				float3 ase_worldBitangent = i.ase_texcoord4.xyz;
				half3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				half3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				half3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 tanNormal612 = UnpackNormal( tex2D( _NormalMap_tex, panner1_g117 ) );
				half3 worldNormal612 = float3(dot(tanToWorld0,tanNormal612), dot(tanToWorld1,tanNormal612), dot(tanToWorld2,tanNormal612));
				#ifdef _USE_NORMAL_ON
					half3 staticSwitch636 = worldNormal612;
				#else
					half3 staticSwitch636 = ase_worldNormal;
				#endif
				float3 ase_worldViewDir = UnityWorldSpaceViewDir(WorldPosition);
				ase_worldViewDir = normalize(ase_worldViewDir);
				half dotResult560 = dot( staticSwitch636 , ase_worldViewDir );
				half smoothstepResult564 = smoothstep( _frensel_range , ( _frensel_range + _frensel_hard ) , ( 1.0 - dotResult560 ));
				half temp_output_565_0 = saturate( smoothstepResult564 );
				#ifdef _FRENSEL_FLIP_ON
					half staticSwitch581 = ( 1.0 - temp_output_565_0 );
				#else
					half staticSwitch581 = temp_output_565_0;
				#endif
				#ifdef _USE_FRENSEL_ON
					half staticSwitch628 = staticSwitch581;
				#else
					half staticSwitch628 = 1.0;
				#endif
				half frensel566 = staticSwitch628;
				half temp_output_501_0 = ( ( (tex2DNode480).a * frensel566 ) * frensel566 );
				float4 screenPos500 = i.ase_texcoord5;
				half4 ase_screenPosNorm500 = screenPos500 / screenPos500.w;
				ase_screenPosNorm500.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm500.z : ase_screenPosNorm500.z * 0.5 + 0.5;
				float screenDepth500 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm500.xy ));
				half distanceDepth500 = saturate( abs( ( screenDepth500 - LinearEyeDepth( ase_screenPosNorm500.z ) ) / ( _Fade_distance ) ) );
				half smoothstepResult624 = smoothstep( _edge_range , ( _edge_range + _edge_hard ) , distanceDepth500);
				half depth605 = smoothstepResult624;
				#ifdef _EDGE_KILL_OR_ADD_ON
					half staticSwitch614 = ( temp_output_501_0 * depth605 );
				#else
					half staticSwitch614 = ( saturate( ( 1.0 - depth605 ) ) + temp_output_501_0 );
				#endif
				#ifdef _USE_DEPTH_ON
					half staticSwitch630 = staticSwitch614;
				#else
					half staticSwitch630 = temp_output_501_0;
				#endif
				half4 break11_g122 = _disslove_speed;
				half mulTime5_g122 = _Time.y * break11_g122.z;
				half2 appendResult6_g122 = (half2(break11_g122.x , break11_g122.y));
				half4 break10_g122 = _disslove_uv;
				half2 appendResult2_g122 = (half2(break10_g122.x , break10_g122.y));
				half2 appendResult3_g122 = (half2(break10_g122.z , break10_g122.w));
				half2 texCoord4_g122 = i.ase_texcoord1.xy * appendResult2_g122 + appendResult3_g122;
				half2 panner1_g122 = ( mulTime5_g122 * appendResult6_g122 + texCoord4_g122);
				half temp_output_7_0_g123 = _edge_hardness;
				half3 texCoord608 = i.ase_texcoord1.xyz;
				texCoord608.xy = i.ase_texcoord1.xyz.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _USE_CUSTOM1_X_DISSOLVE_ON
					half staticSwitch610 = texCoord608.z;
				#else
					half staticSwitch610 = _dissolve;
				#endif
				half lerpResult2_g123 = lerp( temp_output_7_0_g123 , -1.0 , staticSwitch610);
				#ifdef _USE_DISSOLVE_ON
					half staticSwitch632 = saturate( saturate( ( ( tex2D( _dissolve_tex, panner1_g122 ).r * temp_output_7_0_g123 ) - lerpResult2_g123 ) ) );
				#else
					half staticSwitch632 = 1.0;
				#endif
				half dissolve_alpha600 = staticSwitch632;
				half temp_output_9_0_g127 = ( i.ase_color.a * saturate( ( ( staticSwitch630 * dissolve_alpha600 ) * _Base_alpha ) ) );
				half4 appendResult2_g127 = (half4(( ( (( _Base_color * _Base_color_power * half4( desaturateVar547 , 0.0 ) )).rgb * (i.ase_color).rgb ) * temp_output_9_0_g127 ) , temp_output_9_0_g127));
				half4 appendResult6_g127 = (half4(appendResult2_g127.xyz , ( temp_output_9_0_g127 * _add_or_blend )));
				
				
				finalColor = appendResult6_g127;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "PPX_Frensel_shader_GUI"
	
	
}
