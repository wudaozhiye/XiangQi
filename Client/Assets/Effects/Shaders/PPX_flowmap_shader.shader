//警告！！！该shader完全未经过性能优化，请勿引入到项目中
//主要目的是方便动画师或者特效师制作个人demo，请勿用于任何商业用途
//个人知乎账号ID:shuang-miao-80 后续可能会有更新
//https://zhuanlan.zhihu.com/p/421146056
//技术谈论Q群:755239075
//最后,玩的开心!!!
Shader "Effect/PPX_FX/PPX_flowmap_shader"
{
	Properties
	{
		[Enum(ON,0,Off,1)]_Zwrite("Zwrite", Float) = 0
		[Enum(UnityEngine.Rendering.CullMode)]_Cull("Cull", Float) = 0
		_add_or_blend("add_or_blend", Range( 0 , 1)) = 1
		[NoScaleOffset]_base_texture("base_texture", 2D) = "white" {}
		_HUE("HUE", Range( 0 , 1)) = 0
		_Saturation("Saturation", Range( -1 , 1)) = 0
		_Value("Value", Range( -1 , 1)) = 0
		[HDR]_Base_color("Base_color", Color) = (1,1,1,0)
		_Glow("Glow", Range( 0 , 3)) = 1
		_alpha_power("alpha_power", Range( 0 , 3)) = 1
		[NoScaleOffset]_emissive_map("emissive_map", 2D) = "white" {}
		[HDR]_Emissive_color("Emissive_color", Color) = (1,1,1,0)
		_Emissive_power("Emissive_power", Range( 0 , 3)) = 1
		[Toggle(_USE_DISSOLVE_ON)] _use_dissolve("use_dissolve", Float) = 0
		[NoScaleOffset]_dissolve_texture("dissolve_texture", 2D) = "white" {}
		_edge_hardness("edge_hardness", Range( 0 , 22)) = 1
		_dissolve("dissolve", Range( 0 , 1)) = 1
		[Toggle(_USE_CURVE_DISSOLVE_ON)] _use_curve_dissolve("use_curve_dissolve", Float) = 0
		[NoScaleOffset]_flowmap("flowmap", 2D) = "white" {}
		_flow("flow", Range( 0 , 1.5)) = 1
		[Toggle(_USE_CUSTOM1_Z_FLOW_ON)] _use_custom1_z_flow("use_custom1_z_flow", Float) = 0
		_Distort_tex("Distort_tex", 2D) = "white" {}
		_Distort_uv("Distort_uv", Vector) = (1,1,0,0)
		_Distort_mask("Distort_mask", 2D) = "white" {}
		_Distort_mask_uv("Distort_mask_uv", Vector) = (1,1,0,0)
		_X_speed("X_speed", Range( -1 , 1)) = 1
		_Y_speed("Y_speed", Range( -1 , 1)) = 1
		_Time_scale("Time_scale", Range( 0 , 5)) = 1
		_Distort_power("Distort_power", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Transparent" }
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
			#pragma shader_feature_local _USE_CUSTOM1_Z_FLOW_ON
			#pragma shader_feature _USE_DISSOLVE_ON
			#pragma shader_feature _USE_CURVE_DISSOLVE_ON


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform float _Cull;
			uniform float _Zwrite;
			uniform sampler2D _base_texture;
			uniform sampler2D _Distort_tex;
			uniform float4 _Distort_uv;
			uniform float _Time_scale;
			uniform float _X_speed;
			uniform float _Y_speed;
			uniform float _Distort_power;
			uniform sampler2D _Distort_mask;
			uniform float4 _Distort_mask_uv;
			uniform sampler2D _flowmap;
			uniform float _flow;
			uniform float _HUE;
			uniform float _Saturation;
			uniform float _Value;
			uniform float4 _Base_color;
			uniform float _Glow;
			uniform sampler2D _emissive_map;
			uniform float4 _Emissive_color;
			uniform float _Emissive_power;
			uniform float _alpha_power;
			uniform sampler2D _dissolve_texture;
			uniform float _edge_hardness;
			uniform float _dissolve;
			uniform float _add_or_blend;
			float3 HSVToRGB( float3 c )
			{
				float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
				float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
				return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
			}
			
			float3 RGBToHSV(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
				float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
				float d = q.x - min( q.w, q.y );
				float e = 1.0e-10;
				return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.ase_color = v.color;
				o.ase_texcoord1 = v.ase_texcoord;
				o.ase_texcoord2 = v.ase_texcoord1;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
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
				float2 texCoord387 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult433 = (float2(_Distort_uv.x , _Distort_uv.y));
				float2 appendResult434 = (float2(_Distort_uv.z , _Distort_uv.w));
				float2 texCoord404 = i.ase_texcoord1.xy * appendResult433 + appendResult434;
				float mulTime412 = _Time.y * _Time_scale;
				float2 appendResult408 = (float2(_X_speed , _Y_speed));
				float2 normalizeResult414 = normalize( appendResult408 );
				float2 panner406 = ( mulTime412 * normalizeResult414 + texCoord404);
				float4 tex2DNode402 = tex2D( _Distort_tex, ( texCoord404 + panner406 ) );
				float2 appendResult417 = (float2(tex2DNode402.r , tex2DNode402.g));
				float2 appendResult437 = (float2(_Distort_mask_uv.x , _Distort_mask_uv.y));
				float2 appendResult438 = (float2(_Distort_mask_uv.z , _Distort_mask_uv.w));
				float2 texCoord435 = i.ase_texcoord1.xy * appendResult437 + appendResult438;
				float2 uv_flowmap385 = i.ase_texcoord1.xy;
				float4 texCoord400 = i.ase_texcoord2;
				texCoord400.xy = i.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _USE_CUSTOM1_Z_FLOW_ON
				float staticSwitch421 = ( _flow * texCoord400.x );
				#else
				float staticSwitch421 = _flow;
				#endif
				float2 lerpResult388 = lerp( ( texCoord387 + ( appendResult417 * _Distort_power * tex2D( _Distort_mask, texCoord435 ).r ) ) , (tex2D( _flowmap, uv_flowmap385 )).rg , staticSwitch421);
				float4 tex2DNode71 = tex2D( _base_texture, lerpResult388 );
				float3 hsvTorgb2_g56 = RGBToHSV( (tex2DNode71).xyz );
				float3 appendResult5_g56 = (float3(_HUE , _Saturation , _Value));
				float3 break6_g56 = appendResult5_g56;
				float3 hsvTorgb1_g56 = HSVToRGB( float3(( hsvTorgb2_g56.x + break6_g56.x ),( hsvTorgb2_g56.y + break6_g56.y ),( hsvTorgb2_g56.z + break6_g56.z )) );
				float4 texCoord398 = i.ase_texcoord1;
				texCoord398.xy = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_130_0 = ( i.ase_color.a * saturate( ( _alpha_power * tex2DNode71.a ) ) );
				float temp_output_7_0_g53 = _edge_hardness;
				float3 texCoord320 = i.ase_texcoord1.xyz;
				texCoord320.xy = i.ase_texcoord1.xyz.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _USE_CURVE_DISSOLVE_ON
				float staticSwitch212 = ( _dissolve * texCoord320.z );
				#else
				float staticSwitch212 = _dissolve;
				#endif
				float lerpResult2_g53 = lerp( temp_output_7_0_g53 , -1.0 , staticSwitch212);
				#ifdef _USE_DISSOLVE_ON
				float staticSwitch266 = ( temp_output_130_0 * saturate( saturate( ( ( tex2D( _dissolve_texture, lerpResult388 ).r * temp_output_7_0_g53 ) - lerpResult2_g53 ) ) ) );
				#else
				float staticSwitch266 = temp_output_130_0;
				#endif
				float temp_output_9_0_g63 = staticSwitch266;
				float4 appendResult2_g63 = (float4(( ( ( (i.ase_color).rgb * ( hsvTorgb1_g56 * (( _Base_color * _Glow )).rgb ) ) + (( ( tex2D( _emissive_map, lerpResult388 ).r * _Emissive_color ) * ( texCoord398.w * _Emissive_power ) )).rgb ) * temp_output_9_0_g63 ) , temp_output_9_0_g63));
				float4 appendResult6_g63 = (float4(appendResult2_g63.xyz , ( temp_output_9_0_g63 * _add_or_blend )));
				
				
				finalColor = appendResult6_g63;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "Flowmap_GUI"
	
	
}
