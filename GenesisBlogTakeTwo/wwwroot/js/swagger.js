//I want to call the SetHref function BUT....I dont want to call it immediately
setTimeout(SetHref, 1000);

function SetHref() {
    let anchor = document.querySelector(".topbar-wrapper a");
    anchor.href = "/Home/Index";
}