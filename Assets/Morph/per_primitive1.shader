Shader "Unlit/per_primitive1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
            };

            struct TriangleData
            {
                float3 vertex0 : POSITION;
                float3 vertex1 : POSITION;
                float3 vertex2 : POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            StructuredBuffer<TriangleData> _TriangleData;

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
                o.vertex = UnityObjectToClipPos(IN[0].vertex - _TriangleData[triangleID].vertex0 * sin(_Time.y));
                o.uv = IN[0].uv;
                o.triangleID = triangleID;
                triangleStream.Append(o);

                o.vertex = UnityObjectToClipPos(IN[1].vertex - _TriangleData[triangleID].vertex1 * sin(_Time.y));
                o.uv = IN[1].uv;
                o.triangleID = triangleID;
                triangleStream.Append(o);

                o.vertex = UnityObjectToClipPos(IN[2].vertex - _TriangleData[triangleID].vertex2 * sin(_Time.y));
                o.uv = IN[2].uv;
                o.triangleID = triangleID;
                triangleStream.Append(o);
            }

            fixed4 frag (g2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = fixed4(i.triangleID/25000, 0, 0, 1);
                return col;
            }
            ENDCG
        }
    }
}
