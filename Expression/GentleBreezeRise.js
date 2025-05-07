// 기본 설정
var start = 0; // 애니메이션 시작 시간
var end = 10; // 애니메이션 종료 시간 (올라가는 시간)
var startY = value[1]; // 시작 Y 위치
var endY = startY - 300; // 종료 Y 위치 (위로 300픽셀 이동)
var startAmplitude = 30; // 시작 진폭
var endAmplitude = 5; // 종료 진폭
var frequency = 2; // 좌우 움직임의 빈도

// 시간에 따른 선형 보간 계산 (Y축)
var y = linear(time, start, end, startY, endY);

// 시간에 따라 진폭 감소 (X축)
var currentAmplitude = linear(time, start, end, startAmplitude, endAmplitude);
var x = value[0] + currentAmplitude * Math.sin(frequency * time * 2 * Math.PI);

[x, y]
