var interval = 0.08; // 글자를 타이핑하는 타이밍 (단위: 초)
var cursor = true; // 타이핑하는 글자를 보이게 할지 여부 (true면 보이게 하고, false면 숨김)
var cursorVisibleTime = 0.5; // 커서가 깜빡이는 주기 (단위: 초)

// 한글 자음, 모음 및 종성 배열 정의
var CHO = ['ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ', 'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ'];
var JUNG = ['ㅏ', 'ㅐ', 'ㅑ', 'ㅒ', 'ㅓ', 'ㅔ', 'ㅕ', 'ㅖ', 'ㅗ', ['ㅗ', 'ㅏ'], ['ㅗ', 'ㅐ'], ['ㅗ', 'ㅣ'], 'ㅛ', 'ㅜ', ['ㅜ', 'ㅓ'], ['ㅜ', 'ㅔ'], ['ㅜ', 'ㅣ'], 'ㅠ', 'ㅡ', ['ㅡ', 'ㅣ'], 'ㅣ'];
var JONG = ['', 'ㄱ', 'ㄲ', ['ㄱ', 'ㅅ'], 'ㄴ', ['ㄴ', 'ㅈ'], ['ㄴ', 'ㅎ'], 'ㄷ', 'ㄹ', ['ㄹ', 'ㄱ'], ['ㄹ', 'ㅁ'], ['ㄹ', 'ㅂ'], ['ㄹ', 'ㅅ'], ['ㄹ', 'ㅌ'], ['ㄹ', 'ㅍ'], ['ㄹ', 'ㅎ'], 'ㅁ', 'ㅂ', ['ㅂ', 'ㅅ'], 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ'];

// 한글 초성, 중성, 종성 조합을 위한 오프셋 값 정의
var HANGUL_OFFSET = 0xAC00;
var CONSONANTS = ['ㄱ', 'ㄲ', 'ㄳ', 'ㄴ', 'ㄵ', 'ㄶ', 'ㄷ', 'ㄸ', 'ㄹ', 'ㄺ', 'ㄻ', 'ㄼ', 'ㄽ', 'ㄾ', 'ㄿ', 'ㅀ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅄ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ'];

// 완성된 초성, 중성, 종성 배열 정의
var COMPLETE_CHO = ['ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ', 'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ'];
var COMPLETE_JUNG = ['ㅏ', 'ㅐ', 'ㅑ', 'ㅒ', 'ㅓ', 'ㅔ', 'ㅕ', 'ㅖ', 'ㅗ', 'ㅘ', 'ㅙ', 'ㅚ', 'ㅛ', 'ㅜ', 'ㅝ', 'ㅞ', 'ㅟ', 'ㅠ', 'ㅡ', 'ㅢ', 'ㅣ'];
var COMPLETE_JONG = ['', 'ㄱ', 'ㄲ', 'ㄳ', 'ㄴ', 'ㄵ', 'ㄶ', 'ㄷ', 'ㄹ', 'ㄺ', 'ㄻ', 'ㄼ', 'ㄽ', 'ㄾ', 'ㄿ', 'ㅀ', 'ㅁ', 'ㅂ', 'ㅄ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ'];

// 복합 자음과 모음 배열 정의
var COMPLEX_CONSONANTS = [['ㄱ', 'ㅅ', 'ㄳ'], ['ㄴ', 'ㅈ', 'ㄵ'], ['ㄴ', 'ㅎ', 'ㄶ'], ['ㄹ', 'ㄱ', 'ㄺ'], ['ㄹ', 'ㅁ', 'ㄻ'], ['ㄹ', 'ㅂ', 'ㄼ'], ['ㄹ', 'ㅅ', 'ㄽ'], ['ㄹ', 'ㅌ', 'ㄾ'], ['ㄹ', 'ㅍ', 'ㄿ'], ['ㄹ', 'ㅎ', 'ㅀ'], ['ㅂ', 'ㅅ', 'ㅄ']];
var COMPLEX_VOWELS = [['ㅗ', 'ㅏ', 'ㅘ'], ['ㅗ', 'ㅐ', 'ㅙ'], ['ㅗ', 'ㅣ', 'ㅚ'], ['ㅜ', 'ㅓ', 'ㅝ'], ['ㅜ', 'ㅔ', 'ㅞ'], ['ㅜ', 'ㅣ', 'ㅟ'], ['ㅡ', 'ㅣ', 'ㅢ']];

// 각 배열에 대한 해시 맵 생성 함수
function _makeHash(array) {
    var length = array.length;
    var hash = { 0: 0 };

    for (var i = 0; i < length; i++) {
        if (array[i]) {
            hash[array[i].charCodeAt(0)] = i;
        }
    }

    return hash;
}

// 각 배열의 해시 맵 생성
var CONSONANTS_HASH = _makeHash(CONSONANTS);
var CHO_HASH = _makeHash(COMPLETE_CHO);
var JUNG_HASH = _makeHash(COMPLETE_JUNG);
var JONG_HASH = _makeHash(COMPLETE_JONG);

// 복합 자음과 모음 배열에 대한 해시 맵 생성 함수
function _makeComplexHash(array) {
    var length = array.length;
    var hash = {};
    var code1, code2;

    for (var i = 0; i < length; i++) {
        code1 = array[i][0].charCodeAt(0);
        code2 = array[i][1].charCodeAt(0);

        if (typeof hash[code1] === 'undefined') {
            hash[code1] = {};
        }

        hash[code1][code2] = array[i][2].charCodeAt(0);
    }

    return hash;
}

// 복합 자음과 모음 배열의 해시 맵 생성
var COMPLEX_CONSONANTS_HASH = _makeComplexHash(COMPLEX_CONSONANTS);
var COMPLEX_VOWELS_HASH = _makeComplexHash(COMPLEX_VOWELS);

// 자음, 모음, 종성 여부 확인 함수 정의
function _isConsonant(c) {
    return typeof CONSONANTS_HASH[c] !== 'undefined';
}

function _isCho(c) {
    return typeof CHO_HASH[c] !== 'undefined';
}

function _isJung(c) {
    return typeof JUNG_HASH[c] !== 'undefined';
}

function _isJong(c) {
    return typeof JONG_HASH[c] !== 'undefined';
}

// 한글 문자 여부 확인 함수 정의
function _isHangul(c) {
    return 0xAC00 <= c && c <= 0xd7a3;
}

// 복합 모음 및 종성 조합 가능 여부 확인 함수 정의
function _isJungJoinable(a, b) {
    return (COMPLEX_VOWELS_HASH[a] && COMPLEX_VOWELS_HASH[a][b]) ? COMPLEX_VOWELS_HASH[a][b] : false;
}

function _isJongJoinable(a, b) {
    return COMPLEX_CONSONANTS_HASH[a] && COMPLEX_CONSONANTS_HASH[a][b] ? COMPLEX_CONSONANTS_HASH[a][b] : false;
}

// 문자 분해 함수 정의
var disassemble = function (string, grouped) {
    if (string === null) {
        throw new Error('Arguments cannot be null');
    }
    
    if (typeof string === 'object') {
        string = string.join('');
    }

    var result = [];
    var length = string.length;
    var cho, jung, jong, code, r;

    for (var i = 0; i < length; i++) {
        var temp = [];
        code = string.charCodeAt(i);

        if (_isHangul(code)) {
            code -= HANGUL_OFFSET;
            jong = code % 28;
            jung = (code - jong) / 28 % 21;
            cho = parseInt((code - jong) / 28 / 21);
            temp.push(CHO[cho]);

            if (typeof JUNG[jung] === 'object') {
                temp = temp.concat(JUNG[jung]);
            } else {
                temp.push(JUNG[jung]);
            }

            if (jong > 0) {
                if (typeof JONG[jong] === 'object') {
                    temp = temp.concat(JONG[jong]);
                } else {
                    temp.push(JONG[jong]);
                }
            }
        } else if (_isConsonant(code)) {
            if (_isCho(code)) {
                r = CHO[CHO_HASH[code]];
            } else {
                r = JONG[JONG_HASH[code]];
            }
            if (typeof r === 'string') {
                temp.push(r);
            } else {
                temp = temp.concat(r);
            }
        } else if (_isJung(code)) {
            r = JUNG[JUNG_HASH[code]];

            if (typeof r === 'string') {
                temp.push(r);
            } else {
                temp = temp.concat(r);
            }
        } else {
            temp.push(string.charAt(i));
        }

        if (grouped) {
            result.push(temp);
        } else {
            result = result.concat(temp);
        }
    }

    return result;
};

// 분해된 문자열을 다시 합치는 함수 정의
var disassembleToString = function (str) {
    if (typeof str !== 'string') {
        return '';
    }
    str = disassemble(str);
    return str.join('');
};

// 한글 문자열을 조립하는 함수 정의
var assemble = function (array) {
    if (typeof array === 'string') {
        array = disassemble(array);
    }

    var result = [];
    var length = array.length;
    var code, stage = 0;
    var complete_index = -1;
    var previous_code;
    var jong_joined = false;

    function _makeHangul(index) {
        var code, cho, jung1, jung2, jong1 = 0, jong2, hangul = '';
        jong_joined = false;

        if (complete_index + 1 > index) {
            return;
        }

        for (var step = 1; ; step++) {
            if (step === 1) {
                cho = array[complete_index + step].charCodeAt(0);

                if (_isJung(cho)) {
                    if (complete_index + step + 1 <= index && _isJung(jung1 = array[complete_index + step + 1].charCodeAt(0))) {
                        result.push(String.fromCharCode(_isJungJoinable(cho, jung1)));
                        complete_index = index;
                        return;
                    } else {
                        result.push(array[complete_index + step]);
                        complete_index = index;
                        return;
                    }
                } else if (!_isCho(cho)) {
                    result.push(array[complete_index + step]);
                    complete_index = index;
                    return;
                }

                hangul = array[complete_index + step];
            } else if (step === 2) {
                jung1 = array[complete_index + step].charCodeAt(0);

                if (_isCho(jung1)) {
                    cho = _isJongJoinable(cho, jung1);
                    hangul = String.fromCharCode(cho);
                    result.push(hangul);
                    complete_index = index;
                    return;
                } else {
                    hangul = String.fromCharCode((CHO_HASH[cho] * 21 + JUNG_HASH[jung1]) * 28 + HANGUL_OFFSET);
                }
            } else if (step === 3) {
                jung2 = array[complete_index + step].charCodeAt(0);

                if (_isJungJoinable(jung1, jung2)) {
                    jung1 = _isJungJoinable(jung1, jung2);
                } else {
                    jong1 = jung2;
                }

                hangul = String.fromCharCode((CHO_HASH[cho] * 21 + JUNG_HASH[jung1]) * 28 + JONG_HASH[jong1] + HANGUL_OFFSET);
            } else if (step === 4) {
                jung2 = array[complete_index + step].charCodeAt(0);

                if (_isJongJoinable(jong1, jung2)) {
                    jong1 = _isJongJoinable(jong1, jung2);
                } else {
                    jong1 = jung2;
                }

                hangul = String.fromCharCode((CHO_HASH[cho] * 21 + JUNG_HASH[jung1]) * 28 + JONG_HASH[jong1] + HANGUL_OFFSET);
            } else if (step === 5) {
                jung2 = array[complete_index + step].charCodeAt(0);
                jong1 = _isJongJoinable(jong1, jung2);

                hangul = String.fromCharCode((CHO_HASH[cho] * 21 + JUNG_HASH[jung1]) * 28 + JONG_HASH[jong1] + HANGUL_OFFSET);
            }

            if (complete_index + step >= index) {
                result.push(hangul);
                complete_index = index;
                return;
            }
        }
    }

    for (var i = 0; i < length; i++) {
        code = array[i].charCodeAt(0);

        if (!_isCho(code) && !_isJung(code) && !_isJong(code)) {
            _makeHangul(i - 1);
            _makeHangul(i);
            stage = 0;
            continue;
        }

        if (stage === 0) {
            if (_isCho(code)) {
                stage = 1;
            } else if (_isJung(code)) {
                stage = 4;
            }
        } else if (stage === 1) {
            if (_isJung(code)) {
                stage = 2;
            } else {
                if (_isJongJoinable(previous_code, code)) {
                    stage = 5;
                } else {
                    _makeHangul(i - 1);
                }
            }
        } else if (stage === 2) {
            if (_isJong(code)) {
                stage = 3;
            } else if (_isJung(code)) {
                if (_isJungJoinable(previous_code, code)) {

                } else {
                    _makeHangul(i - 1);
                    stage = 4;
                }
            } else {
                _makeHangul(i - 1);
                stage = 1;
            }
        } else if (stage === 3) {
            if (_isJong(code)) {
                if (!jong_joined && _isJongJoinable(previous_code, code)) {
                    jong_joined = true;
                } else {
                    _makeHangul(i - 1);
                    stage = 1;
                }
            } else if (_isCho(code)) {
                _makeHangul(i - 1);
                stage = 1;
            } else if (_isJung(code)) {
                _makeHangul(i - 2);
                stage = 2;
            }
        } else if (stage === 4) {
            if (_isJung(code)) {
                if (_isJungJoinable(previous_code, code)) {
                    _makeHangul(i);
                    stage = 0;
                } else {
                    _makeHangul(i - 1);
                }
            } else {
                _makeHangul(i - 1);
                stage = 1;
            }
        } else if (stage === 5) {
            if (_isJung(code)) {
                _makeHangul(i - 2);
                stage = 2;
            } else {
                _makeHangul(i - 1);
                stage = 1;
            }
        }

        previous_code = code;
    }

    _makeHangul(i - 1);

    return result.join('');
};

// 이전 코드에서 txt, standardTime, splited, time을 사용하는 부분을 이해하기 쉽게 수정
var txt = text.sourceText;
var standardTime = time - thisLayer.startTime;
var splited = disassemble(txt);

if (standardTime < 0) {
    result = "";
} else {
    result = assemble(splited.splice(0, Math.floor(standardTime / interval))) + (cursor && Math.round(time % cursorVisibleTime) === 0 ? "I" : "");
}

result;
