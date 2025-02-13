ko.components.register("SC13_1", {
    viewModel: function(params) {
        var self = this;

        self.vendors = ko.observableArray();
        self.users = ko.observableArray();
        self.hasContinueData = ko.observable(false);

        self.vendorId = ko.observable();
        self.userId = ko.observable();
        self.userName = ko.observable();

        self.ajaxing = ko.observable(false);

        // ユーザ一覧は20件ずつサーバから取得する
        const limit = 20;

        params.mode.subscribe(function(newVal) {
            if (newVal === 'list') {
                self.users.removeAll();
                self.hasContinueData(false);
                fetchData({continue: false});
            }
        });

        function fetchData(option /* continue: 続きを表示する場合のみtrue */) {

            self.ajaxing(true);

            $.ajax({
                url: "./sc13_list.do",
                data: JSON.stringify({
                    vendorId: self.vendorId(),
                    userId: self.userId(),
                    userName: self.userName(),
                    offset: option.continue ? self.users().length : 0,
                    limit: limit,
                }),
                headers: {"X-XSRF-TOKEN": getCsrfTokenFromCookie()},
            }).done((data) => {
                self.vendors(data.vendors);

                if (!option.continue) {
                    self.users.removeAll();
                    self.hasContinueData(false);
                }
                // foreachバインドのafterAddに検知させる目的で、ObservableArrayに１件ずつpushする
                data.users.forEach(function(user) {
                    self.users.push(user);
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

        // 「検索」ボタン
        self.search = function() {
            fetchData({continue: false});
        };

        // 「クリア」ボタン
        self.clear = function() {
            self.vendorId("");
            self.userId("");
            self.userName("");
        };

        self.flashAnimation = function(el) {
            if (self.users().length > limit) {
                flash_TR_element(el);
            }
        };

        self.showSC13_2 = function() {
            params.vendorsList(self.vendors());
            params.mode('regist');
        }

        self.showSC13_3 = function(user) {
            params.vendorsList(self.vendors());
            params.selectedUser(user);
            params.mode('detail');
        }

        fetchData({continue: false});
    },
    template: {
        element: "SC13_1-template"
    }
});
