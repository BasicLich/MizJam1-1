// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TileAlpha"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		[PerRendererData]_Column("Column", Int) = 2
		[PerRendererData]_Row("Row", Int) = 0
		[PerRendererData]_Color("Color", Color) = (1,1,1,0)
		[PerRendererData]_Offset("_Offset", Vector) = (0,0,0,0)

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha , SrcAlpha OneMinusSrcAlpha
		Cull Back
		ColorMask RGBA
		ZWrite On
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
			

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 ase_texcoord : TEXCOORD0;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
#endif
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord1 : TEXCOORD1;
			};

			uniform float4 _Color;
			uniform sampler2D _MainTex;
			uniform int _Column;
			uniform float2 _Offset;
			uniform int _Row;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
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
				float2 uv032 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_43_0 = ( ( 1.0 - ( 14.0 / 16.0 ) ) * 0.5 );
				float2 temp_cast_0 = (temp_output_43_0).xx;
				float2 temp_cast_1 = (( 1.0 - temp_output_43_0 )).xx;
				float2 break42 = (temp_cast_0 + (uv032 - float2( 0,0 )) * (temp_cast_1 - temp_cast_0) / (float2( 1,1 ) - float2( 0,0 )));
				float2 appendResult35 = (float2(( ( break42.x + ( _Column + ( _Offset.x / 14.0 ) ) ) / 48.0 ) , ( ( break42.y + ( _Row + ( _Offset.y / 14.0 ) ) ) / 22.0 )));
				
				
				finalColor = ( _Color * tex2D( _MainTex, appendResult35 ).a );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback "False"
}
/*ASEBEGIN
Version=18100
178;129;1703;833;702.405;407.4095;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;36;-1376,-16;Inherit;False;Constant;_BaseTileSize;BaseTileSize;3;0;Create;True;0;0;False;0;False;16;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-1376,-96;Inherit;False;Constant;_FinalTileSize;FinalTileSize;3;0;Create;True;0;0;False;0;False;14;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;39;-1200,-96;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;41;-1088,-96;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-944,-96;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;48;-416,560;Inherit;False;Property;_Offset;_Offset;4;1;[PerRendererData];Create;True;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;32;-896,32;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;44;-800,-128;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;50;-240,656;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;49;-240,560;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;31;-400,208;Inherit;False;Property;_Column;Column;1;1;[PerRendererData];Create;True;0;0;False;0;False;2;2;0;1;INT;0
Node;AmplifyShaderEditor.IntNode;21;-400,288;Inherit;False;Property;_Row;Row;2;1;[PerRendererData];Create;True;0;0;False;0;False;0;0;0;1;INT;0
Node;AmplifyShaderEditor.TFHCRemapNode;40;-640,-96;Inherit;True;5;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,1;False;3;FLOAT2;0,0;False;4;FLOAT2;1,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;42;-368,-96;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;51;-160,160;Inherit;False;2;2;0;INT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;52;-160,272;Inherit;False;2;2;0;INT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-448,384;Inherit;False;Constant;_ColumnsAmount1;ColumnsAmount;0;0;Create;True;0;0;False;0;False;48;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-448,464;Inherit;False;Constant;_RowsAmount;RowsAmount;0;0;Create;True;0;0;False;0;False;22;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;33;0,160;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;34;0,272;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;22;144,272;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;23;144,160;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;35;272,160;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;4;-2,-220;Inherit;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;False;81059de5c5a2028498764de13393e337;81059de5c5a2028498764de13393e337;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;5;448,144;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;46;528,-32;Inherit;False;Property;_Color;Color;3;1;[PerRendererData];Create;True;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;768,96;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;912,96;Float;False;True;-1;2;ASEMaterialInspector;100;1;TileAlpha;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;2;5;False;-1;10;False;-1;2;5;False;-1;10;False;-1;True;0;False;-1;0;False;-1;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;0;False;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;;0
WireConnection;39;0;37;0
WireConnection;39;1;36;0
WireConnection;41;0;39;0
WireConnection;43;0;41;0
WireConnection;44;0;43;0
WireConnection;50;0;48;2
WireConnection;50;1;37;0
WireConnection;49;0;48;1
WireConnection;49;1;37;0
WireConnection;40;0;32;0
WireConnection;40;3;43;0
WireConnection;40;4;44;0
WireConnection;42;0;40;0
WireConnection;51;0;31;0
WireConnection;51;1;49;0
WireConnection;52;0;21;0
WireConnection;52;1;50;0
WireConnection;33;0;42;0
WireConnection;33;1;51;0
WireConnection;34;0;42;1
WireConnection;34;1;52;0
WireConnection;22;0;34;0
WireConnection;22;1;19;0
WireConnection;23;0;33;0
WireConnection;23;1;20;0
WireConnection;35;0;23;0
WireConnection;35;1;22;0
WireConnection;5;0;4;0
WireConnection;5;1;35;0
WireConnection;47;0;46;0
WireConnection;47;1;5;4
WireConnection;1;0;47;0
ASEEND*/
//CHKSM=B40FC002BF90E2D2ABD47A7EDC3F7E8BC73AEC11