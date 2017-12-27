$(function () {
    var areYouSure = function () {
        return confirm('Are you sure you want to delete this?');
    };
    $('.delete-form').on('submit', areYouSure);
    $('.btn-delete').on('click', areYouSure);
});