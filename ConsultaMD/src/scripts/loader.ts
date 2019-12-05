const loaderStart = () => {
    (<HTMLElement>document.getElementsByClassName('preloader-background')[0]).style.display = "block";
}

const loaderStop = () => {
    (<HTMLElement>document.getElementsByClassName('preloader-background')[0]).style.display = "none";
}

//$('.preloader-background').bind({
//    ajaxStart: function () {
//        console.log("start");
//        $(this).show();
//    },
//    ajaxStop: function () {
//        console.log("stop");
//        $(this).hide();
//    }
//});
//$(document).ajaxStart(function () {
//    loaderStart();
//});
 
//$(document).ajaxStop(function () {
//    loaderStop();
//});