

const menu = document.querySelector(".menu");

menu.addEventListener("click", function () {
    expandSidebar();
})
function expandSidebar() {
    document.querySelector("body").classList.toggle("short");
    let keepSidebar = document.querySelectorAll("body.short");
    if (keepSidebar.length === 1) {
        localStorage.setItem("keepSidebar", "true");
    } else {
        localStorage.removeItem("keepSidebar");
    }
}
function showHover() {
    const li = document.querySelectorAll(".short .sidebar-nav li a");
    if (li.length > 0) {
        li.forEach(function (item){
            item.addEventListener("mouseover", function () {
                const text = item.querySelector(".short .sidebar-text");
                if (text) {
                    text.classList.add("hover");

                }
            })
            item.addEventListener("mouseout", function () {
                const text = item.querySelector(".short .sidebar-text");
                if (text) {
                    text.classList.remove("hover");
                }
            })
        })
    }
}
function showStoredSidebar() {
    if (localStorage.getItem("keepSidebar") === "true") {
        document.querySelector("body").classList.add("short");
    }
}

showStoredSidebar();
showHover();