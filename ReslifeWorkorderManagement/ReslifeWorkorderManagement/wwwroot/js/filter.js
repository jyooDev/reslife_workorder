document.getElementById("applyFilters").addEventListener("click", function () {
    const buildingFilter = Array.from(document.getElementById("buildingFilter").selectedOptions).map(option => option.value);
    const progressFilter = Array.from(document.getElementById("progressFilter").selectedOptions).map(option => option.value);
    const areaFilter = Array.from(document.getElementById("areaFilter").selectedOptions).map(option => option.value);

    console.log("building selected: ", buildingFilter);
    console.log("progress selected: ", progressFilter);
    console.log("area selected: ", areaFilter);

    const rows = document.querySelectorAll("#workOrdersTable tbody tr");
    console.log(rows);

    rows.forEach((row) => {
        const building = row.querySelector('[data-cell="building"]').textContent.trim();
        const progress = row.querySelector('.progress-dropdown').value;
        const area = row.querySelector('[data-cell="area"]').textContent.trim();

        console.log(building);
        console.log(progress);
        console.log(area);

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
});

document.getElementById("resetFilters").addEventListener("click", function () {
    const buildingFilter = Array.from(document.getElementById("buildingFilter").selectedOptions).map(option => option.value);
    const progressFilter = Array.from(document.getElementById("progressFilter").selectedOptions).map(option => option.value);
    const areaFilter = Array.from(document.getElementById("areaFilter").selectedOptions).map(option => option.value);
});