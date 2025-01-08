// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    var PlaceHolderElement = $('#PlaceHolderHere');

    $('button[data-toggle="modal"]').click(function (event) {
        var button = $(this);
        var url = button.data('url');
        console.log("HERE");
        console.log(url);
        $.get(url).done(function (data) {
            PlaceHolderElement.html(data);
            PlaceHolderElement.find('.modal').modal('show');
        }).fail(function () {
            alert("Failed to load modal content.");
        });
    });

    PlaceHolderElement.on('click', '[data-save]', function (event) {
        var button = $(this);
        var actionType = button.data('save');
        if (actionType == 'form') {
            FormHandler(button);
        } else if (actionType == 'post') {
            SimplePostHandler(button);
        }
    });

    PlaceHolderElement.on('click', '[data-dismiss="modal"]', function () {
        PlaceHolderElement.find('.modal').modal('hide');
    });
});

function SimplePostHandler(button) {
    var PlaceHolderElement = $('#PlaceHolderHere');
    PlaceHolderElement.find('.modal').modal('hide');
    var url = button.data('url');
    console.log(url);
    $.post(url, function (response) {
        console.log(response);
        console.log(response.success);
        if (response.success) {
            alert(response.message);
            location.reload();
        }
        else {
            PlaceHolderElement.html(response);
            PlaceHolderElement.find('.modal').modal('show');
        }
    }).fail(function () {
        alert(response.message)
    });
}

function FormHandler(button) {
    var PlaceHolderElement = $('#PlaceHolderHere');
    var form = button.parents('.modal').find('form');
    var actionUrl = form.attr('action');
    console.log('URL:', actionUrl);
    var sendData = form.serialize();
    console.log('sendData:', sendData);
    $.post(actionUrl, sendData).done(function (response) {
        PlaceHolderElement.find('.modal').modal('hide');

        console.log(response.message);
        if (response.success) {
            alert(response.message);
            location.reload();
        }
        else if (!response.success) {
            PlaceHolderElement.html(response);
            if (!(response.message == undefined)) {
                alert(response.message);
            }
            PlaceHolderElement.find('.modal').modal('show');
        }
    }).fail(function () {
        alert("Failed to process the request.")
    });
}