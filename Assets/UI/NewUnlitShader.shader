Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Brightness("Brightness",Range(0,100))=1
        _Saturation("Saturation",Range(0,100))=1
        _Contrast("_Contrast",Range(0,100))=1
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _Brightness;
            fixed _Saturation;
            fixed _Contrast;


            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                //return col;

                fixed4 texResult = tex2D(_MainTex, i.uv);
                //计算亮度                    
                fixed3 color = texResult * _Brightness;
                //计算饱和度
                fixed luminance = 0.2125 * texResult.r + 0.7154 * texResult.r + 0.0721 * texResult.b;
                fixed3 luminanceColor = fixed3(luminance, luminance, luminance);
                color = lerp(luminanceColor, color, _Saturation);
                //计算对比读度
                fixed3 avgColor = fixed3(0.5, 0.5, 0.5);
                color = lerp(avgColor, color, _Contrast);
                return fixed4(color, 1);
            }
            ENDCG
        }
    }
}