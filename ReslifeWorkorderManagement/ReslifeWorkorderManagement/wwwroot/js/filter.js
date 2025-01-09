document.addEventListener("DOMContentLoaded", (event) => {
    styleAllProgressDropdown();
    sortByProgressAndCreatedTime();
});

function styleAllProgressDropdown() {
    const ProgressDropdown = document.querySelectorAll(".progress-dropdown");
    ProgressDropdown.forEach((element) => {
        const progress = element.value;
        if (progress == 0) {
            element.classList.add("progress-requested");
        }
        else if (progress == 1) {
            element.classList.add("progress-inprogress");
        }
        else {
            element.classList.add("progress-complete");
        }
    });
}

function styleProgressDropdown(id, newProgress) {
    const element = document.querySelector(`.progress-dropdown[data-id="${id}"]`);
    console.log(element);
    if (!element) {
        console.error(`No dropdown found with data-id="${id}"`);
        return;
    }
    element.className = "";
    element.classList.add("badge");
    element.classList.add("progress-dropdown");
    if (newProgress == 0) {
        element.classList.add("progress-requested");
    }
    else if (newProgress == 1) {
        element.classList.add("progress-inprogress");
    }
    else {
        element.classList.add("progress-complete");
    }
}

function updateProgress(selectElement) {

    const newProgress = selectElement.value;
    var url = $(selectElement).data('url');
    var sendData = {
        newProgress: newProgress
    };

    console.log('URL: ', url);
    console.log("Progress", newProgress);

    $.post(url, sendData).done(function (response) {
        console.log('Response Success: ', response.success);
        if (response.success) {
            styleProgressDropdown(response.workOrderId, response.progress);
        }
        else {
            if (!(response.message == undefined)) {
                alert(response.message);
            }
        }
    }).fail(function () {
        alert("Failed to update status.")
    });
    sortByProgressAndCreatedTime();
}
document.getElementById("applyFilters").addEventListener("click", function () {
    const buildingFilter = Array.from(document.getElementById("buildingFilter").selectedOptions).map(option => option.value);
    const progressFilter = Array.from(document.getElementById("progressFilter").selectedOptions).map(option => option.value);
    const areaFilter = Array.from(document.getElementById("areaFilter").selectedOptions).map(option => option.value);

    const rows = document.querySelectorAll("#workOrdersTable tbody tr");

    rows.forEach((row) => {
        const building = row.querySelector('[data-cell="building"]').textContent.trim();
        const progress = row.querySelector('.progress-dropdown').value;
        const area = row.querySelector('[data-cell="area"]').textContent.trim();


        let isVisible = true;

        if (!(buildingFilter.includes("All") || buildingFilter.includes(building))) {
            isVisible = false;
        }
        if (!(progressFilter.includes("All") || progressFilter.includes(progress))) {
            isVisible = false;
        }
        if (!(areaFilter.includes("All") || areaFilter.includes(area))) {
            isVisible = false;
        }
        else {
            console.log("Room selected");
        }

        row.style.display = isVisible ? "" : "none";
    });
    styleVisibleRows();
});

document.getElementById("resetFilters").addEventListener("click", function () {
    document.getElementById("buildingFilter").value = "All";
    document.getElementById("progressFilter").value = "All";
    document.getElementById("areaFilter").value = "All";
    const rows = document.querySelectorAll("#workOrdersTable tbody tr");
});


function styleVisibleRows() {
    const rows = Array.from(document.querySelectorAll("#workOrdersTable tbody tr"));
    let visibleIndex = 0;
    rows.forEach((row) => {
        if (row.style.display === "") {
            if (visibleIndex % 2 === 1) {           
                row.style.background = "hsl(0 50% 0% / 0.05)"; 
            } else {
                row.style.background = "#f7f7f7";
            }
            visibleIndex++;
        }
    });
}

function sortByProgressAndCreatedTime() {
    const rows = Array.from(document.querySelectorAll("#workOrdersTable tbody tr"));
    rows.sort((a, b) => {
        const progressA = parseInt(a.querySelector('.progress-dropdown').value, 10);
        const progressB = parseInt(b.querySelector(".progress-dropdown").value, 10);

        console.log(progressA);
        console.log(progressB);
        if (progressA !== progressB) {
            console.log("What");
            return progressA - progressB;
        }
        const createdTimeA = new Date(a.querySelector("[data-created]").getAttribute("data-created")).getTime();
        const createdTimeB = new Date(b.querySelector("[data-created]").getAttribute("data-created")).getTime();
        console.log(createdTimeA);
        return createdTimeB - createdTimeA; 
    });

    const tbody = document.querySelector("#workOrdersTable tbody");
    rows.forEach((row) => tbody.appendChild(row));
}
