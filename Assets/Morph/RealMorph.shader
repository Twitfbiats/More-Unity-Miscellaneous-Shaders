Shader "Unlit/RealMorph"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MorphTex ("Morph Texture", 2D) = "white" {}
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
                float triangleID : TEXCOORD1;
                float2 morphUV : TEXCOORD2;
            };

            struct PerTriangleData
            {
                float3 v0 : POSITION;
                float3 v1 : POSITION;
                float3 v2 : POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _MorphTex;
            float4 _MorphTex_ST;
            StructuredBuffer<PerTriangleData> _PerTriangleData;

            v2g vert (appdata v)
            {
                v2g o;
                o.vertex = v.vertex;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            [maxvertexcount(3)]
            void geom(triangle v2g IN[3], inout TriangleStream<g2f> triangleStream, uint triangleID: SV_PRIMITIVEID)
            {
                g2f o;
                o.vertex = UnityObjectToClipPos(lerp(IN[0].vertex, _PerTriangleData[triangleID].v0, abs(sin(_Time.y))));
                o.uv = IN[0].uv;
                o.morphUV = _PerTriangleData[triangleID].uv0;
                o.triangleID = triangleID;
                triangleStream.Append(o);

                o.vertex = UnityObjectToClipPos(lerp(IN[1].vertex, _PerTriangleData[triangleID].v1, abs(sin(_Time.y))));
                o.uv = IN[1].uv;
                o.morphUV = _PerTriangleData[triangleID].uv1;
                o.triangleID = triangleID;
                triangleStream.Append(o);

                o.vertex = UnityObjectToClipPos(lerp(IN[2].vertex, _PerTriangleData[triangleID].v2, abs(sin(_Time.y))));
                o.uv = IN[2].uv;
                o.morphUV = _PerTriangleData[triangleID].uv2;
                o.triangleID = triangleID;
                triangleStream.Append(o);
            }

            fixed4 frag (g2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = lerp(tex2D(_MainTex, i.uv), tex2D(_MorphTex, i.morphUV), abs(sin(_Time.y)));
                return col;
            }
            ENDCG
        }
    }
}
