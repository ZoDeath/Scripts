function updateCellsForButton() {
  var sheet = SpreadsheetApp.getActiveSpreadsheet().getActiveSheet();

  // 체크박스 업데이트 (J37:J61 -> J8:J32)
  var checkboxSourceRange = sheet.getRange('J37:J61');
  var updatedCheckboxValues = checkboxSourceRange.getValues().map(function (row) {
    return [row[0] === '완료' ? '완료' : '미완료'];
  });
  sheet.getRange('J8:J32').setValues(updatedCheckboxValues);

  // 드롭다운 값 복사 (D37:D61 -> D8:D32)
  sheet.getRange('D37:D61').copyTo(sheet.getRange('D8:D32'), { contentsOnly: true });

  // 유튜브 썸네일 이미지 처리 (I37:I61 -> C37:C61)
  for (var row = 37; row <= 61; row++) {
    var youtubeUrl = sheet.getRange('I' + row).getValue();
    var videoId = youtubeUrl.match(/(?:https?:\/\/)?(?:www\.)?(?:youtube\.com\/watch\?v=|youtu\.be\/)([^\s&]+)/);

    var thumbnailRange = sheet.getRange('C' + row);
    var existingValue = thumbnailRange.getValue();

    if (videoId && videoId[1]) {
      var thumbnailUrl = "https://img.youtube.com/vi/" + videoId[1] + "/0.jpg";
      var youtubeUrlCell = sheet.getRange('I' + row);
      youtubeUrlCell.setValue(youtubeUrl);
      youtubeUrlCell.setFontColor('blue');

      // 이미지가 없는 경우에만 이미지 생성
      if (!existingValue) {
        thumbnailRange.clearContent();
        thumbnailRange.setFormula('=IMAGE("' + thumbnailUrl + '")');
      }
    } else {
      // 유튜브 URL이 없는 경우 썸네일 셀을 비웁니다.
      thumbnailRange.clearContent();
    }
  }
}
