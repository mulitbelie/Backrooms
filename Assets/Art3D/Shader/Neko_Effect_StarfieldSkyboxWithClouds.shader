// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Neko/Effect/StarfieldSkyboxWithClouds"
{
Properties
    {
        //【夜晚控制】
        _Exposure ("曝光度", Range(0, 8)) = 1.0
        //【地平线设置】
        _OffsetHorizon ("地平线：位移", Range(-1, 1)) = 0
        _HorizonSmoothness ("地平线：过渡柔和度", Range(0.1, 3)) = 1.5
        _HorizonGlowRange ("地平线发光：范围", Range(0.5, 10)) = 3.3
        _HorizonGlowIntensity ("地平线发光：强度", Range(0, 3)) = 0.8
        _HorizonBlendFactor ("地平线：颜色混合度", Range(0, 1)) = 0.7
        _HorizonColorNight ("夜晚：地平线颜色", Color) = (0.1, 0.1, 0.3, 1)
        _NightFogColor ("夜晚：低空雾色", Color) = (0.05, 0.05, 0.15, 1)
        //【夜晚天空设置】
        _NightTopColor ("夜空：顶部颜色", Color) = (0, 0, 0.05, 1)
        _NightBottomColor ("夜空：底部颜色", Color) = (0.03, 0.03, 0.15, 1)
        //【云朵设置 - 增强版】
        [Toggle(_CLOUD_ON)] _CLOUD_ON("云朵：开启/关闭", Float) = 1
        [NoScaleOffset]_CloudNoiseTexture("云贴图", 2D) = "black" {}   
        _CloudColor1("云朵：主色", Color) = (0.2, 0.2, 0.35, 1)                        
        _CloudColor2("云朵：阴影色", Color) = (0.1, 0.1, 0.2, 1)  
        _CloudPower("云朵：强度", Range(0.1, 3.0)) = 1.2
        _CloudAmount("云朵：数量倍增", Range(0.1, 5.0)) = 2.5
        _CloudCoverage("云朵：覆盖率", Range(0, 1)) = 0.7
        _CloudFadePosition("云朵：远处消失位置", Range(0, .97)) = .74              
        _CloudFadeAmount("云朵：远处消失程度", Range(0, 1)) = .6                     
        _CloudDensity("云朵：密度", Range(0, 2)) = 1.2
        _CloudSmallDetails("云朵：细节强度", Range(0, 1)) = 0.4
        _CloudSpeed("云朵：漂移速度", Range(0, 0.5)) = .05                               
        _CloudDirection("云朵：漂移方向", Range(0, 6.283)) = 1.57                  
        _CloudHeight("云朵：高度", Range(0, 1)) = .6                             
        _CloudTextureTiling("云朵：基础纹理缩放", Range(.01, 10)) = 1.5
        _CloudDetailTiling("云朵：细节纹理缩放", Range(.01, 10)) = 3.0
        _CloudAlpha("云朵：透明度", Range(0.0, 1.0)) = 0.9                            
        //【星星设置】
        _Stars ("星星:贴图", 2D) = "black" { }
        _StarsCutoff ("星星:显示阈值", Range(0.01, 1)) = 0.08
        _StarsSpeed ("星星:移动速度", Range(0, 0.5)) = 0.1
        _StarsBrightness ("星星:亮度", Range(0, 10)) = 3.0
        _StarsFadeEdge ("星星:边缘柔和度", Range(0, 2)) = 0.6
        _StarsDensity ("星星:密度", Range(0, 1)) = 0.8
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Background" 
            "Queue" = "Background"
            "PreviewType" = "Skybox"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100
        
        Pass
        {
            Name "Skybox"
            ZWrite Off
            Cull Off
            Blend Off
            ColorMask RGB
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local _CLOUD_ON
            // #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            TEXTURE2D(_Stars); SAMPLER(sampler_Stars);
            TEXTURE2D(_CloudNoiseTexture); SAMPLER(sampler_CloudNoiseTexture);

            CBUFFER_START(UnityPerMaterial)
                float _Exposure;
                
                float4 _NightTopColor;
                float4 _NightBottomColor;
                float4 _HorizonColorNight;
                float4 _NightFogColor;
                float _OffsetHorizon;
                float _HorizonSmoothness;
                float _HorizonGlowRange;
                float _HorizonGlowIntensity;
                float _HorizonBlendFactor;
                
                // 星星参数
                float _StarsCutoff;
                float _StarsSpeed;
                float _StarsBrightness;
                float _StarsFadeEdge;
                float _StarsDensity;

                // 云朵参数
                float _CloudAlpha;
                float _CloudPower;
                float _CloudAmount;
                float _CloudCoverage;
                float _CloudHeight;
                float _CloudSpeed;
                float _CloudDensity;
                float _CloudSmallDetails;
                float _CloudDirection;
                float _CloudFadePosition;
                float _CloudFadeAmount;
                float _CloudTextureTiling;
                float _CloudDetailTiling;
                half4 _CloudColor1;
                half4 _CloudColor2;
            CBUFFER_END
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 texcoord : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 texcoord : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                #ifdef _CLOUD_ON
                    float4 cloudUVs : TEXCOORD2;
                    float4 detailUVs : TEXCOORD3;
                #endif
                float3 smoothVertex : TEXCOORD4;
            };

            #define _CLOUD_HEIGHT_LIMITS float2(30, 100)

            inline float2 Rotate2d(float2 p, float angle) {
                return mul(float2x2(cos(angle), -sin(angle), sin(angle), cos(angle)), p);
            }

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.texcoord = v.texcoord;
                o.worldPos = TransformObjectToWorld(v.positionOS.xyz);
                o.smoothVertex = v.positionOS.xyz;
                
                #ifdef _CLOUD_ON
                    // 云朵UV计算 - 增加多层UV
                    float3 cloudWorldVertex = normalize(mul((float3x3)unity_ObjectToWorld, v.positionOS.xyz));

                    float computedHeight = lerp(_CLOUD_HEIGHT_LIMITS.x, _CLOUD_HEIGHT_LIMITS.y, 1 - _CloudHeight);
                    cloudWorldVertex.y *= computedHeight * .2f;

                    float cloudSpeed = _CloudSpeed * _Time.y;
                    cloudWorldVertex.xz = Rotate2d(cloudWorldVertex.xz, _CloudDirection);
                    cloudWorldVertex = normalize(cloudWorldVertex);
                    
                    // 基础云层UV
                    o.cloudUVs.xy = (cloudWorldVertex.xz * _CloudTextureTiling) + float2(cloudSpeed, cloudSpeed);
                    o.cloudUVs.zw = (cloudWorldVertex.xz * _CloudTextureTiling * 0.8) + float2(cloudSpeed * 0.7, cloudSpeed * 0.9);
                    
                    // 细节云层UV - 增加更多云朵细节
                    o.detailUVs.xy = (cloudWorldVertex.xz * _CloudDetailTiling) + float2(cloudSpeed * 1.3, cloudSpeed * 1.1);
                    o.detailUVs.zw = (cloudWorldVertex.xz * _CloudDetailTiling * 1.5) + float2(cloudSpeed * 0.9, cloudSpeed * 1.4);
                #endif

                return o;
            }

            // 改进的云朵噪声函数
            float CloudNoise(float2 uv, float timeOffset)
            {
                float4 tex1 = SAMPLE_TEXTURE2D(_CloudNoiseTexture, sampler_CloudNoiseTexture, uv + timeOffset);
                float4 tex2 = SAMPLE_TEXTURE2D(_CloudNoiseTexture, sampler_CloudNoiseTexture, uv * 1.7 - timeOffset * 0.5);
                
                float noise1 = tex1.r * 0.6 + tex1.g * 0.3 + tex1.b * 0.1;
                float noise2 = tex2.r * 0.4 + tex2.g * 0.4 + tex2.b * 0.2;
                
                return (noise1 * 0.7 + noise2 * 0.3);
            }

            half4 frag(Varyings i) : SV_Target
            {
                // 基础地平线计算 - 使用更平滑的过渡
                float horizonBase = saturate(i.texcoord.y - _OffsetHorizon);
                
                // 改进的地平线过渡 - 使用smoothstep获得更柔和的边缘
                float horizonTransition = smoothstep(0.0, _HorizonSmoothness, horizonBase);
                
                // 改进的地平线发光效果 - 使用更平滑的衰减曲线
                float horizonGlow = exp(-pow(abs(horizonBase) * _HorizonGlowRange, 2));
                horizonGlow *= _HorizonGlowIntensity;
                
                // 天空UV计算
                float2 skyUV = i.texcoord.xz / (i.texcoord.y + 0.001); // 避免除以零

                // 星星效果
                float3 stars = SAMPLE_TEXTURE2D(_Stars, sampler_Stars, skyUV + (_StarsSpeed * _Time.y)).rgb;
                // 星星密度控制
                stars *= _StarsDensity;
                // 星星柔和边缘与亮度
                stars = smoothstep(_StarsCutoff, _StarsCutoff + _StarsFadeEdge, stars);
                stars *= _StarsBrightness;
                // 改进的星星分布 - 使用更平滑的衰减曲线
                float starFade = smoothstep(0.0, 1.0, horizonBase);
                stars *= pow(starFade, 0.5);

                // 改进的夜晚天空渐变 - 使用更平滑的插值
                float3 nightGradient = lerp(_NightBottomColor.rgb, _NightTopColor.rgb, horizonTransition);
                
                // 改进的地平线颜色混合 - 使用分层混合
                float3 horizonColor = _HorizonColorNight.rgb * horizonGlow;
                
                // 改进的低空夜雾效果 - 使用更自然的衰减
                float fogFactor = 1.0 - smoothstep(0.1, 0.6, horizonBase);
                float3 nightFog = _NightFogColor.rgb * fogFactor * 0.5;

                // 改进的颜色混合系统
                float3 baseSky = lerp(nightGradient, horizonColor, _HorizonBlendFactor * horizonGlow);
                baseSky = lerp(baseSky, nightFog, fogFactor * 0.3);
                
                // 组合基础夜空颜色
                float3 final_color = baseSky + stars;
                
                // 增强的夜晚云朵系统
                #ifdef _CLOUD_ON
                    // 多层云朵噪声
                    float timeOffset = _Time.y * _CloudSpeed;
                    
                    // 基础云层
                    float baseCloud1 = CloudNoise(i.cloudUVs.xy, timeOffset);
                    float baseCloud2 = CloudNoise(i.cloudUVs.zw, timeOffset * 0.8);
                    float baseCloud = (baseCloud1 * 0.6 + baseCloud2 * 0.4) * _CloudAmount;
                    
                    // 细节云层 - 增加更多云朵变化
                    float detailCloud1 = CloudNoise(i.detailUVs.xy, timeOffset * 1.2);
                    float detailCloud2 = CloudNoise(i.detailUVs.zw, timeOffset * 0.6);
                    float detailCloud = (detailCloud1 * 0.5 + detailCloud2 * 0.5) * _CloudSmallDetails;
                    
                    // 合并云层
                    float combinedCloud = saturate(baseCloud + detailCloud * 0.7);
                    
                    // 应用云朵覆盖率控制
                    combinedCloud = smoothstep(1.0 - _CloudCoverage, 1.0, combinedCloud);
                    
                    // 云朵淡出效果
                    float fadeOutPercent = smoothstep(_CloudFadePosition, 1, length(i.smoothVertex.xz));
                    combinedCloud *= 1.0 - fadeOutPercent * _CloudFadeAmount;
                    
                    // 应用密度控制 - 反转逻辑使数值越大云越多
                    float densityFactor = pow(abs(combinedCloud), 1.0 / max(_CloudDensity, 0.1));
                    
                    // 夜晚云朵颜色 - 使用更丰富的颜色混合
                    float3 cloudColor = lerp(_CloudColor2.rgb, _CloudColor1.rgb, densityFactor);
                    cloudColor *= _CloudPower * 3.0;
                    
                    // 云朵透明度控制
                    float cloudAlpha = densityFactor * _CloudAlpha;
                    cloudAlpha *= step(0.0f, i.smoothVertex.y); // 只显示上半球
                    
                    // 改进的云朵与夜空融合
                    final_color = lerp(final_color.rgb, cloudColor, cloudAlpha * (1.0 - horizonGlow * 0.2));
                #endif
                
                // 统一使用曝光度控制整体亮度
                final_color *= _Exposure;
                
                return half4(final_color.rgb, 1.0);
            }
            ENDHLSL
        }
    }
//    CustomEditor "EffectSkyShaderEditor"
}