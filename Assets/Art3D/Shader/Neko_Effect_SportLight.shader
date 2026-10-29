Shader "Neko/Effect/SportLight"
{
    Properties
    {
        [HDR]_Color("Color", Color) = (1, 1, 1, 0)
        _Opacity("Opacity", Float) = 1
        _EdgeFallof("EdgeFallof", Float) = 0.3
        _Vector2("Vector2", Vector) = (0, 1, 0, 0)
        // 【新增】新增软相交强度与噪声对比度微调属性，方便在 Inspector 中直观调参
        _Softness("Softness", Float) = 1.0
        _NoiseContrast("Noise Contrast", Float) = 1.5
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
            "DisableBatching"="False"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalUnlitSubTarget"
        }
        Pass
        {
            Name "Universal Forward"
            Tags {}
            // Render State
            Cull Back
            // 【优化】改为加法混合（Additive），让光柱拥有真实的光晕叠加与通透感
            Blend SrcAlpha One 
            ZTest LEqual
            ZWrite Off

            HLSLPROGRAM
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #pragma instancing_options renderinglayer
            #pragma vertex vert
            #pragma fragment frag
            
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TEXCOORD0
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_UNLIT
            #define _FOG_FRAGMENT 1
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define REQUIRE_DEPTH_TEXTURE

            // Includes
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv0 : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS;
                float3 normalWS;
                float4 texCoord0;
            };

            struct SurfaceDescriptionInputs
            {
                float3 WorldSpaceNormal;
                float3 WorldSpaceViewDirection;
                float3 WorldSpacePosition;
                float4 ScreenPosition;
                float2 NDCPosition;
                float2 PixelPosition;
                float4 uv0;
                float3 TimeParameters;
            };

            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
            };

            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                float4 texCoord0 : INTERP0;
                float3 positionWS : INTERP1;
                float3 normalWS : INTERP2;
            };

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                output.positionCS = input.positionCS;
                output.texCoord0.xyzw = input.texCoord0;
                output.positionWS.xyz = input.positionWS;
                output.normalWS.xyz = input.normalWS;
                return output;
            }

            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                output.texCoord0.xyzw = input.texCoord0.xyzw;
                output.positionWS = input.positionWS.xyz;
                output.normalWS = input.normalWS.xyz;
                return output;
            }

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _Opacity;
                float _EdgeFallof;
                float2 _Vector2;
                float _Softness;
                float _NoiseContrast;
            CBUFFER_END
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"

            void Unity_Distance_float3(float3 A, float3 B, out float Out)
            {
                Out = distance(A, B);
            }

            void Unity_Subtract_float(float A, float B, out float Out)
            {
                Out = A - B;
            }

            void Unity_Clamp_float(float In, float Min, float Max, out float Out)
            {
                Out = clamp(In, Min, Max);
            }

            void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
            {
                Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
            }

            void Unity_Lerp_float(float A, float B, float T, out float Out)
            {
                Out = lerp(A, B, T);
            }

            void Unity_SceneDepth_Linear01_float(float4 UV, out float Out)
            {
                Out = Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
            }

            void Unity_Multiply_float_float(float A, float B, out float Out)
            {
                Out = A * B;
            }

            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
            {
                Out = UV * Tiling + Offset;
            }

            float Unity_SimpleNoise_ValueNoise_Deterministic_float(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                f = f * f * (3.0 - 2.0 * f);
                uv = abs(frac(uv) - 0.5);
                float2 c0 = i + float2(0.0, 0.0);
                float2 c1 = i + float2(1.0, 0.0);
                float2 c2 = i + float2(0.0, 1.0);
                float2 c3 = i + float2(1.0, 1.0);
                float r0;
                Hash_Tchou_2_1_float(c0, r0);
                float r1;
                Hash_Tchou_2_1_float(c1, r1);
                float r2;
                Hash_Tchou_2_1_float(c2, r2);
                float r3;
                Hash_Tchou_2_1_float(c3, r3);
                float bottomOfGrid = lerp(r0, r1, f.x);
                float topOfGrid = lerp(r2, r3, f.x);
                float t = lerp(bottomOfGrid, topOfGrid, f.y);
                return t;
            }

            void Unity_SimpleNoise_Deterministic_float(float2 UV, float Scale, out float Out)
            {
                float freq, amp;
                Out = 0.0f;
                freq = pow(2.0, float(0));
                amp = pow(0.5, float(3 - 0));
                Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy * (Scale / freq))) * amp;
                freq = pow(2.0, float(1));
                amp = pow(0.5, float(3 - 1));
                Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy * (Scale / freq))) * amp;
                freq = pow(2.0, float(2));
                amp = pow(0.5, float(3 - 2));
                Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy * (Scale / freq))) * amp;
            }

            void Unity_Add_float(float A, float B, out float Out)
            {
                Out = A + B;
            }

            void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
            {
                Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
            }

            void Unity_Saturate_float(float In, out float Out)
            {
                Out = saturate(In);
            }

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }
            
            // Graph Pixel
            struct SurfaceDescription
            {
                float3 BaseColor;
                float Alpha;
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                
                float4 _Property_fc6c6b834e1e43db82d9515243bcb0e9_Out_0_Vector4 = IsGammaSpace()
                         ? LinearToSRGB(_Color)
                         : _Color;

                float4 _UV_661cb4211d6842fa868b104da85cd808_Out_0_Vector4 = IN.uv0;
                float _Split_f68ecc4d570a474589c9047e37534cf4_G_2_Float =
                    _UV_661cb4211d6842fa868b104da85cd808_Out_0_Vector4[1];

                // 距离裁剪（相机淡出）
                float _Distance_022e43c929ec4125aefd1657c4174c8e_Out_2_Float;
                Unity_Distance_float3(_WorldSpaceCameraPos, IN.WorldSpacePosition,
                    _Distance_022e43c929ec4125aefd1657c4174c8e_Out_2_Float);
                float _Subtract_be93c03d404342319821616549079732_Out_2_Float;
                Unity_Subtract_float(_Distance_022e43c929ec4125aefd1657c4174c8e_Out_2_Float, float(0.5),
                    _Subtract_be93c03d404342319821616549079732_Out_2_Float);
                float _Clamp_fc8a414e9671483891a59f52f272408e_Out_3_Float;
                Unity_Clamp_float(_Subtract_be93c03d404342319821616549079732_Out_2_Float, float(0), float(1),
                    _Clamp_fc8a414e9671483891a59f52f272408e_Out_3_Float);

                float _Property_5bc0d9d046d043f7a888b27834df8d14_Out_0_Float = _Opacity;
                float _Property_91c9aef7364245f3b2f28655d2afffa5_Out_0_Float = _EdgeFallof;
                
                // 菲涅尔边缘衰减
                float _FresnelEffect_300b3d6fb548461681c0c383a1173ff8_Out_3_Float;
                Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection,
                    _Property_91c9aef7364245f3b2f28655d2afffa5_Out_0_Float,
                    _FresnelEffect_300b3d6fb548461681c0c383a1173ff8_Out_3_Float);
                float _Clamp_76ef421906424b40ac7019ecb0688ad8_Out_3_Float;
                Unity_Clamp_float(_FresnelEffect_300b3d6fb548461681c0c383a1173ff8_Out_3_Float, float(0), float(1),
                                                  _Clamp_76ef421906424b40ac7019ecb0688ad8_Out_3_Float);
                float _Lerp_cc2d2601e3c34b388803f2dc80c33d8a_Out_3_Float;
                Unity_Lerp_float(_Property_5bc0d9d046d043f7a888b27834df8d14_Out_0_Float, float(0),
                                                                    _Clamp_76ef421906424b40ac7019ecb0688ad8_Out_3_Float,
                                                                    _Lerp_cc2d2601e3c34b388803f2dc80c33d8a_Out_3_Float);

                // 深度软相交（Soft Particles）
                float _SceneDepth_18d9c562c18c4a75a8f41d049504d06e_Out_1_Float;
                Unity_SceneDepth_Linear01_float(float4(IN.NDCPosition.xy, 0, 0),
                         _SceneDepth_18d9c562c18c4a75a8f41d049504d06e_Out_1_Float);
                float _Multiply_0a341d7c325b436daf2c20f6f5aacbfb_Out_2_Float;
                Unity_Multiply_float_float(_SceneDepth_18d9c562c18c4a75a8f41d049504d06e_Out_1_Float,
                             _ProjectionParams.z, _Multiply_0a341d7c325b436daf2c20f6f5aacbfb_Out_2_Float);
                float4 _ScreenPosition_d0df452efcb94bfcb15b834022937427_Out_0_Vector4 = IN.ScreenPosition;
                float _Split_fc67024b78d9463aa6dfee1b405c1abb_A_4_Float =
                    _ScreenPosition_d0df452efcb94bfcb15b834022937427_Out_0_Vector4[3];
                float _Subtract_c547ff2e19ce4466bd11e8188b305ffb_Out_2_Float;
                Unity_Subtract_float(_Multiply_0a341d7c325b436daf2c20f6f5aacbfb_Out_2_Float,
                      _Split_fc67024b78d9463aa6dfee1b405c1abb_A_4_Float,
                      _Subtract_c547ff2e19ce4466bd11e8188b305ffb_Out_2_Float);
                
                // 【优化】结合可调节的 _Softness 属性，让交界处更加自然细腻
                float softMul = max(_Softness, 0.001);
                float _Clamp_c9449a66ffa54b6dae3c5bcf0ac826f3_Out_3_Float;
                Unity_Clamp_float(_Subtract_c547ff2e19ce4466bd11e8188b305ffb_Out_2_Float * softMul, float(0), float(1),
                                                                  _Clamp_c9449a66ffa54b6dae3c5bcf0ac826f3_Out_3_Float);

                float _Multiply_cb13e704af9149798d3088905126f777_Out_2_Float;
                Unity_Multiply_float_float(_Lerp_cc2d2601e3c34b388803f2dc80c33d8a_Out_3_Float,
                    _Clamp_c9449a66ffa54b6dae3c5bcf0ac826f3_Out_3_Float,
                    _Multiply_cb13e704af9149798d3088905126f777_Out_2_Float);
                float _Multiply_1b455a978f4545e0b82b26fcc39f6671_Out_2_Float;
                Unity_Multiply_float_float(_Clamp_fc8a414e9671483891a59f52f272408e_Out_3_Float,
                                         _Multiply_cb13e704af9149798d3088905126f777_Out_2_Float,
                                         _Multiply_1b455a978f4545e0b82b26fcc39f6671_Out_2_Float);
                float _Multiply_9eef201ab8ab4c16b16b9955cbc2b1b5_Out_2_Float;
                Unity_Multiply_float_float(_Split_f68ecc4d570a474589c9047e37534cf4_G_2_Float,
                                                                            _Multiply_1b455a978f4545e0b82b26fcc39f6671_Out_2_Float,
                                                                            _Multiply_9eef201ab8ab4c16b16b9955cbc2b1b5_Out_2_Float);

                // 双层噪声流动（模拟空气微尘）
                float _Multiply_b5adf4d30d2942b6945a8e29624d6cc5_Out_2_Float;
                Unity_Multiply_float_float(IN.TimeParameters.x, 0.02,
                                      _Multiply_b5adf4d30d2942b6945a8e29624d6cc5_Out_2_Float);
                float2 _TilingAndOffset_ac99ae1bf1be4f5a932271796f404029_Out_3_Vector2;
                Unity_TilingAndOffset_float(IN.uv0.xy, float2(1, 0.02),
                    (_Multiply_b5adf4d30d2942b6945a8e29624d6cc5_Out_2_Float.xx),
                    _TilingAndOffset_ac99ae1bf1be4f5a932271796f404029_Out_3_Vector2);
                float _SimpleNoise_6d9fa27ee5e148618cd88f67ec0178cf_Out_2_Float;
                Unity_SimpleNoise_Deterministic_float(_TilingAndOffset_ac99ae1bf1be4f5a932271796f404029_Out_3_Vector2,
                                          float(75), _SimpleNoise_6d9fa27ee5e148618cd88f67ec0178cf_Out_2_Float);

                float _Multiply_1d94eb2bbd634e04b0ff7d02288ce7ff_Out_2_Float;
                Unity_Multiply_float_float(IN.TimeParameters.x, -0.03,
                    _Multiply_1d94eb2bbd634e04b0ff7d02288ce7ff_Out_2_Float);
                float2 _TilingAndOffset_e52313e831d14c128bb5ee4d2c8edbe2_Out_3_Vector2;
                Unity_TilingAndOffset_float(IN.uv0.xy, float2(1, -0.04),
                    (_Multiply_1d94eb2bbd634e04b0ff7d02288ce7ff_Out_2_Float.xx),
                    _TilingAndOffset_e52313e831d14c128bb5ee4d2c8edbe2_Out_3_Vector2);
                float _SimpleNoise_fe917c8d71234a3e8d011fb4c51ff674_Out_2_Float;
                Unity_SimpleNoise_Deterministic_float(_TilingAndOffset_e52313e831d14c128bb5ee4d2c8edbe2_Out_3_Vector2,
                                                                   float(32.3),
                                                                   _SimpleNoise_fe917c8d71234a3e8d011fb4c51ff674_Out_2_Float);

                float _Add_86797691ff0c465692cb70c92f1df944_Out_2_Float;
                Unity_Add_float(_SimpleNoise_6d9fa27ee5e148618cd88f67ec0178cf_Out_2_Float,
                                _SimpleNoise_fe917c8d71234a3e8d011fb4c51ff674_Out_2_Float,
                                _Add_86797691ff0c465692cb70c92f1df944_Out_2_Float);

                // 【优化】通过对噪声结果应用幂运算（Contrast），让光柱内部的明暗层次更清晰、不灰暗
                float enhancedNoise = pow(saturate(_Add_86797691ff0c465692cb70c92f1df944_Out_2_Float * 0.5), _NoiseContrast);

                float _Multiply_31f93a16269446a59d34005a11caedb9_Out_2_Float;
                Unity_Multiply_float_float(_Multiply_9eef201ab8ab4c16b16b9955cbc2b1b5_Out_2_Float,
                                           enhancedNoise, _Multiply_31f93a16269446a59d34005a11caedb9_Out_2_Float);

                float4 _UV_156f03814eb842debdfb4c62b81d00ea_Out_0_Vector4 = IN.uv0;
                float _Split_ae0da52ff0a7452dad8e24687629b9ab_G_2_Float =
                    _UV_156f03814eb842debdfb4c62b81d00ea_Out_0_Vector4[1];
                float2 _Property_dfa4e3547a3946889c4d90232ecf9c27_Out_0_Vector2 = _Vector2;
                
                float _Remap_6115831c9c4040bb85add0128227fd29_Out_3_Float;
                Unity_Remap_float(_Split_ae0da52ff0a7452dad8e24687629b9ab_G_2_Float,
                          _Property_dfa4e3547a3946889c4d90232ecf9c27_Out_0_Vector2,
                          float2(-1, 1),
                          _Remap_6115831c9c4040bb85add0128227fd29_Out_3_Float);
                float _Saturate_7168b7a18e5641f09e032916661c0304_Out_1_Float;
                Unity_Saturate_float(_Remap_6115831c9c4040bb85add0128227fd29_Out_3_Float,
                    _Saturate_7168b7a18e5641f09e032916661c0304_Out_1_Float);

                float _Multiply_9bbb42bf778b420496d20b37afa53b65_Out_2_Float;
                Unity_Multiply_float_float(_Multiply_31f93a16269446a59d34005a11caedb9_Out_2_Float,
                         _Saturate_7168b7a18e5641f09e032916661c0304_Out_1_Float,
                         _Multiply_9bbb42bf778b420496d20b37afa53b65_Out_2_Float);

                surface.BaseColor = (_Property_fc6c6b834e1e43db82d9515243bcb0e9_Out_0_Vector4.xyz);
                surface.Alpha = _Multiply_9bbb42bf778b420496d20b37afa53b65_Out_2_Float;
                return surface;
            }

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                float3 unnormalizedNormalWS = input.normalWS;
                const float renormFactor = 1.0 / length(unnormalizedNormalWS);
                
                output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;
                output.WorldSpaceViewDirection = GetWorldSpaceNormalizeViewDir(input.positionWS);
                output.WorldSpacePosition = input.positionWS;
                output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);

                #if UNITY_UV_STARTS_AT_TOP
                output.PixelPosition = float2(input.positionCS.x,
                                              (_ProjectionParams.x < 0)
                                                  ? (_ScaledScreenParams.y - input.positionCS.y)
                                                  : input.positionCS.y);
                #else
                output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
                #endif

                output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
                output.NDCPosition.y = 1.0f - output.NDCPosition.y;

                output.uv0 = input.texCoord0;
                output.TimeParameters = _TimeParameters.xyz; 

                return output;
            }

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"

            ENDHLSL
        }
    }
}