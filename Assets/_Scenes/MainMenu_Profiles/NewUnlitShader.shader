Shader "Custom/VHSGlitchShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlitchAmount ("Glitch Amount", Range(0, 1)) = 0.1
        _ScanLines ("Scan Lines", Float) = 200.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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
            float _GlitchAmount;
            float _ScanLines;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Glitch effect
                float2 uv = i.uv;
                uv.x += sin(_Time.y * 10.0) * _GlitchAmount * 0.1;
                uv.y += cos(_Time.y * 15.0) * _GlitchAmount * 0.1;

                // Scan lines
                float scanLines = sin(uv.y * _ScanLines * 3.14159);
                scanLines = smoothstep(0.5, 0.6, scanLines);

                // Combine effects
                fixed4 col = tex2D(_MainTex, uv);
                col.rgb *= scanLines;

                return col;
            }
            ENDCG
        }
    }
}