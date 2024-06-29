Shader "Unlit/morph2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TimeScale ("Time Scale", Float) = 0.1
        _DistanceScale ("Distance Scale", Float) = 0.1
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2g
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            struct g2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _TimeScale;
            float _DistanceScale;

            v2g vert (appdata v)
            {
                v2g o;
                o.vertex = v.vertex;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            [maxvertexcount(3)]
			void geom(triangle v2g IN[3], inout TriangleStream<g2f> triangleStream)
			{
                g2f o;
                float3 centerNormal = normalize(cross(IN[1].vertex - IN[0].vertex, IN[2].vertex - IN[0].vertex));
                o.vertex = UnityObjectToClipPos(IN[0].vertex + centerNormal * sin(_Time.y * _TimeScale) * _DistanceScale);
                o.uv = IN[0].uv;
                triangleStream.Append(o);
                o.vertex = UnityObjectToClipPos(IN[1].vertex + centerNormal * sin(_Time.y * _TimeScale) * _DistanceScale);
                o.uv = IN[1].uv;
                triangleStream.Append(o);
                o.vertex = UnityObjectToClipPos(IN[2].vertex + centerNormal * sin(_Time.y * _TimeScale) * _DistanceScale);
                o.uv = IN[2].uv;
                triangleStream.Append(o);
			}

            fixed4 frag (g2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
