// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Flood"
{
	Properties
	{
		_PixelSize("PixelSize", Float) = 0.007936508
		_MainTex("MainTex", 2D) = "white" {}

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
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
			#include "UnityShaderVariables.cginc"


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

			uniform sampler2D _MainTex;
			uniform float _PixelSize;
			float4 MyCustomExpression15( sampler2D Tex , float2 UV , float PS , float Noise )
			{
				float4 base = tex2D(Tex, UV);
				float4 result = base;
				float isColored = result.a * Noise;
				for(int x = -1; x <= 1; x++)
				{
				for(int y = -1; y <= 1; y++)
				{
					float4 p = tex2D(Tex, UV + float2(x * PS, y * PS));
					float colored = p.a * Noise * isColored;	
					result = colored * min(p, result) + (1 - colored) * result;
				}
				}
				return max(result, base - 0.1 * max(base - result, 0.205));
			}
			

			
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
				sampler2D Tex15 = _MainTex;
				float2 uv03 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 UV15 = uv03;
				float PS15 = _PixelSize;
				float2 uv017 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float mulTime33 = _Time.y * 3.0;
				float Noise15 = step( 0.7 , abs( (-1.0 + (frac( (0.0 + (( distance( frac( ( ( uv017 * float2( 3,3 ) ) + mulTime33 ) ) , float2( 0.5,0.5 ) ) * ( abs( ( _SinTime.y + uv017.x ) ) + 1.0 ) * ( abs( ( uv017.y + _SinTime.z ) ) + 1.0 ) ) - 0.0) * (5.0 - 0.0) / (1.0 - 0.0)) ) - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) ) );
				float4 localMyCustomExpression15 = MyCustomExpression15( Tex15 , UV15 , PS15 , Noise15 );
				
				
				finalColor = localMyCustomExpression15;
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
774;149;1703;827;1491.588;475.1297;1.430491;True;True
Node;AmplifyShaderEditor.TextureCoordinatesNode;17;-1120,224;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-864,224;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;3,3;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;33;-976,352;Inherit;False;1;0;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinTimeNode;18;-1024,448;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;32;-672,224;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;34;-752,608;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-752,496;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;22;-512,224;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.AbsOpNode;40;-624,496;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;42;-624,608;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;43;-496,608;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;27;-272,224;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;41;-496,496;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-64,224;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;31;272,224;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;30;544,224;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;38;720,224;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;39;896,224;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1;-640,128;Inherit;False;Property;_PixelSize;PixelSize;0;0;Create;True;0;0;False;0;False;0.007936508;0.001984127;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;2;-704,-192;Inherit;True;Property;_MainTex;MainTex;1;0;Create;True;0;0;False;0;False;f4d31aa109c919d4595094f627510932;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.StepOpNode;36;1056,224;Inherit;True;2;0;FLOAT;0.7;False;1;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-704,0;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CustomExpressionNode;44;40.46747,-427.2082;Inherit;False;float4 base = tex2D(Tex, UV)@$float4 result = base@$float isColored = result.a * Noise@$for(int x = -1@ x <= 1@ x++)${$for(int y = -1@ y <= 1@ y++)${$	float4 p = tex2D(Tex, UV + float2(x * PS, y * PS))@$	float colored = p.a * Noise * isColored@	$	result = colored * min(p, result) + (1 - colored) * result@$}$}$return result@;4;False;4;True;Tex;SAMPLER2D;;In;;Inherit;False;True;UV;FLOAT2;0,0;In;;Inherit;False;True;PS;FLOAT;0;In;;Inherit;False;True;Noise;FLOAT;0;In;;Inherit;False;My Custom Expression;True;False;0;4;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CustomExpressionNode;15;-153.5,-126;Inherit;False;float4 base = tex2D(Tex, UV)@$float4 result = base@$float isColored = result.a * Noise@$for(int x = -1@ x <= 1@ x++)${$for(int y = -1@ y <= 1@ y++)${$	float4 p = tex2D(Tex, UV + float2(x * PS, y * PS))@$	float colored = p.a * Noise * isColored@	$	result = colored * min(p, result) + (1 - colored) * result@$}$}$return max(result, base - 0.1 * max(base - result, 0.205))@;4;False;4;True;Tex;SAMPLER2D;;In;;Inherit;False;True;UV;FLOAT2;0,0;In;;Inherit;False;True;PS;FLOAT;0;In;;Inherit;False;True;Noise;FLOAT;0;In;;Inherit;False;My Custom Expression;True;False;0;4;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CustomExpressionNode;16;-162.6175,-419.6718;Inherit;False;float4 result = tex2D(Tex, UV)@$float isColored = 1 - step( result.r, 0) + 1 - step(result.g, 0) + 1 - step(result.b, 0)@$isColored = saturate(isColored)@$for(int x = -1@ x <= 1@ x++)${$for(int y = -1@ y <= 1@ y++)${$	float4 p = tex2D(Tex, UV + float2(x * PS, y * PS))@$	float colored = 1 - step(p.r, 0) + 1 - step(p.g, 0) + 1 - step(p.b, 0)@	$	result = colored * min(p, result) + (1 - colored) * result@$}$}$return isColored * result + (1 - isColored) * float4(0,0,0,0)@;4;False;3;True;Tex;SAMPLER2D;;In;;Inherit;False;True;UV;FLOAT2;0,0;In;;Inherit;False;True;PS;FLOAT;0;In;;Inherit;False;My Custom Expression;True;False;0;3;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;96,-128;Float;False;True;-1;2;ASEMaterialInspector;100;1;Flood;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;0;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;True;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;0;False;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;;0
WireConnection;37;0;17;0
WireConnection;32;0;37;0
WireConnection;32;1;33;0
WireConnection;34;0;17;2
WireConnection;34;1;18;3
WireConnection;26;0;18;2
WireConnection;26;1;17;1
WireConnection;22;0;32;0
WireConnection;40;0;26;0
WireConnection;42;0;34;0
WireConnection;43;0;42;0
WireConnection;27;0;22;0
WireConnection;41;0;40;0
WireConnection;28;0;27;0
WireConnection;28;1;41;0
WireConnection;28;2;43;0
WireConnection;31;0;28;0
WireConnection;30;0;31;0
WireConnection;38;0;30;0
WireConnection;39;0;38;0
WireConnection;36;1;39;0
WireConnection;15;0;2;0
WireConnection;15;1;3;0
WireConnection;15;2;1;0
WireConnection;15;3;36;0
WireConnection;0;0;15;0
ASEEND*/
//CHKSM=0DDA9A53039FA5B2585CEB299037E7BD8255B5B0