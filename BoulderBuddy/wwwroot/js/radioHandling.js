$("[name='radio_grade']").change(function () {
    var id = 1
    var value = this.id
    console.log('gra');


    $.post("MarkGrading", {
        id: id,
        value: value
    }, function (res) {
        console.log(123);
    });
});

$("[name='radio_ascent']").change(function () {
    var id = 1
    var value = this.id
    console.log('asc');
    console.log(id);


    $.post("MarkAscent", {
        id: id,
        value: value
    }, function (res) {
        console.log(321);
    });
});
