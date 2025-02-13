ko.components.register("SC12_1", {
    viewModel: function(params) {
        var self = this;

        self.entryMode = params.entryMode;
        self.infos = ko.observableArray();
        self.hasContinueData = ko.observable(false);

        self.ajaxing = ko.observable(false);

        // お知らせ一覧は10件ずつサーバから取得する
        const limit = 10;

        params.mode.subscribe(function(newVal) {
            if (newVal === 'list') {
                fetchData({continue: false});
            }
        });

        function fetchData(option /* continue: 続きを表示する場合のみtrue */) {

            self.ajaxing(true);

            $.ajax({
                url: "./sc12_list.do",
                data: JSON.stringify({
                    offset: option.continue ? self.infos().length : 0,
                    limit: limit,
                }),
                headers: {"X-XSRF-TOKEN": getCsrfTokenFromCookie()},
            }).done((data) => {

                if (!option.continue) {
                    self.infos.removeAll();
                }

                // foreachバインドのafterAddに検知させる目的で、ObservableArrayに１件ずつpushする
                data.infos.forEach(function(info) {
                    self.infos.push(info);
                });

                // サーバからの真偽値は文字列で来るので、比較方法に注意すること
                self.hasContinueData(data.continue === "true" ? true : false);

            }).fail((xhr) => {
                if (xhr.status === 401) {
                    alert(global.const.message.e00002);
                    location.hash = "/SC00";
                } else {
                    alert(global.const.message.e00001 + " http status:" + xhr.status);
                }

            }).always(() => {
                self.ajaxing(false);
            });

        }

        // 「続きを表示」ボタン
        self.fetchMore = function() {
            fetchData({continue: true});
        };

        self.showSC12_2 = function() {
            params.mode('regist');
        }

        self.showSC12_3 = function(info) {
            params.selectedInfo(info);
            params.mode('detail');
        }

        self.flashAnimation = function(el) {
            if (self.infos().length > limit) {
                flash_TR_element(el);
            }
        };

        fetchData({continue: false});
    },
    template: {
        element: "SC12_1-template"
    }
});
