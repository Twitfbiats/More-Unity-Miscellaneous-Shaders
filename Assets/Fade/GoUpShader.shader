Shader "Vip/GeometryShaderWithDisappear"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Speed("Speed", Float) = 5
        _DelayTimeByHeight("DelayTimeByHeight", Float) = 2
		_GeoRelativePosPortion("GeoRelativePosPortion", Float) = 0.2
		_TimeMultiplier("TimeMultiplier", Float) = 1
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
			#pragma geometry geom
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal: NORMAL;
				float3 tangent: TANGENT;
			};

			struct v2g
			{
				float2 uv : TEXCOORD0;
				float4 vertex : POSITION;
				float3 normal: NORMAL;
				float3 tangent: TANGENT;
				float3 worldPos: TEXCOORD1;
			};

			struct g2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 normal: NORMAL;
				float3 tangent: TANGENT;
				float3 worldPos: TEXCOORD1;
			};

			sampler2D _MainTex;

			float _Speed;
			float _StartTime;
            float _DelayTimeByHeight;
			float _GeoRelativePosPortion;
			float _TimeMultiplier;

			v2g vert (appdata v)
			{
				v2g o;
				o.vertex = v.vertex;
				o.uv = v.uv;
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.tangent = UnityObjectToWorldDir(v.tangent);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}

			[maxvertexcount(3)]
			void geom(triangle v2g IN[3], inout TriangleStream<g2f> triangleStream)
			{
                float realTime = (_Time.y - _StartTime) * _TimeMultiplier;
				g2f o;
				float3 tempPos = (IN[0].vertex + IN[1].vertex + IN[2].vertex) / 3;

				if (tempPos.y + realTime > _DelayTimeByHeight)
                {
					float3 dir1 = IN[0].vertex - tempPos;
					float3 dir2 = IN[1].vertex - tempPos;
					float3 dir3 = IN[2].vertex - tempPos;
                    tempPos = float3(tempPos.x, tempPos.y + (realTime - _DelayTimeByHeight + tempPos.y) * _Speed * step(_DelayTimeByHeight, tempPos.y + realTime), tempPos.z);

                    o.uv = (IN[0].uv + IN[1].uv + IN[2].uv) / 3;
					o.normal = (IN[0].normal + IN[1].normal + IN[2].normal) / 3;
					o.tangent = (IN[0].tangent + IN[1].tangent + IN[2].tangent) / 3;
					o.worldPos = (IN[0].worldPos + IN[1].worldPos + IN[2].worldPos) / 3;

					o.vertex = UnityObjectToClipPos(tempPos + dir1 * _GeoRelativePosPortion);
                    triangleStream.Append(o);
					o.vertex = UnityObjectToClipPos(tempPos + dir2 * _GeoRelativePosPortion);
					triangleStream.Append(o);
					o.vertex = UnityObjectToClipPos(tempPos + dir3 * _GeoRelativePosPortion);
					triangleStream.Append(o);
                }
                else
                {
                    o.vertex = UnityObjectToClipPos(IN[0].vertex);
                    o.uv = IN[0].uv;
					o.normal = IN[0].normal;
					o.tangent = IN[0].tangent;
					o.worldPos = IN[0].worldPos;
                    triangleStream.Append(o);
                    o.vertex = UnityObjectToClipPos(IN[1].vertex);
                    o.uv = IN[1].uv;
					o.normal = IN[1].normal;
					o.tangent = IN[1].tangent;
					o.worldPos = IN[1].worldPos;
                    triangleStream.Append(o);
                    o.vertex = UnityObjectToClipPos(IN[2].vertex);
                    o.uv = IN[2].uv;
					o.normal = IN[2].normal;
					o.tangent = IN[2].tangent;
					o.worldPos = IN[2].worldPos;
                    triangleStream.Append(o);
                }
			}
			
			fixed4 frag (g2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				
				// Calculate lighting using Unity's built-in functions
                float3 worldNormal = normalize(i.normal);
                float3 worldPos = i.worldPos;

                // Simple diffuse lighting calculation
                float3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
                float3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));

                // Calculate the diffuse term
                float diff = max(0, dot(worldNormal, lightDir));
                half3 diffuse = diff * _LightColor0.rgb;

                // Sample the ambient light
                half3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

                // Combine texture color with lighting
                half3 color = col.rgb * (diffuse + ambient);
                return half4(color, col.a);
			}
			ENDCG
		}
	}
}