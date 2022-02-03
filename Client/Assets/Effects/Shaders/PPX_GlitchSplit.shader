//警告！！！该shader完全未经过性能优化，请勿引入到项目中
//主要目的是方便动画师或者特效师制作个人demo，请勿用于任何商业用途
//个人知乎账号ID:shuang-miao-80 后续可能会有更新
//https://zhuanlan.zhihu.com/p/421146056
//技术谈论Q群:755239075
//最后,玩的开心!!!
Shader "Effect/PPX_FX/PPX_GlitchSplit"
{
	Properties
	{

		[Enum(Off, 0, On, 1)] _ZWrite ("ZWrite", Float) = 0
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Float) = 8
		[Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", Float) = 0


		_mask("mask", 2D) = "white" {}
		_Base_alpha("Base_alpha", Range( 0 , 3)) = 1
		_scale("scale", Range( 0 , 1)) = 0.1
		_shift("shift", Range( 0 , 1)) = 1
		[Toggle(_CUSTOM1_X_CONTROL_SHIFT_ON)] _custom1_x_control_shift("custom1_x_control_shift", Float) = 0
		[NoScaleOffset]_mask2("mask2", 2D) = "white" {}
		_noise_mask_uv("noise_mask_uv", Vector) = (1,1,0,0)
		_X_speed("X_speed", Range( -1 , 1)) = 1
		_Y_speed("Y_speed", Range( -1 , 1)) = 0
		_speed("speed", Range( 0 , 4)) =1
		[Toggle(_CUSTOM1_ZW_MOVE_UV_ON)] _custom1_zw_move_uv("custom1_zw_move_uv", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}
	
	SubShader
	{
		
		
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" }
		LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaToMask Off
		// Cull Off
		ColorMask RGBA
		// ZWrite On
		// ZTest LEqual
		Offset 0 , 0
		
		ZWrite [_ZWrite]
		ZTest [_ZTest]
		Cull [_Cull]
		


		
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
			#include "UnityShaderVariables.cginc"
			#pragma shader_feature_local _CUSTOM1_X_CONTROL_SHIFT_ON
			#pragma shader_feature_local _CUSTOM1_ZW_MOVE_UV_ON


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
			uniform sampler2D _mask2;
			SamplerState sampler_mask2;
			uniform float _speed;
			uniform float _X_speed;
			uniform float _Y_speed;
			uniform float4 _noise_mask_uv;
			uniform float _scale;
			uniform sampler2D _mask;
			SamplerState sampler_mask;
			uniform float4 _mask_ST;
			uniform float _Base_alpha;
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
				float2 temp_output_12_0 = (ase_grabScreenPosNorm).xy;
				float4 texCoord36 = i.ase_texcoord2;
				texCoord36.xy = i.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _CUSTOM1_X_CONTROL_SHIFT_ON
					float staticSwitch40 = texCoord36.z;
				#else
					float staticSwitch40 = _shift;
				#endif
				float mulTime54 = _Time.y * _speed;
				float2 appendResult47 = (float2(_X_speed , _Y_speed));
				float2 normalizeResult50 = normalize( appendResult47 );
				float2 appendResult52 = (float2(_noise_mask_uv.x , _noise_mask_uv.y));
				float2 appendResult53 = (float2(_noise_mask_uv.z , _noise_mask_uv.w));
				float2 texCoord33 = i.ase_texcoord2.xy * appendResult52 + appendResult53;
				float2 panner46 = ( mulTime54 * normalizeResult50 + texCoord33);
				float4 texCoord41 = i.ase_texcoord3;
				texCoord41.xy = i.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float4 appendResult42 = (float4(texCoord41.x , texCoord41.y , 0.0 , 0.0));
				#ifdef _CUSTOM1_ZW_MOVE_UV_ON
					float4 staticSwitch43 = ( float4( panner46, 0.0 , 0.0 ) + appendResult42 );
				#else
					float4 staticSwitch43 = float4( panner46, 0.0 , 0.0 );
				#endif
				float temp_output_21_0 = ( ( staticSwitch40 * tex2D( _mask2, staticSwitch43.xy ).r ) * _scale );
				float2 appendResult19 = (float2(temp_output_21_0 , 0.0));
				float4 screenColor1 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( temp_output_12_0 + appendResult19 ));
				float4 screenColor27 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,temp_output_12_0);
				float2 appendResult25 = (float2(( temp_output_21_0 * -1.0 ) , 0.0));
				float4 screenColor28 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( temp_output_12_0 + appendResult25 ));
				float3 appendResult17 = (float3(screenColor1.r , screenColor27.g , screenColor28.b));
				float2 uv_mask = i.ase_texcoord2.xy * _mask_ST.xy + _mask_ST.zw;
				float4 appendResult6 = (float4((appendResult17).xyz , saturate( ( ( tex2D( _mask, uv_mask ).r * _Base_alpha ) * i.ase_color.a ) )));
				
				
				finalColor = appendResult6;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ColorShiftGui"
	
	
}
/*ASEBEGIN
Version=18500
2718;213;2560;1349;4197.198;-444.1667;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;48;-3901.436,888.0757;Inherit;False;Property;_X_speed;X_speed;7;0;Create;True;0;0;False;0;False;1;1;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-3893.436,990.0757;Inherit;False;Property;_Y_speed;Y_speed;8;0;Create;True;0;0;False;0;False;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;32;-3993.021,505.9489;Inherit;False;Property;_noise_mask_uv;noise_mask_uv;6;0;Create;True;0;0;False;0;False;1,1,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;53;-3739.436,697.0757;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-3845.436,1161.076;Inherit;False;Property;_speed;speed;9;0;Create;True;0;0;False;0;False;1.941176;0;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;52;-3729.436,549.0757;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;47;-3601.436,898.0757;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NormalizeNode;50;-3430.436,903.0757;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;33;-3414.882,592.8538;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;41;-3407.014,1198.997;Inherit;False;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;54;-3668.198,1221.167;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;42;-3061.215,1271.797;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PannerNode;46;-3100.436,690.0757;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;35;-2786.796,1007.076;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-2739.189,293.7329;Inherit;False;Property;_shift;shift;3;0;Create;True;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;36;-2860.719,422.4231;Inherit;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;43;-2569.215,736.1956;Inherit;True;Property;_custom1_zw_move_uv;custom1_zw_move_uv;10;0;Create;True;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT4;0,0,0,0;False;0;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,0,0;False;7;FLOAT4;0,0,0,0;False;8;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;30;-2372.541,820.323;Inherit;True;Property;_mask2;mask2;5;1;[NoScaleOffset];Create;True;0;0;False;0;False;-1;f3d73c60f6a206b4e8e7b017350d56d8;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;40;-2276.476,384.1929;Inherit;False;Property;_custom1_x_control_shift;custom1_x_control_shift;4;0;Create;True;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-2088.362,585.2517;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-2044.897,725.0225;Inherit;False;Property;_scale;scale;2;0;Create;True;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-1761.23,835.8756;Inherit;False;Constant;_Float3;Float 3;4;0;Create;True;0;0;False;0;False;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-1898.34,421.213;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-1616.47,691.5488;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GrabScreenPosition;10;-1826.315,54.51969;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;20;-1671.299,563.2758;Inherit;False;Constant;_Float2;Float 2;3;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-1464.778,836.9907;Inherit;False;Constant;_Float0;Float 0;3;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;25;-1399.321,652.4677;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SwizzleNode;12;-1482.947,110.19;Inherit;False;FLOAT2;0;1;2;3;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;19;-1584.642,428.8095;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;29;-1223.872,466.6981;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-131,515.5;Inherit;False;Property;_Base_alpha;Base_alpha;1;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-1189.233,81.81508;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;5;-320,273.5;Inherit;True;Property;_mask;mask;0;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenColorNode;27;-918.1046,125.3563;Inherit;False;Global;_GrabScreen1;Grab Screen 1;0;0;Create;True;0;0;False;0;False;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenColorNode;1;-1001.258,-114.1479;Inherit;False;Global;_GrabScreen0;Grab Screen 0;0;0;Create;True;0;0;False;0;False;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;61,173.5;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;28;-890.6711,383.1419;Inherit;False;Global;_GrabScreen2;Grab Screen 2;0;0;Create;True;0;0;False;0;False;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;37;33.9523,616.8899;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;286.4564,370.8874;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;17;-312.0077,-119.0901;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SwizzleNode;7;140,-21.5;Inherit;False;FLOAT3;0;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;44;530.3113,174.2401;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;6;355,4.5;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;569,-49;Float;False;True;-1;2;ColorShiftGui;100;1;mask;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;False;False;False;False;False;False;True;0;False;-1;True;0;False;-1;True;True;True;True;True;0;False;-1;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;7;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;;False;0
WireConnection;53;0;32;3
WireConnection;53;1;32;4
WireConnection;52;0;32;1
WireConnection;52;1;32;2
WireConnection;47;0;48;0
WireConnection;47;1;49;0
WireConnection;50;0;47;0
WireConnection;33;0;52;0
WireConnection;33;1;53;0
WireConnection;54;0;51;0
WireConnection;42;0;41;1
WireConnection;42;1;41;2
WireConnection;46;0;33;0
WireConnection;46;2;50;0
WireConnection;46;1;54;0
WireConnection;35;0;46;0
WireConnection;35;1;42;0
WireConnection;43;1;46;0
WireConnection;43;0;35;0
WireConnection;30;1;43;0
WireConnection;40;1;15;0
WireConnection;40;0;36;3
WireConnection;31;0;40;0
WireConnection;31;1;30;1
WireConnection;21;0;31;0
WireConnection;21;1;22;0
WireConnection;23;0;21;0
WireConnection;23;1;24;0
WireConnection;25;0;23;0
WireConnection;25;1;45;0
WireConnection;12;0;10;0
WireConnection;19;0;21;0
WireConnection;19;1;20;0
WireConnection;29;0;12;0
WireConnection;29;1;25;0
WireConnection;26;0;12;0
WireConnection;26;1;19;0
WireConnection;27;0;12;0
WireConnection;1;0;26;0
WireConnection;4;0;5;1
WireConnection;4;1;9;0
WireConnection;28;0;29;0
WireConnection;38;0;4;0
WireConnection;38;1;37;4
WireConnection;17;0;1;1
WireConnection;17;1;27;2
WireConnection;17;2;28;3
WireConnection;7;0;17;0
WireConnection;44;0;38;0
WireConnection;6;0;7;0
WireConnection;6;3;44;0
WireConnection;0;0;6;0
ASEEND*/
//CHKSM=671944AD23DB73A9C6DA68899C7760E1880DFBBB