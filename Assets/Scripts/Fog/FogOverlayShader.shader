Shader "Custom/FogOverlay"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _ShipPos ("Ship World Position", Vector) = (0, 0, 0, 0)
        _FogStart ("Fog Start", Float) = 10
        _FogEnd ("Fog End", Float) = 30
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float3 worldPos : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float3 _ShipPos;
            float _FogStart;
            float _FogEnd;

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color;
                output.worldPos = mul(UNITY_MATRIX_M, float4(input.positionOS.xyz, 1.0)).xyz;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                color *= input.color;

                float dist = length(input.worldPos - _ShipPos);
                float blendRadius = max(_FogEnd - _FogStart, 0.01f);

                // 0 = fully transparent (clear view), 1 = fully opaque black (fog)
                float fogAmount = saturate((dist - _FogStart) / blendRadius);
                fogAmount = smoothstep(0.0, 1.0, fogAmount);

                color.a *= fogAmount;
                return color;
            }
            ENDHLSL
        }
    }
    Fallback "Sprites/Default"
}
