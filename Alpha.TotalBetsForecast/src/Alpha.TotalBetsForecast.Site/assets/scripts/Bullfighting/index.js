
var bullfighting = {
    rootUrl: document.getElementById('RootURL').value,
    countDownSeconds: 30,    
    isStartCountdown: false,    
    currentRequest: null,

    urls: {
        getMatches: 'Bullfighting/GetMatches'
    },

    domElements: {
        btnSubmit: null,
        mainContent: null,
        refreshValues: null,
        ddlRefresh: null,
        blockIconSmall: null
    },

    init: function () {
        $(document).ajaxStop($.unblockUI);
        bullfighting.cachedDOM();
        bullfighting.registerEvents();

        bullfighting.loadMatchForecasts();
    },

    getAbsoluteUrl: function (relativeUrl) {
        return bullfighting.rootUrl + relativeUrl;
    },

    cachedDOM: function () {
        bullfighting.domElements.btnSubmit = $('#submitRefresh');
        bullfighting.domElements.mainContent = $('#mainContent');
        bullfighting.domElements.refreshValues = $('#refresh-value a');
        bullfighting.domElements.ddlRefresh = $('#ddlRefresh');
        bullfighting.domElements.blockIconSmall = $('#blockIconSmall');
    },

    registerEvents: function () {

        bullfighting.domElements.btnSubmit
            .off('click.forecast')
            .on('click.forecast', function(event){
                bullfighting.loadMatchForecasts();                
                event.stopPropagation();
            });
       
        bullfighting.domElements.refreshValues
            .off('click.forecast')
            .on('click.forecast', function () {
                bullfighting.countDownSeconds = parseInt($(this).data('refresh-value'));
                bullfighting.domElements.ddlRefresh.text(bullfighting.countDownSeconds);
                if (bullfighting.isStartCountdown) {
                    bullfighting.stopCountdown();
                    bullfighting.startCountdown();
                }                
            });
    },

    loadMatchForecasts: function () {
        if (bullfighting.currentRequest) {
            bullfighting.currentRequest.abort();
        }

        bullfighting.stopCountdown();
        bullfighting.domElements.blockIconSmall.show();

        bullfighting.currentRequest = $.ajax({
            url: bullfighting.getAbsoluteUrl(bullfighting.urls.getMatches),
            type: "GET",
        }).done(function (responeHtml) {            
                bullfighting.removeLoadingIcon();
                bullfighting.domElements.mainContent.html(responeHtml);          
                bullfighting.startCountdown();
        }).fail(function (jqXHR, textStatus, errorThrown) {
            bullfighting.removeLoadingIcon();
        });        
    },

    removeLoadingIcon: function () {
        bullfighting.domElements.ddlRefresh.removeClass('block-icon');
        bullfighting.domElements.blockIconSmall.hide();
        if (!bullfighting.isStartCountdown) {
            bullfighting.startCountdown();
        }
    },

    addMatchForecast: function (newMatchForecast, rawHtml) {
        var currentMatchForecast = bullfighting.domElements.mainContent.find('tbody[id=' + newMatchForecast.attr('id') + ']');

        currentMatchForecast.length > 0
                ? currentMatchForecast.html(newMatchForecast.html())
                : bullfighting.domElements.mainContent.append(rawHtml)

        bullfighting.domElements.mainContent.find('tbody[id=no-info-body]').remove();
    },    

    startCountdown: function () {
        bullfighting.domElements.ddlRefresh.countdown({
            date: +(new Date) + bullfighting.countDownSeconds * 1000,
            render: function (data) {
                if (data.sec != 0) {
                    bullfighting.domElements.ddlRefresh.text(data.sec);
                }
            },
            onEnd: function () {
                bullfighting.refreshDone();
            }
        });

        bullfighting.domElements.ddlRefresh.data('countdown').update(+(new Date) + bullfighting.countDownSeconds * 1000).start();

        bullfighting.isStartCountdown = true;
    },

    refreshDone: function () {
        if (bullfighting.isStartCountdown) {
            bullfighting.domElements.btnSubmit.trigger('click.forecast');
            bullfighting.domElements.ddlRefresh.data('countdown').update(+(new Date) + bullfighting.countDownSeconds * 1000).start();
        }
        else {
            bullfighting.stopCountdown();
        }        
    },

    stopCountdown: function () {
        bullfighting.isStartCountdown = false;
        var countDown = bullfighting.domElements.ddlRefresh.data('countdown');
        if (countDown) {
            countDown.stop();
            bullfighting.domElements.ddlRefresh.text(bullfighting.countDownSeconds);
        }
    }
};

$(function () {
    bullfighting.init();
});