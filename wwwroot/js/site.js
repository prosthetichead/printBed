document.addEventListener('DOMContentLoaded', () => {


    const editModalBtns = document.querySelectorAll(".simpleEditModal-btn");
    // Cache the modal element and the OK button
    const modalEl = document.getElementById('simpleEditModal');
    const okBtn = document.getElementById("simpleEditModal-ok");

    editModalBtns.forEach(btn => {
        btn.addEventListener("click", function () {
            // Transfer data attributes to the OK button
            okBtn.dataset.action = this.dataset.action;
            okBtn.dataset.controller = this.dataset.controller;
            okBtn.dataset.update = this.dataset.update;

            const showFilePicker = this.dataset.showfilepicker != 'false'; // dataset stores strings
            const title = this.dataset.title;
            const id = this.dataset.id;
            const name = this.dataset.name; 

            // Reset File Input
            const fileInput = document.getElementById('simpleEditModal-file');
            if (fileInput) fileInput.value = '';

            // Set Title and Values
            document.getElementById('simpleEditModal-title').textContent = title;
            document.getElementById('simpleEditModal-id').value = id;

            // Handle Name (check if name exists to avoid setting "undefined")
            const nameInput = document.getElementById('simpleEditModal-name');
            if (name) nameInput.value = name;
            else nameInput.value = '';

            // Show or Hide File Picker
            const fileInputGroup = document.getElementById('simpleEditModal-fileInputGroup');
            if (fileInputGroup) {
                if (showFilePicker) {
                    fileInputGroup.style.display = 'block';
                } else {
                    fileInputGroup.style.display = 'none';
                }
            }

            // Show Bootstrap 5 Modal
            // We create a new instance or get the existing one
            const modalInstance = bootstrap.Modal.getOrCreateInstance(modalEl);
            modalInstance.show();
        });
    });

    // Modal OK Button Click
    if (okBtn) {
        okBtn.addEventListener("click", function () {
            const name = document.getElementById('simpleEditModal-name').value;
            const id = document.getElementById('simpleEditModal-id').value;

            const action = this.dataset.action;
            const controller = this.dataset.controller;
            const updateSelectId = this.dataset.update;

            const formData = new FormData();

            const fileInput = document.getElementById('simpleEditModal-file');
            if (fileInput && fileInput.files.length > 0) {
                formData.append('image', fileInput.files[0]);
            }

            formData.append('name', name);

            if (action !== 'create') {
                formData.append('id', id);
            }

            fetch(`/${controller}/${action}`, {
                method: 'POST',
                body: formData
            })
                .then(response => response.json())
                .then(data => {
                    console.log(data);
                    
                    const targetSelect = document.querySelector(updateSelectId);

                    if (targetSelect) {
                        console.log("update list");

                        // Create and append option
                        const option = document.createElement("option");
                        option.value = data.id;
                        option.text = data.name;
                        targetSelect.appendChild(option);

                        // Set value and trigger change event
                        targetSelect.value = data.id;
                        targetSelect.dispatchEvent(new Event('change'));

                        // Hide the modal manually if needed
                        const modalInstance = bootstrap.Modal.getInstance(modalEl);
                        if (modalInstance) modalInstance.hide();
                    } else {
                        console.log("reload page");
                        location.reload();
                    }
                })
                .catch(error => console.error('Error:', error));
        });
    }
});






////Create Creator Modal

//$(".newTag-btn").click(function () {
//    var updateSelectId = $(this).data('update');
//    var textBoxId = $(this).data('text');
//    var textBox = $(textBoxId);
//    var updateSelect = $(updateSelectId);

//    console.log(textBoxId);

//    var formData = new FormData();
//    formData.append('name', textBox.val());

//    $.ajax({
//        url: '/Tags/Create',
//        type: 'POST',
//        data: formData,
//        contentType: false,
//        processData: false,
//        dataType: 'json',
//        success: function (data) {
//            console.log(data);
//            if (updateSelect.length > 0) {

//                let simpleSelect = document.querySelector(updateSelectId).slim;
//                var selectData = simpleSelect.getData();
//                if (selectData.some(item => item.value === data.id)) {
//                    console.log('The value already exists.');
//                } else {
//                    selectData.push({ text: data.name, value: data.id });
//                    simpleSelect.setData(selectData);
//                }

//                var values = simpleSelect.getSelected();
//                values.push(data.id);
//                simpleSelect.setSelected(values);            
//            }
//        }
//    });
//});

//$(".simpleEditModal-btn").click(function () {
//    //
//    $("#simpleEditModal-ok").data("action", $(this).data('action'));
//    $("#simpleEditModal-ok").data("controller", $(this).data('controller'));
//    $("#simpleEditModal-ok").data("update", $(this).data('update'));
    
//    var showFilePicker = $(this).data('showFilePicker');
//    var title = $(this).data('title');
//    var id = $(this).data('id');
//    var name = $(this).data('name');
         
//    $('#simpleEditModal-file').val(null); //clear the file picker..
//    $('#simpleEditModal-title').html(title);
//    $('#simpleEditModal-name').val(name);
//    $('#simpleEditModal-id').val(id);
//    if (showFilePicker == false) {
//        $("#simpleEditModal-fileInputGroup").hide();
//    }
//    else {
//        $("#simpleEditModal-fileInputGroup").show();
//    }
    
//    $('#simpleEditModal').modal('show');
//})



//$("#simpleEditModal-ok").click(function () {
//    var name = $('#simpleEditModal-name').val();
//    var id = $('#simpleEditModal-id').val(); 
//    var action = $(this).data('action');
//    var controller = $(this).data('controller');
//    var updateSelectId = $(this).data('update');

//    var formData = new FormData();
//    if ($('#simpleEditModal-file').length > 0) {
//        formData.append('image', $('#simpleEditModal-file')[0].files[0]);
//    }
//    formData.append('name', name);
//    if (action != 'create') {
//        formData.append('id', id);
//    }

//    $.ajax({
//        url: '/' + controller + '/' + action,
//        type: 'POST',
//        data: formData,
//        contentType: false,
//        processData: false,
//        dataType: 'json',
//        success: function (data) {
//            console.log(data);
//            if ($(updateSelectId).length > 0) {
//                console.log("update list");
//                $(updateSelectId).append($('<option/>', {
//                    value: data.id,
//                    text: data.name
//                }));
//                $(updateSelectId).val(data.id).change();
//            }
//            else {
//                console.log("reload page");
//                location.reload();
//            }
//        }
//    });
//});

//$('.delete-btn').click(function () {
//    var controller = $(this).data('controller');
//    var id = $(this).data('id');

//    $.ajax({
//        url: '/' + controller + '/Delete/' + id,
//        type: 'POST',
//        contentType: false,
//        processData: false,
//        dataType: 'json',
//        success: function (data) {
//            console.log(data);
//            console.log("deleted");
//            location.reload();
//        },
//        error: function (jqXHR, textStatus, errorThrown) {
//            console.error("An error occurred: " + textStatus + " - " + errorThrown);
//        }

//    });

//});