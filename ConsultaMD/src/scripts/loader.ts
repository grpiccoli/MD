function loaderStart() {
    (<HTMLElement>document.getElementsByClassName('preloader-background')[0]).style.display = "block";
}

function loaderStop() {
    (<HTMLElement>document.getElementsByClassName('preloader-background')[0]).style.display = "none";
}