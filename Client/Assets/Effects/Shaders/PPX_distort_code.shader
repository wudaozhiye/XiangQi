
//警告！！！该shader完全未经过性能优化，请勿引入到项目中
//主要目的是方便动画师或者特效师制作个人demo，请勿用于任何商业用途
//个人知乎账号ID:shuang-miao-80 后续可能会有更新
//https://zhuanlan.zhihu.com/p/421146056
//技术谈论Q群:755239075
//最后,玩的开心!!!
Shader "Effect/PPX_FX/Distort_shader"
{
	Properties
	{
		_mask("mask", 2D) = "white" {}
		_BASE_ALPHA("BASE_ALPHA", Range(0,3)) = 1
		_shift("shift", Range(0,0.2)) = 0.1
		_normal_map("normal_map", 2D) = "white" {}
		[NoScaleOffset]_dissolve_texture("dissolve_texture", 2D) = "white" {}
		_DISSOLVE_UV("DISSOLVE_UV", Vector) = (1,1,0,0)
		_edge_hardness("edge_hardness", Range(0,22)) = 12
		_dissolve("dissolve", Range(0,1)) = 1
		[Toggle(_USE_CURVE_DISSOLVE_ON)] _use_curve_dissolve("use_curve_dissolve", Float) = 0
		[Toggle(_USE_CURVE_POWER_ON)] _use_curve_POWER("use_curve_POWER", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}
	
	SubShader
	{
		
		
 Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }

		LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaToMask Off
		Cull Off
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		GrabPass{ }

		Pass
		{
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM

			#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
				#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
			#else
				#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
			#endif


			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
				//only defining to not throw compilation error over Unity 5.5
				#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#pragma shader_feature_local _USE_CURVE_POWER_ON
			#pragma shader_feature_local _USE_CURVE_DISSOLVE_ON


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
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
			uniform float _shift;
			uniform sampler2D _dissolve_texture;
			SamplerState sampler_dissolve_texture;
			uniform float4 _DISSOLVE_UV;
			uniform float _edge_hardness;
			uniform float _dissolve;
			uniform sampler2D _normal_map;
			uniform float4 _normal_map_ST;
			uniform sampler2D _mask;
			SamplerState sampler_mask;
			uniform float4 _mask_ST;
			uniform float _BASE_ALPHA;
			inline float4 ASE_ComputeGrabScreenPos( float4 pos )
			{
				#if UNITY_UV_STARTS_AT_TOP
					float scale = -1.0;
				#else
					float scale = 1.0;
				#endif
				float4 o = pos;
				o.y = pos.w * 0.5f;
				o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
				return o;
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
				
				o.ase_texcoord2 = v.ase_texcoord;
				o.ase_texcoord3 = v.ase_texcoord1;
				o.ase_color = v.color;
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
				float4 screenPos = i.ase_texcoord1;
				float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( screenPos );
				float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
				float4 texCoord100 = i.ase_texcoord2;
				texCoord100.xy = i.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _USE_CURVE_POWER_ON
					float staticSwitch99 = ( _shift * texCoord100.z );
				#else
					float staticSwitch99 = _shift;
				#endif
				float2 texCoord111 = i.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult112 = (float2(_DISSOLVE_UV.x , _DISSOLVE_UV.y));
				float2 appendResult113 = (float2(_DISSOLVE_UV.z , _DISSOLVE_UV.w));
				float4 texCoord105 = i.ase_texcoord3;
				texCoord105.xy = i.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult106 = (float2(texCoord105.x , texCoord105.y));
				float temp_output_7_0_g47 = _edge_hardness;
				float4 texCoord68 = i.ase_texcoord2;
				texCoord68.xy = i.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _USE_CURVE_DISSOLVE_ON
					float staticSwitch73 = ( _dissolve * texCoord68.w );
				#else
					float staticSwitch73 = _dissolve;
				#endif
				float lerpResult2_g47 = lerp( temp_output_7_0_g47 , -1.0 , staticSwitch73);
				float DISSOLVE88 = saturate( saturate( ( ( tex2D( _dissolve_texture, ( ( ( texCoord111 * appendResult112 ) + appendResult113 ) + appendResult106 ) ).r * temp_output_7_0_g47 ) - lerpResult2_g47 ) ) );
				float2 uv_normal_map = i.ase_texcoord2.xy * _normal_map_ST.xy + _normal_map_ST.zw;
				float4 screenColor1 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( (ase_grabScreenPosNorm).xy + ( ( staticSwitch99 * DISSOLVE88 ) * (tex2D( _normal_map, uv_normal_map )).rg ) ));
				float2 uv_mask = i.ase_texcoord2.xy * _mask_ST.xy + _mask_ST.zw;
				float4 appendResult6 = (float4((screenColor1).rgb , saturate( ( ( tex2D( _mask, uv_mask ).r * _BASE_ALPHA ) * i.ase_color.a ) )));
				
				
				finalColor = appendResult6;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "distortGui"
	
	
}
