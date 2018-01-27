$(function () {
    $('.plus').on("click", function () {
        var count = $("[name=quantity]").val();
        if(count >= 10){
            return;
        }
        count++;
        $("[name=quantity]").val(count);
    });
    $('.minus').on("click", function () {
        var count = $("[name=quantity]").val();
        if (count <= 1) {
            return;
        }
        count--;
        $("[name=quantity]").val(count);
    });
});