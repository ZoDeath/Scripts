// 객체에 움직임을 부여하고 이 움직임을 특정 시간 동안 반복하는 기능을 수행
// 반복적인 루프 실행시 실행되는 프레임 수를 설정함

freq = 1; // 'freq'는 wiggle 함수의 빈도를 설정합니다. 여기서 1은 초당 한 번의 변동을 의미함.
amp = 10; // 'amp'는 wiggle 함수의 진폭을 설정합니다. 이 값은 변동의 최대 크기를 결정함.

frameCount = 120; // 'frameCount'는 루프가 실행될 전체 프레임 수를 설정함.
fps = 30; // 'fps'는 프레임 속도를 나타냅니다. 여기서는 30fps를 의미함.

// 'loopTime'은 프레임 수를 초 단위로 변환하여 루프의 총 시간을 계산함.
loopTime = frameCount / fps; 

// 't'는 현재 시간을 'loopTime'으로 나눈 나머지를 계산합니다. 이는 움직임을 루프하기 위한 시간 계산.
t = time % loopTime;

// 'wiggle1'은 현재 시간 't'에서의 wiggle 함수 값을 계산함.
wiggle1 = wiggle(freq, amp, 1, 0.5, t);

// 'wiggle2'는 이전 루프 주기에서의 wiggle 함수 값을 계산함.
wiggle2 = wiggle(freq, amp, 1, 0.5, t - loopTime);

// 'linear' 함수는 시간 't'에 따라 'wiggle1'과 'wiggle2' 사이를 부드럽게 전환함.
linear(t, 0, loopTime, wiggle1, wiggle2);


freq = 1;
amp = 10;
frameCount = 120;
fps = 30;
loopTime = frameCount / fps; 
t = time % loopTime;
wiggle1 = wiggle(freq, amp, 1, 0.5, t);
wiggle2 = wiggle(freq, amp, 1, 0.5, t - loopTime);
linear(t, 0, loopTime, wiggle1, wiggle2);

///////////////////////////////////////////////////////////////////////////////////////////////

//좌우 움직임
freq = 2;
amp = 70;
loopTime = 20;
t = time % loopTime;
xWiggle = wiggle(freq, amp, 1, 0.5, t);
yValue = value[1]; // Y 위치는 현재값을 유지함
wiggle1 = [xWiggle[0], yValue];

t = time % loopTime;
xWiggle = wiggle(freq, amp, 1, 0.5, t - loopTime);
wiggle2 = [xWiggle[0], yValue];

[xPos, yPos] = linear(t, 0, loopTime, wiggle1, wiggle2);
[xPos, value[1]]; 

///////////////////////////////////////////////////////////////////////////////////////////////

//상하 움직임
freq = 1;
amp = 30;
frameCount = 600; // 원하는 프레임 수
fps = 30; // 프레임 속도
loopTime = frameCount / fps; // 프레임 수를 초 단위로 변환
t = time % loopTime;
yWiggle = wiggle(freq, amp, 1, 0.5, t);
xValue = value[0]; // X 위치는 현재값을 유지함
wiggle1 = [xValue, yWiggle[1]];

t = time % loopTime;
yWiggle = wiggle(freq, amp, 1, 0.5, t - loopTime);
wiggle2 = [xValue, yWiggle[1]];

[xPos, yPos] = linear(t, 0, loopTime, wiggle1, wiggle2);
[value[0], yPos];

