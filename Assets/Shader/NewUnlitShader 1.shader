Shader "Unlit/NewUnlitShader 1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainTex2 ("Texture", 2D) = "black" {}
        _DeltaTime("DeltaTime",float)=1
        _CamPosition("CamPosition",vector)= (0,0,0)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100


        //旋转位移测试
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

                o.vertex = mul(UNITY_MATRIX_VP, mul(v.vertex, otw));
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : COLOR0
            {
                fixed4 col;
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
                float3 normal : NORMAL0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : POSITION0;
                float3 lv : TEXCOORD1;
                float3 cv : TEXCOORD2;
                float3 normal : TEXCOORD3;
                float1 length : TEXCOORD4;
            };

            sampler2D _MainTex;
            sampler2D _MainTex2;
            float3 _CamPosition;

            v2f vert(appdata v)
            {
                v2f o;
                //float3 lightPosition = float3(0, 10, 1);
                const float3 lightPosition = _CamPosition;
                float3 position = mul(unity_ObjectToWorld, v.vertex);

                o.normal = v.normal;
                o.length = length(lightPosition - v.vertex);

                o.lv = normalize(lightPosition - position);
                o.cv = normalize(_CamPosition - position);

                o.vertex = mul(UNITY_MATRIX_VP, float4(position, 1));
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : COLOR0
            {
                float diffuseIntensity = max(0, dot(i.normal, i.lv));
                float fallOffItensity = 10 / pow(i.length, 2);

                float4 baseColor = tex2D(_MainTex, i.uv);
                float colorItensity = (baseColor.x + baseColor.y + baseColor.z) / 3 * 0.8f + 0.03f;
                float4 diffuse = float4(1, 1, 1, 1) * baseColor * diffuseIntensity * fallOffItensity * colorItensity;


                float3 h = normalize((i.cv + i.lv) / 2);
                float specularItensity = max(0.0f, dot(h, i.normal));
                //float3 h = normalize(Input.Normal * diffuseIntensity * 2 - Input.lv);
                //float specularItensity = max(0.0f, dot(h, Input.cv));
                float4 specular = float4(0.1f, 0.1f, 0.1f, 0.1f) * pow(specularItensity, 1.0f) * fallOffItensity;

                float4 ambient = float4(1, 1, 1, 1);

                return diffuse + specular;
            }
            ENDCG
        }
    }
}