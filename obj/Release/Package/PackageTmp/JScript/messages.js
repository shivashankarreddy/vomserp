//function SuccessMessage(msg) {
//    document.getElementById('divMyMessage').removeAttribute('span');    
//    document.getElementById('divMyMessage').append('<span class="Success">' + msg + '</span>');
//    document.getElementById('divMyMessage').fadeTo(2000, 1).fadeOut(3000);
//}
//function ErrorMessage(msg) {
//    document.getElementById('divMyMessage').removeAttribute('span');
//    document.getElementById('divMyMessage').innerHTML('<span class="Error">' + msg + '</span>');
//    document.getElementById('divMyMessage').fadeTo(2000, 1).fadeOut(3000);
//}

function SuccessMessage(msg) {
    $('[id$=divMyMessage]').find("span").remove();
    $('[id$=divMyMessage]').append('<span class="Success">' + msg + '</span>');
    $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
}
function ErrorMessage(msgs) {
    $('[id$=divMyMessage]').find("span").remove();
    $('[id$=divMyMessage]').append('<span class="Error">' + msgs + '</span>');
    $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
}