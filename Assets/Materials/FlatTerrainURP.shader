Shader "Custom/FlatTerrainURP"
{
    Properties
    {
        [HideInInspector] _BaseMap("Control (RGBA)", 2D) = "white" {}
        [HideInInspector] _BaseMap0("Layer 0", 2D) = "white" {}
        [HideInInspector] _BaseMap1("Layer 1", 2D) = "white" {}
        [HideInInspector] _BaseMap2("Layer 2", 2D) = "white" {}
        [HideInInspector] _BaseMap3("Layer 3", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_BaseMap);     SAMPLER(sampler_BaseMap);
            TEXTURE2D(_BaseMap0);    SAMPLER(sampler_BaseMap0);
            TEXTURE2D(_BaseMap1);    SAMPLER(sampler_BaseMap1);
            TEXTURE2D(_BaseMap2);    SAMPLER(sampler_BaseMap2);
            TEXTURE2D(_BaseMap3);    SAMPLER(sampler_BaseMap3);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(OUT.worldPos);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Flat normal from derivatives
                float3 dx = ddx(IN.worldPos);
                float3 dy = ddy(IN.worldPos);
                float3 normal = normalize(cross(dx, dy));

                // Sample control (splat) map
                float4 control = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);

                // Sample terrain layers
                float3 col = 0;
                col += SAMPLE_TEXTURE2D(_BaseMap0, sampler_BaseMap0, IN.uv).rgb * control.r;
                col += SAMPLE_TEXTURE2D(_BaseMap1, sampler_BaseMap1, IN.uv).rgb * control.g;
                col += SAMPLE_TEXTURE2D(_BaseMap2, sampler_BaseMap2, IN.uv).rgb * control.b;
                col += SAMPLE_TEXTURE2D(_BaseMap3, sampler_BaseMap3, IN.uv).rgb * control.a;

                // Lighting
                Light mainLight = GetMainLight();
                float3 lightDir = normalize(mainLight.direction);
                float3 lightColor = mainLight.color;
                float NdotL = max(0, dot(normal, lightDir));

                float3 finalColor = col * lightColor * NdotL;

                return float4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/TerrainEngine/Splatmap/Lightmap-FirstPass"
}