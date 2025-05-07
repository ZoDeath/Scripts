// RHV/fx-UITrail-Alpha.shader
Shader "RHV/fx-UITrail-Alpha"
{
    Properties
    {
        // 기본 유니티 텍스처 슬롯 (자동 할당됨. 실 사용 안함)
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        // 머티리얼 전반에 적용할 색상 틴트
        _Color ("Tint", Color) = (1,1,1,1)

        // 최종 출력 색상에 곱해질 색상 값 (R,G,B)
        _ColorOverlay("Color Overlay", Color) = (1,1,1,1)

        // 전체 투명도에 영향을 주는 강도 값
        _GlobalAlphaIntensity("Global Alpha Intensity", Range(0, 5)) = 2

        // 투명도에 곡률을 적용하는 지수 값
        _GlobalAlphaFalloff("Global Alpha Falloff", Range(1, 5)) = 1

        // 트레일의 길이 느낌을 제어하는 감쇠 값
        _TrailLengthFalloff("Trail Length Falloff", Range(1, 10)) = 1.5

        // 회전 애니메이션 속도
        _TimeSpeed("Time Speed", Range(0, 5)) = 1

        // 회전 트레일 개수 설정 (최대 4개)
        [IntRange]_TrailNumber("Trail Number", Range(1, 4)) = 2

        // 트레일의 알파 강도를 위한 기준 텍스처
        _BaseTexture("Base Texture", 2D) = "white" {}

        // 회전되는 트레일 텍스처
        _RotatingTexture("Rotating Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha // 일반적인 투명도 블렌딩 방식
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

        // 변수 선언
        fixed4 _Color;
        fixed4 _ColorOverlay;
        sampler2D _BaseTexture;
        sampler2D _RotatingTexture;
        float _TimeSpeed;
        float _GlobalAlphaIntensity;
        float _GlobalAlphaFalloff;
        float _TrailLengthFalloff;
        float _TrailNumber;

        // 버텍스 셰이더: 화면 위치와 UV 전달
        v2f vert (appdata_t v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.texcoord = v.texcoord;
            return o;
        }

        // 프래그먼트 셰이더: 픽셀 단위의 색상 계산
        fixed4 frag (v2f i) : SV_Target
        {
            float2 uv = i.texcoord;
            float4 baseColor = tex2D(_BaseTexture, uv);

            // 시간 기반 회전각 계산
            float t = _Time.y * (_TimeSpeed * 3.0);

            // 중심을 기준으로 회전되도록 좌표 이동
            float2 p = uv - 0.5;

            // 여러 각도에서 회전된 텍스처 알파값 샘플링
            float a1 = tex2D(_RotatingTexture, mul(p, float2x2(cos(t), -sin(t), sin(t), cos(t))) + 0.5).r;
            float a2 = tex2D(_RotatingTexture, mul(p, float2x2(cos(t + 3.15), -sin(t + 3.15), sin(t + 3.15), cos(t + 3.15))) + 0.5).r;
            float a3 = tex2D(_RotatingTexture, mul(p, float2x2(cos(t + 1.575), -sin(t + 1.575), sin(t + 1.575), cos(t + 1.575))) + 0.5).r;
            float a4 = tex2D(_RotatingTexture, mul(p, float2x2(cos(t + 4.725), -sin(t + 4.725), sin(t + 4.725), cos(t + 4.725))) + 0.5).r;

            // 트레일 개수에 맞게 밝기 합산
            float trail = (_TrailNumber >= 4.0 ? a1 + a2 + a3 + a4 : _TrailNumber == 3.0 ? a1 + a2 + a3 : _TrailNumber == 2.0 ? a1 + a2 : a1);

            // 트레일 알파값 부드럽게 곡선 처리
            float trailAlpha = clamp(pow(trail, 1.1 * _TrailLengthFalloff), 0.0, 1.0);

            // 최종 알파값 계산 (트레일 × 밝기 × 강도)
            float alpha = clamp(pow(((baseColor.r * trailAlpha) * 1.5) * _GlobalAlphaIntensity, _GlobalAlphaFalloff), 0.0, 1.0);

            // 결과 출력: 색은 Overlay, 알파는 계산값
            return fixed4(_ColorOverlay.rgb, alpha);
        }
        ENDCG }
    }
}
