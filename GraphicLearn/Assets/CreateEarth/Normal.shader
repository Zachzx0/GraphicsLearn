Shader "Unlit/Normal"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_NormalTex ("Normal Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
#include "Lighting.cginc"
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal:NORMAL;
				float4 tangent : TANGENT;
	};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 light :TEXCOORD1;
			};

			sampler2D _MainTex;
			sampler2D _NormalTex;
			float4 _MainTex_ST;
			float4 _NormalTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.uv, _NormalTex);
				 float3 binormal = cross(normalize(v.normal), normalize(v.tangent.xyz))*v.tangent.w;
				float3x3 rotation = float3x3(v.tangent.xyz,binormal,v.normal);
				o.light = mul(rotation, ObjSpaceLightDir(v.vertex));
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv.xy);
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
				float3 tangentNormal = UnpackNormal(tex2D(_NormalTex, i.uv.zw));
				float3 tangentLight = normalize(i.light);
				fixed3 lambert = 0.5 * dot(tangentNormal, tangentLight) + 0.5;
				fixed3 diffuse = lambert  * _LightColor0.xyz + ambient;
				fixed4 color = tex2D(_MainTex, i.uv);
				return fixed4(lambert * color.rgb, 1.0);
			}
			ENDCG
		}
	}
}
