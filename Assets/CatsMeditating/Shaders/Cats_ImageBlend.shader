Shader "Cats/ImageBlend"
{
    Properties
    {
        _MainTex ("First texture", 2D) = "white" {}
        _SecondTex ("Second texture", 2D) = "white" {}
        _BlendParam("Blend", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

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
            sampler2D _SecondTex;
            float _BlendParam;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 first = tex2D(_MainTex, i.uv);
                fixed4 second = tex2D(_SecondTex, i.uv);
                
                return lerp(first, second, _BlendParam);
            }
            ENDCG
        }
    }
}
