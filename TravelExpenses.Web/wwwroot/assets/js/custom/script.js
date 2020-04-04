$(document).on('ready',function(){
    "use strict"; 

    /* Skip Loading */
    $('.page-loading > span').on('click', function(){
        $(this).parent().fadeOut();
    });
});



$(window).on('load',function(){
    "use strict";

    $('.page-loading').fadeOut();

});