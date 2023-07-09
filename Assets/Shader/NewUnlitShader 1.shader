Shader "Unlit/NewUnlitShader 1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainTex2 ("Texture", 2D) = "black" {}
        _DeltaTime("DeltaTime",float)=1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION0;
                float2 uv : TEXCOORD0;
                float4 normal: NORMAL0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : POSITION0;
            };

            sampler2D _MainTex;
            sampler2D _MainTex2;
            float _DeltaTime;

            v2f vert(appdata v)
            {
                v2f o;
                float4x4 otw = {
                    1, 0, 0, 0,
                    0, 1, 0, 0,
                    0, 0, 1, 0,
                    0, 0, 1, 1
                };
                float1 t = _DeltaTime * 100;

                float4x4 rotateY = {
                    cos(t * 3.14 / 180), -sin(t * 3.14 / 180), 0, 0,
                    sin(t * 3.14 / 180), cos(t * 3.14 / 180), 0, 0,
                    0, 0, 1, 0,
                    0, 0, 0, 1
                };

                otw = mul(rotateY, otw);

                o.vertex = mul(UNITY_MATRIX_VP, mul(v.vertex, otw
                               ));
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : COLOR0
            {
                fixed4 col;
                //col = tex2D(_MainTex, i.uv + _Time * 0.1);
                col = tex2D(_MainTex, i.uv);


                return col;
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION0;
                float2 uv : TEXCOORD0;
                float2 normal : NORMAL0;
                float4 diff:COLOR0;
                float4 sped:COLOR1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : POSITION0;
                float2 normal : NORMAL0;
                float4 diff:COLOR0;
                float4 sped:COLOR1;
            };

            sampler2D _MainTex;
            sampler2D _MainTex2;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = mul(unity_ObjectToWorld, v.vertex);
                o.normal = mul(unity_ObjectToWorld, v.normal);

                float3 lightSource = (0, 10, 0);
                float3 ligntToVertex = o.vertex - lightSource;
                //diffuseValue = 100 * float3(1, 0, 0) * max(dot(lTov, normal), 0);


                o.vertex = mul(UNITY_MATRIX_VP, v.vertex);
                o.uv = v.uv;
                return o;

                float4x4 otw = {
                    1, 0, 0, 0,
                    0, 1, 0, 0,
                    0, 0, 1, 0,
                    0, 0, 0, 1
                };
            }

            fixed4 frag(v2f i) : COLOR0
            {
                fixed4 col;
                // if (i.vertex.y < 500)
                // {
                //     col = tex2D(_MainTex, i.uv) * 0.5 + tex2D(_MainTex2, i.uv) * 0.5;
                // }
                // else
                // {
                //     //col = tex2D(_MainTex, i.uv + _Time * 0.1);
                //     col = tex2D(_MainTex, i.uv);
                // }

                col = tex2D(_MainTex, i.uv);


                return col;
            }
            ENDCG
        }
    }
}