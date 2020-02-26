
    $(document).ready(function () {

            var disabledDates = ["2020-02-21", "2020-02-15"];
            $('#dateinput').multiDatesPicker({
        beforeShowDay: function (date) {
                    var string = jQuery.datepicker.formatDate('yy-mm-dd', date);
    return [disabledDates.indexOf(string) == -1]
}
});
});

        function onClick(elem) {
            var $this = $(elem);
    var val = $this.val();
            if (val == '') {
        alert('please select a date');
            } else {
                var values = $this.val().split(',');
    var count = 0;
                for (var a = 0; a < values.length; a++) {
        count++;
}
}
document.getElementById("result").innerHTML = "Total price :$" + count * 5;
        }
