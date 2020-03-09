<script>

    $(document).ready(function () {
        let disabledDates = [];
    @foreach (var item in Model)
        {
            @:disabledDates.push(new Date('@item.ToString("yyyy-MM-dd")'));
};
        $('#dateinput').multiDatesPicker({
        minDate: 0,
maxDate: 365,
//dateFormat: "yyyy-mm-dd",
altField: '#dateinput',
showOtherMonths: true,
selectOtherMonths: true,
addDisabledDates: disabledDates,
});
});

    function calculatePay(elem) {
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

</script>