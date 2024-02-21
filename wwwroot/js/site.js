// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

//import { func } from "../lib/three/examples/jsm/nodes/code/FunctionNode";

// Write your JavaScript code.


//Create Creator Modal

$(".newTag-btn").click(function () {
    var updateSelectId = $(this).data('update');
    var textBoxId = $(this).data('text');
    var textBox = $(textBoxId);
    var updateSelect = $(updateSelectId);

    console.log(textBoxId);

    var formData = new FormData();
    formData.append('name', textBox.val());

    $.ajax({
        url: '/Tags/Create',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        dataType: 'json',
        success: function (data) {
            console.log(data);
            if (updateSelect.length > 0) {

                let simpleSelect = document.querySelector(updateSelectId).slim;
                var selectData = simpleSelect.getData();
                if (selectData.some(item => item.value === data.id)) {
                    console.log('The value already exists.');
                } else {
                    selectData.push({ text: data.name, value: data.id });
                    simpleSelect.setData(selectData);
                }

                var values = simpleSelect.getSelected();
                values.push(data.id);
                simpleSelect.setSelected(values);            
            }
        }
    });

});

$(".simpleEditModal-btn").click(function () {
    //
    $("#simpleEditModal-ok").data("action", $(this).data('action'));
    $("#simpleEditModal-ok").data("controller", $(this).data('controller'));
    $("#simpleEditModal-ok").data("update", $(this).data('update'));
    
    var showFilePicker = $(this).data('showFilePicker');
    var title = $(this).data('title');
    var id = $(this).data('id');
    var name = $(this).data('name');
         
    $('#simpleEditModal-file').val(null); //clear the file picker..
    $('#simpleEditModal-title').html(title);
    $('#simpleEditModal-name').val(name);
    $('#simpleEditModal-id').val(id);
    if (showFilePicker == false) {
        $("#simpleEditModal-fileInputGroup").hide();
    }
    else {
        $("#simpleEditModal-fileInputGroup").show();
    }
    
    $('#simpleEditModal').modal('show');
})



$("#simpleEditModal-ok").click(function () {
    var name = $('#simpleEditModal-name').val();
    var id = $('#simpleEditModal-id').val(); 
    var action = $(this).data('action');
    var controller = $(this).data('controller');
    var updateSelectId = $(this).data('update');

    var formData = new FormData();
    if ($('#simpleEditModal-file').length > 0) {
        formData.append('image', $('#simpleEditModal-file')[0].files[0]);
    }
    formData.append('name', name);
    if (action != 'create') {
        formData.append('id', id);
    }

    $.ajax({
        url: '/' + controller + '/' + action,
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        dataType: 'json',
        success: function (data) {
            console.log(data);
            if ($(updateSelectId).length > 0) {
                console.log("update list");
                $(updateSelectId).append($('<option/>', {
                    value: data.id,
                    text: data.name
                }));
                $(updateSelectId).val(data.id).change();
            }
            else {
                console.log("reload page");
                location.reload();
            }
        }
    });
});

$('.delete-btn').click(function () {
    var controller = $(this).data('controller');
    var id = $(this).data('id');

    $.ajax({
        url: '/' + controller + '/Delete/' + id,
        type: 'POST',
        contentType: false,
        processData: false,
        dataType: 'json',
        success: function (data) {
            console.log(data);
            console.log("deleted");
            location.reload();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error("An error occurred: " + textStatus + " - " + errorThrown);
        }

    });

});