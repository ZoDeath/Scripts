function updateCellsForButton() {
  var sheet = SpreadsheetApp.getActiveSpreadsheet().getActiveSheet();

  // 내용 복사
  copyRangeContents(sheet, 'C37:C61', 'D8:D32');
  copyRangeContents(sheet, 'D37:H61', 'E8:H32');

  // I 열에 함수 설정
  setFormulasInColumnI(sheet);

  // 체크박스 상태에 따라 배경색 변경
  updateBackgroundColorBasedOnCheckbox(sheet);
}

function copyRangeContents(sheet, sourceRange, targetRange) {
  // sourceRange의 내용을 targetRange로 복사 (함수가 아닌 데이터만)
  sheet.getRange(sourceRange).copyTo(sheet.getRange(targetRange), { contentsOnly: true });
}

function setFormulasInColumnI(sheet) {
  for (var i = 0; i < 25; i++) { // 25행 (D37:D61 -> E8:E32)
    var row = 8 + i;
    var formula = '=IF(D' + row + '="-", "", 100000)';
    sheet.getRange('I' + row).setFormula(formula);
  }
}

function updateBackgroundColorBasedOnCheckbox(sheet) {
  var checkboxSourceValues = sheet.getRange('J37:J61').getValues();
  for (var i = 0; i < checkboxSourceValues.length; i++) {
    var targetRow = i + 37;
    var checkboxValue = checkboxSourceValues[i][0];
    var rangeToColor = sheet.getRange('B' + targetRow + ':J' + targetRow);

    var backgroundColor = checkboxValue === '완료' ? '#e6e6e6' : '#ffffff';
    rangeToColor.setBackground(backgroundColor);

    updateRowBackgroundColor(sheet, i + 8, checkboxValue);
  }
}

function updateRowBackgroundColor(sheet, row, checkboxValue) {
  var backgroundColor = checkboxValue === '완료' ? '#e6e6e6' : '#ffffff';
  sheet.getRange('B' + row + ':J' + row).setBackground(backgroundColor);
  sheet.getRange('J' + row).setValue(checkboxValue);
}
