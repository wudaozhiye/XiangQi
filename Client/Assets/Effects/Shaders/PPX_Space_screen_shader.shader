//警告！！！该shader完全未经过性能优化，请勿引入到项目中
//主要目的是方便动画师或者特效师制作个人demo，请勿用于任何商业用途
//个人知乎账号ID:shuang-miao-80 后续可能会有更新
//https://zhuanlan.zhihu.com/p/421146056
//技术谈论Q群:453762906
//最后,玩的开心!!!
Shader "Effect/PPX_FX/PPX_Space_screen_shader"
{
	Properties
	{
		[Enum(UnityEngine.Rendering.CompareFunction)]_Ztest("Ztest", Float) = 4
		[Enum(UnityEngine.Rendering.CullMode)]_Cull("Cull", Float) = 0
		[Enum(Off,0,On,1)]_Zwrite("Zwrite", Float) = 0
		[NoScaleOffset]_base_texture("base_texture", 2D) = "white" {}
		_base_uv("base_uv", Vector) = (1,1,0,0)
		_base_speed("base_speed", Vector) = (0,0,1,0)
		[Toggle(_USE_CHANGE_COLOR_ON)] _use_change_color("use_change_color", Float) = 0
		_HUE("HUE", Range( 0 , 1)) = 0
		_Saturation("Saturation", Range( -1 , 1)) = 0
		_Value("Value", Range( -1 , 1)) = 0
		_alpha_power("alpha_power", Range( 0 , 2)) = 1
		[HDR]_base_front_color("base_front_color", Color) = (1,1,1,0)
		_base_front_power("base_front_power", Range( 0 , 3)) = 1
		[NoScaleOffset]_mask_texture("mask_texture", 2D) = "white" {}
		_mask_uv("mask_uv", Vector) = (1,1,0,0)
		_mask_speed("mask_speed", Vector) = (0,0,0,0)
		[NoScaleOffset]_dissolve_texture("dissolve_texture", 2D) = "white" {}
		_dissolve_uv("dissolve_uv", Vector) = (1,1,0,0)
		_dissolve_speed("dissolve_speed", Vector) = (0,0,0,0)
		_edge_hardness("edge_hardness", Range( 0 , 22)) = 11
		_dissolve("dissolve", Range( 0 , 1)) = 1
		[Toggle(_USE_CUSTOM1_X_DISSOLVE_ON)] _use_custom1_x_dissolve("use_custom1_x_dissolve", Float) = 0

	}
	
	SubShader
	{
		
		
		// Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" }
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
		ZTest [_Ztest]
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
			#pragma shader_feature_local _USE_CHANGE_COLOR_ON
			#pragma shader_feature_local _USE_CUSTOM1_X_DISSOLVE_ON


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
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

			uniform half _Cull;
			uniform half _Zwrite;
			uniform half _Ztest;
			uniform sampler2D _base_texture;
			uniform half4 _base_speed;
			uniform half4 _base_uv;
			uniform half _HUE;
			uniform half _Saturation;
			uniform half _Value;
			uniform half4 _base_front_color;
			uniform half _base_front_power;
			uniform half _alpha_power;
			SamplerState sampler_base_texture;
			uniform sampler2D _mask_texture;
			SamplerState sampler_mask_texture;
			uniform half4 _mask_speed;
			uniform half4 _mask_uv;
			uniform sampler2D _dissolve_texture;
			SamplerState sampler_dissolve_texture;
			uniform half4 _dissolve_speed;
			uniform half4 _dissolve_uv;
			uniform half _edge_hardness;
			uniform half _dissolve;
			half3 HSVToRGB( half3 c )
			{
				half4 K = half4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
				half3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
				return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
			}
			
			half3 RGBToHSV(half3 c)
			{
				half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				half4 p = lerp( half4( c.bg, K.wz ), half4( c.gb, K.xy ), step( c.b, c.g ) );
				half4 q = lerp( half4( p.xyw, c.r ), half4( c.r, p.yzx ), step( p.x, c.r ) );
				half d = q.x - min( q.w, q.y );
				half e = 1.0e-10;
				return half3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord1 = screenPos;
				
				o.ase_color = v.color;
				o.ase_texcoord2.xyz = v.ase_texcoord.xyz;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.w = 0;
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
				half2 appendResult569 = (half2(_base_speed.x , _base_speed.y));
				half mulTime565 = _Time.y * _base_speed.z;
				half2 appendResult561 = (half2(_base_uv.x , _base_uv.y));
				float4 screenPos = i.ase_texcoord1;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				half2 appendResult564 = (half2(_base_uv.z , _base_uv.w));
				half4 tex2DNode71 = tex2D( _base_texture, ( ( appendResult569 * mulTime565 ) + ( ( appendResult561 * (( ase_screenPosNorm / ase_screenPosNorm.w )).xy ) + appendResult564 ) ) );
				half3 hsvTorgb2_g120 = RGBToHSV( (tex2DNode71).xyz );
				half3 appendResult5_g120 = (half3(_HUE , _Saturation , _Value));
				half3 break6_g120 = appendResult5_g120;
				half3 hsvTorgb1_g120 = HSVToRGB( half3(( hsvTorgb2_g120.x + break6_g120.x ),( hsvTorgb2_g120.y + break6_g120.y ),( hsvTorgb2_g120.z + break6_g120.z )) );
				#ifdef _USE_CHANGE_COLOR_ON
					half4 staticSwitch520 = half4( hsvTorgb1_g120 , 0.0 );
				#else
					half4 staticSwitch520 = tex2DNode71;
				#endif
				half4 break11_g117 = _mask_speed;
				half mulTime5_g117 = _Time.y * break11_g117.z;
				half2 appendResult6_g117 = (half2(break11_g117.x , break11_g117.y));
				half4 break10_g117 = _mask_uv;
				half2 appendResult2_g117 = (half2(break10_g117.x , break10_g117.y));
				half2 appendResult3_g117 = (half2(break10_g117.z , break10_g117.w));
				half2 texCoord4_g117 = i.ase_texcoord2.xy * appendResult2_g117 + appendResult3_g117;
				half2 panner1_g117 = ( mulTime5_g117 * appendResult6_g117 + texCoord4_g117);
				half4 break11_g118 = _dissolve_speed;
				half mulTime5_g118 = _Time.y * break11_g118.z;
				half2 appendResult6_g118 = (half2(break11_g118.x , break11_g118.y));
				half4 break10_g118 = _dissolve_uv;
				half2 appendResult2_g118 = (half2(break10_g118.x , break10_g118.y));
				half2 appendResult3_g118 = (half2(break10_g118.z , break10_g118.w));
				half2 texCoord4_g118 = i.ase_texcoord2.xyz.xy * appendResult2_g118 + appendResult3_g118;
				half2 panner1_g118 = ( mulTime5_g118 * appendResult6_g118 + texCoord4_g118);
				half temp_output_7_0_g119 = _edge_hardness;
				half3 texCoord320 = i.ase_texcoord2.xyz;
				texCoord320.xy = i.ase_texcoord2.xyz.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _USE_CUSTOM1_X_DISSOLVE_ON
					half staticSwitch212 = texCoord320.z;
				#else
					half staticSwitch212 = _dissolve;
				#endif
				half lerpResult2_g119 = lerp( temp_output_7_0_g119 , -1.0 , staticSwitch212);
				half dissolve448 = saturate( saturate( ( ( tex2D( _dissolve_texture, panner1_g118 ).r * temp_output_7_0_g119 ) - lerpResult2_g119 ) ) );
				half final_alpha466 = ( ( i.ase_color.a * saturate( ( _alpha_power * ( tex2DNode71.a * tex2D( _mask_texture, panner1_g117 ).r ) ) ) ) * dissolve448 );
				half4 appendResult465 = (half4((( ( half4( (i.ase_color).rgb , 0.0 ) * staticSwitch520 ) * _base_front_color * _base_front_power )).rgb , final_alpha466));
				half temp_output_9_0_g121 = saturate( (appendResult465).w );
				half4 appendResult2_g121 = (half4(( (appendResult465).xyz * temp_output_9_0_g121 ) , temp_output_9_0_g121));
				half4 appendResult6_g121 = (half4(appendResult2_g121.xyz , ( temp_output_9_0_g121 * 1.0 )));
				
				
				finalColor = appendResult6_g121;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "PPX_Space_screen_shader_Gui"
	
	
}
