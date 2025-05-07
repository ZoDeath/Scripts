// RHV/fx-UITrail-Multiply.shader
Shader "RHV/fx-UITrail-Multiply"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {} // 유니티 내부에서 사용. 실제론 사용 안함
        _Color ("Tint", Color) = (1,1,1,1) // 전반적인 색상 틴트
        _ColorOverlay("Color Overlay", Color) = (1,1,1,1) // 곱해질 색상 값 (출력 색상)
        _GlobalAlphaIntensity("Global Alpha Intensity", Range(0, 5)) = 2 // 전체 알파 밝기 강도
        _GlobalAlphaFalloff("Global Alpha Falloff", Range(1, 5)) = 1 // 알파 조절 곡선 정도
        _TrailLengthFalloff("Trail Length Falloff", Range(1, 10)) = 1.5 // 트레일 길이 강도
        _TimeSpeed("Time Speed", Range(0, 5)) = 1 // 회전 속도 계수
        [IntRange]_TrailNumber("Trail Number", Range(1, 4)) = 2 // 트레일 수 (1~4)
        _BaseTexture("Base Texture", 2D) = "white" {} // 트레일 알파 기준 밝기 텍스처
        _RotatingTexture("Rotating Texture", 2D) = "white" {} // 회전 트레일 텍스처
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend DstColor Zero // Multiply 블렌딩 설정
        Cull Off ZWrite Off ZTest LEqual

        Pass {
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        struct appdata_t {
            float4 vertex : POSITION;
            float4 color : COLOR;
            float2 texcoord : TEXCOORD0;
        };

        struct v2f {
            float4 vertex : SV_POSITION;
            float2 texcoord : TEXCOORD0;
        };

        fixed4 _Color;
        fixed4 _ColorOverlay;
        sampler2D _BaseTexture;
        sampler2D _RotatingTexture;
        float _TimeSpeed;
        float _GlobalAlphaIntensity;
        float _GlobalAlphaFalloff;
        float _TrailLengthFalloff;
        float _TrailNumber;

        v2f vert (appdata_t v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.texcoord = v.texcoord;
            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            float2 uv = i.texcoord;
            float4 baseColor = tex2D(_BaseTexture, uv); // 밝기 기준 텍스처 샘플

            float t = _Time.y * (_TimeSpeed * 3.0); // 회전용 시간값 계산
            float2 p = uv - 0.5; // 중심 기준 회전용 위치 조정

            float a1 = tex2D(_RotatingTexture, mul(p, float2x2(cos(t), -sin(t), sin(t), cos(t))) + 0.5).r;
            float a2 = tex2D(_RotatingTexture, mul(p, float2x2(cos(t + 3.15), -sin(t + 3.15), sin(t + 3.15), cos(t + 3.15))) + 0.5).r;
            float a3 = tex2D(_RotatingTexture, mul(p, float2x2(cos(t + 1.575), -sin(t + 1.575), sin(t + 1.575), cos(t + 1.575))) + 0.5).r;
            float a4 = tex2D(_RotatingTexture, mul(p, float2x2(cos(t + 4.725), -sin(t + 4.725), sin(t + 4.725), cos(t + 4.725))) + 0.5).r;

            float trail = (_TrailNumber >= 4.0 ? a1 + a2 + a3 + a4 : _TrailNumber == 3.0 ? a1 + a2 + a3 : _TrailNumber == 2.0 ? a1 + a2 : a1);
            float trailAlpha = clamp(pow(trail, 1.1 * _TrailLengthFalloff), 0.0, 1.0);

            float alpha = clamp(pow(((baseColor.r * trailAlpha) * 1.5) * _GlobalAlphaIntensity, _GlobalAlphaFalloff), 0.0, 1.0);

            return fixed4(_ColorOverlay.rgb * baseColor.rgb, alpha); // 곱셈 결과 반환
        }
        ENDCG }
    }
}
