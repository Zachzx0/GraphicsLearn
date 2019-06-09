// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Unlit/DeferredShading"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("MainColor",Color)= (1,1,1,1)
	}
	SubShader
	{
		Tags { 
			"RenderType"="Opaque" 
			"LightMode" = "Deferred" 
		}
		LOD 100

		Pass
		{ 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 normal:NORMAL;
		};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 n : TEXCOORD1;
			};

			struct ObjGeomtryData {
				float4 gBuffer0 : SV_Target0;
				float4 gBuffer1 : SV_Target1;
				float4 gBuffer2 : SV_Target2;
				float4 gBuffer3 : SV_Target3;
			};
		
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.n = mul(v.normal, (float3x3)unity_WorldToObject);
				return o; 
			}


			ObjGeomtryData frag(v2f i) { 
				ObjGeomtryData output;
				output.gBuffer0.rgb = _Color.rgb;
				output.gBuffer0.a = 1;
				output.gBuffer1.rgb = fixed3(1,1,1);
				output.gBuffer1.a = 1;
				output.gBuffer2 = float4(i.n * 0.5 + 0.5, 1);
				//output.gBuffer3 = float4(1, 1, 1, 1);
				return output;
			}
			ENDCG
		}
	}
}
