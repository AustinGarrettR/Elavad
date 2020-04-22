// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Skybox/Blended Cubemap" {
    Properties{
        _Tint("Tint Color", Color) = (.5, .5, .5, .5)
        [Gamma] _Exposure("Exposure", Range(0, 8)) = 1.0
        _Rotation("Rotation", Range(0, 360)) = 0
        _Opacity1("Opacity 1", Range(0,1)) = 0.0
        _Opacity2("Opacity 2", Range(0,1)) = 0.0
        _Opacity3("Opacity 3", Range(0,1)) = 0.0
        _Opacity4("Opacity 4", Range(0,1)) = 0.0
        [NoScaleOffset] _Tex1("Cubemap 1 (HDR)", Cube) = "grey" {}
        [NoScaleOffset] _Tex2("Cubemap 2 (HDR)", Cube) = "grey" {}
        [NoScaleOffset] _Tex3("Cubemap 3 (HDR)", Cube) = "grey" {}
        [NoScaleOffset] _Tex4("Cubemap 4 (HDR)", Cube) = "grey" {}
    }

        SubShader{
            Tags { "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
            Cull Off ZWrite Off

            Pass {

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0

                #include "UnityCG.cginc"

                samplerCUBE _Tex1;
                samplerCUBE _Tex2;
                samplerCUBE _Tex3;
                samplerCUBE _Tex4;
                half4 _Tex1_HDR;
                half4 _Tex2_HDR;
                half4 _Tex3_HDR;
                half4 _Tex4_HDR;
                half4 _Tint;
                half _Exposure;
                half _Opacity1;
                half _Opacity2;
                half _Opacity3;
                half _Opacity4;
                float _Rotation;

                float3 RotateAroundYInDegrees(float3 vertex, float degrees)
                {
                    float alpha = degrees * UNITY_PI / 180.0;
                    float sina, cosa;
                    sincos(alpha, sina, cosa);
                    float2x2 m = float2x2(cosa, -sina, sina, cosa);
                    return float3(mul(m, vertex.xz), vertex.y).xzy;
                }

                struct appdata_t {
                    float4 vertex : POSITION;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f {
                    float4 vertex : SV_POSITION;
                    float3 texcoord : TEXCOORD0;
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                v2f vert(appdata_t v)
                {
                    v2f o;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                    float3 rotated = RotateAroundYInDegrees(v.vertex, _Rotation);
                    o.vertex = UnityObjectToClipPos(rotated);
                    o.texcoord = v.vertex.xyz;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    half4 tex1 = texCUBE(_Tex1, i.texcoord);
                    half4 tex2 = texCUBE(_Tex2, i.texcoord);
                    half4 tex3 = texCUBE(_Tex3, i.texcoord);
                    half4 tex4 = texCUBE(_Tex4, i.texcoord);
                    half3 c = (DecodeHDR(tex1, _Tex1_HDR) * _Opacity1) + (DecodeHDR(tex2, _Tex2_HDR) * _Opacity2) + (DecodeHDR(tex3, _Tex3_HDR) * _Opacity3) + (DecodeHDR(tex4, _Tex4_HDR) * _Opacity4);
                    c = c * _Tint.rgb * unity_ColorSpaceDouble.rgb;
                    c *= _Exposure;
                    return half4(c, 1);
                }
                ENDCG
            }
    }


        Fallback "Skybox/Cubed"

}