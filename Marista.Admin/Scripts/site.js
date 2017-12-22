$(function () {
    $('.delete-form').on('submit', function () {
        alert('hi');
        return confirm('Are you sure you want to delete this?');
    });
});