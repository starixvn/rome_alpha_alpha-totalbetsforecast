var quickIntroduction = {
    domElements: {
        beginModel: null,
        carouselMain: null,
        btnLetGo: null,
        carouseles: null,
        leftCarousel: null,
        rightCarousel: null,
        bothCarousel: null,
        btnClose: null,
        quickIntroductionModal: null,
        btnShowUserguide: null,
        helpModal: null
    },

    readQuickIntroductionCookieName: 'readQuickIntroduction',

    cacheElements: function () {
        quickIntroduction.domElements.beginModel = $('#beginModal');
        quickIntroduction.domElements.carouselMain = $('#carousel-main');
        quickIntroduction.domElements.btnLetGo = $(".js-let-go-btn");

        quickIntroduction.domElements.leftCarousel = $(".left.carousel-control");
        quickIntroduction.domElements.rightCarousel = $(".right.carousel-control");
        quickIntroduction.domElements.bothCarousel = $(".carousel-control");
        quickIntroduction.domElements.btnClose = $('#btnClose');
        quickIntroduction.domElements.quickIntroductionModal = $('#quickIntroductionModal');
        quickIntroduction.domElements.btnShowUserguide = $('#btnShowUserguide');
        quickIntroduction.domElements.helpModal = $('#helpModal');
        
    },
    registerEvents: function () {
        var cookieExpire = { expires: 365, path: window.location.pathname };
        if (quickIntroduction.isIe()) {
            cookieExpire = { expires: 365 };
        }

        quickIntroduction.domElements.btnLetGo.click(function () {
            quickIntroduction.domElements.beginModel.on('hidden.bs.modal', function () {
                quickIntroduction.domElements.quickIntroductionModal.modal('show');
            })
            quickIntroduction.domElements.beginModel.modal('hide');

            $.cookie(quickIntroduction.readQuickIntroductionCookieName, true, cookieExpire);
        });

        quickIntroduction.domElements.btnClose.click(function () {
            $.cookie(quickIntroduction.readQuickIntroductionCookieName, true, cookieExpire);
        });

        quickIntroduction.domElements.btnShowUserguide.click(function () {
            quickIntroduction.domElements.quickIntroductionModal.on('hidden.bs.modal', function () {
                quickIntroduction.domElements.helpModal.modal('show');
            })
            quickIntroduction.domElements.quickIntroductionModal.modal('hide');
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
    },
    isIe: function () {
        var ua = window.navigator.userAgent;
        var msie = ua.indexOf("MSIE ");

        if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./))  // If Internet Explorer, return version number
        {
            return true;
        }

        return false;
    }
};

$(function () {
    quickIntroduction.init();
});
