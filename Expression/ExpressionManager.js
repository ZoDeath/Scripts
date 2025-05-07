(function (me){
	var scriptName = "익스프레션 킬러"; // 스크립트 이름 설정.
	var mes = ""; // 메시지 문자열 초기화.

	// 프로퍼티에서 레이어를 반환하는 함수.
	var getLayerFromProp = function(prop) {
		var ret = null;
		var p = prop;
		while (p != null){
			if ((p instanceof AVLayer) || (p instanceof ShapeLayer) || (p instanceof TextLayer) || (p instanceof LightLayer) || (p instanceof CameraLayer)) {
				ret = p;
				break;
			}
			p = p.parentProperty;
		}
		return ret;
	}

	// 활성 컴포지션을 반환하는 함수. 없으면 null 반환.
	var getActiveComp = function() {
		var ac = app.project.activeItem;
		if (!(ac instanceof CompItem)) {
			return null;
		} else {
			return ac;
		}
	}

	// 중복된 키 프레임을 제거하는 함수.
	var killkeys = function(prop) {
		var b = false;
		switch (prop.propertyValueType) {
			case PropertyValueType.ThreeD_SPATIAL:
			case PropertyValueType.ThreeD:
			case PropertyValueType.COLOR:
			case PropertyValueType.TwoD_SPATIAL:
			case PropertyValueType.TwoD:
			case PropertyValueType.OneD:
				b = true;
				break;
			default:
				b = false;
				break;
		}
		if (b == false) return;
		var k = prop.numKeys;
		if (k >= 3) {
			for (var i = k - 1; i >= 2; i--) {
				var p = prop.keyValue(i).toString();
				var p0 = prop.keyValue(i - 1).toString();
				var p1 = prop.keyValue(i + 1).toString();
				if ((p == p0) && (p == p1)) {
					prop.removeKey(i);
				}
			}
			if (prop.numKeys == 2) {
				var s0 = prop.keyValue(1).toString();
				var s1 = prop.keyValue(2).toString();
				if (s0 == s1) {
					prop.removeKey(2);
					prop.removeKey(1);
				}
			}
		}
	}

	// 익스프레션을 키 프레임으로 변환하는 함수.
	var killExp = function(cmp, layer, prop) {
		if ((prop.canSetExpression === true) && (prop.expressionEnabled === true)) {
			try {
				var frm = Math.floor(cmp.duration * cmp.frameRate);
				for (var i = 0; i <= frm; i++) {
					var tm = i / cmp.frameRate;
					prop.setValueAtTime(tm, prop.valueAtTime(tm, false));
				}
				prop.expressionEnabled = false;
			} catch(e) {
				alert(e.toString());
			}
			killkeys(prop);
			try {
				mes += prop.name + "(" + cmp.name + "/" + layer.name + ")\n";
			} catch(e) {
				alert(e.toString());
			}
		}
	}

	// 프로퍼티를 재귀적으로 검사하고 처리하는 함수.
	var propsExec = function(cmp, layer, prop) {
		if (prop == null) return;
		if (prop instanceof Property) {
			killExp(cmp, layer, prop);
		} else if (prop instanceof PropertyGroup) {
			if (prop.numProperties > 0) {
				for (var i = 1; i <= prop.numProperties; i++) {
					propsExec(cmp, layer, prop.property(i));
				}
			}
		}
	}

	// 프로퍼티 검색을 시작하는 함수.
	var propsExecMain = function(cmp, layer) {
		propsExec(cmp, layer, layer.property("ADBE Effect Parade"));
		propsExec(cmp, layer, layer.property("ADBE Transform Group"));
		propsExec(cmp, layer, layer.property("ADBE Root Vectors Group"));
	}

	// 선택된 프로퍼티에 대해 익스프레션을 키 프레임으로 변환하는 함수.
	var kilExpresiionFromProp = function() {
		var ac = getActiveComp();
		if (ac == null) {
			alert("컴포지션을 활성화해야 함");
			return;
		}
		if (ac.selectedProperties.length <= 0) {
			alert("프로퍼티를 선택해야 함");
			return;
		}
		mes = "";
		app.beginUndoGroup(scriptName + ": 선택된 프로퍼티");
		var props = ac.selectedProperties;
		for (var i = 0; i < props.length; i++) {
			var layer = getLayerFromProp(props[i]);
			killExp(ac, layer, props[i]);
		}
		app.endUndoGroup();
		if (mes == "") {
			mes = "변환 없음";
		}
		alert(mes);
	}

	// 나머지 코드는 이와 유사한 패턴을 따름.
	// 각각의 함수는 After Effects 내에서 특정 목적을 위해 실행됨.
	// 예를 들어, 익스프레션을 활성화하거나, 특정 컴포지션에 대한 작업을 수행함.
	// 사용자 인터페이스 요소는 스크립트의 작업을 제어하는 데 사용됨.
})();
