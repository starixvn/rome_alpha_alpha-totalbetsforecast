var quickIntroduction = {
    domElements: {
        beginModel: null,
        carouselMain: null,
        btnLetGo: null,
        carouseles: null,
        leftCarousel: null,
        rightCarousel: null,
        bothCarousel: null,
        btnClose: null
    },

    readQuickIntroductionCookieName: 'readQuickIntroduction',

    cacheElements: function () {
        quickIntroduction.domElements.beginModel = $('#beginModal');
        quickIntroduction.domElements.carouselMain = $('#carousel-main');
        quickIntroduction.domElements.btnLetGo = $(".js-let-go-btn");
        quickIntroduction.domElements.carouseles = $(".carousel");

        quickIntroduction.domElements.leftCarousel = $(".left.carousel-control");
        quickIntroduction.domElements.rightCarousel = $(".right.carousel-control");
        quickIntroduction.domElements.bothCarousel = $(".carousel-control");
        quickIntroduction.domElements.btnClose = $('#btnClose');
    },
    registerEvents: function () {
        quickIntroduction.domElements.btnLetGo.click(function () {
            quickIntroduction.domElements.beginModel.modal('hide');
            $.cookie(quickIntroduction.readQuickIntroductionCookieName, true, { expires: 365, path: window.location.pathname });
        });

        quickIntroduction.domElements.btnClose.click(function () {
            $.cookie(quickIntroduction.readQuickIntroductionCookieName, true, { expires: 365, path: window.location.pathname });
        });

        quickIntroduction.domElements.carouseles.on('slid.bs.carousel', function () {
            var carouselData = $(this).data('bs.carousel');
            var currentIndex = carouselData.getItemIndex(carouselData.$element.find('.item.active'));

            var total = carouselData.$items.length;

            // Now display this wherever you want
            var text = (currentIndex + 1) + " of " + total;
            $('#slidetext').text(text);
        });

        quickIntroduction.domElements.carouselMain.carousel({
            interval: false
        });

        quickIntroduction.domElements.carouselMain.on("slid.bs.carousel", "", quickIntroduction.checkItem).trigger('slid.bs.carousel');
    },
    checkItem: function(){
        if (quickIntroduction.domElements.carouselMain.find(".carousel-inner .item:first").hasClass("active")) {
            quickIntroduction.domElements.leftCarousel.hide();
            quickIntroduction.domElements.rightCarousel.show();
        } else if (quickIntroduction.domElements.carouselMain.find(".carousel-inner .item:last").hasClass("active")) {
            quickIntroduction.domElements.leftCarousel.show();
            quickIntroduction.domElements.rightCarousel.hide();
        } else {
            quickIntroduction.domElements.bothCarousel.show();
        }
    },
    init: function () {
        var isShowQuickIntroduction = $.cookie(quickIntroduction.readQuickIntroductionCookieName);

        if (isShowQuickIntroduction)
        {
            return;
        }

        quickIntroduction.cacheElements();

        quickIntroduction.domElements.beginModel.modal({ backdrop: 'static' }).modal('show');

        quickIntroduction.registerEvents();
    }
};

$(function () {
    quickIntroduction.init();
});
