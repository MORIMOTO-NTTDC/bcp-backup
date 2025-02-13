ko.components.register("SC14_1", {
    viewModel: function(params) {
        var self = this;

        self.entryMode = params.entryMode;
        self.vendors = ko.observableArray();
        self.accounts = ko.observableArray();
        self.hasContinueData = ko.observable(false);

        self.vendorId = ko.observable("");
        self.accountId = ko.observable("");
        self.accountName = ko.observable("");
        self.accountAddress = ko.observable("");
        self.syncError = ko.observable(false);
        self.diskAlert = ko.observable(false);
        self.versionAlert = ko.observable(false);

        if (global.uvo.sc14mode() === "1") {
            self.syncError = ko.observable(true);
        } else if (global.uvo.sc14mode() === "2") {
            self.diskAlert = ko.observable(true);
        } else if (global.uvo.sc14mode() === "3") {
            self.versionAlert = ko.observable(true);
        }

        self.ajaxing = ko.observable(false);

        // アカウント一覧は20件ずつサーバから取得する
        const limit = 20;

        params.mode.subscribe(function(newVal) {
            if (newVal === 'list') {
                fetchData({continue: false});
            }
        });

        function fetchData(option /* continue: 続きを表示する場合のみtrue */) {

            self.ajaxing(true);

            $.ajax({
                url: "./sc14_list.do",
                data: JSON.stringify({
                    vendorId: self.vendorId(),
                    accountId: self.accountId(),
                    accountName: self.accountName(),
                    accountAddress: self.accountAddress(),
                    syncError: self.syncError()? '1':'0',
                    diskAlert: self.diskAlert()? '1':'0',
                    versionAlert: self.versionAlert()? '1':'0',
                    offset: option.continue ? self.accounts().length : 0,
                    limit: limit,
                }),
                headers: {"X-XSRF-TOKEN": getCsrfTokenFromCookie()},
            }).done((data) => {
                self.vendors(data.vendors);

                if (!option.continue) {
                    self.accounts.removeAll();
                    self.hasContinueData(false);
                }

                // foreachバインドのafterAddに検知させる目的で、ObservableArrayに１件ずつpushする
                data.accounts.forEach(function(account) {
                    self.accounts.push(account);
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
            self.accountId("");
            self.accountName("");
            self.accountAddress("");
            self.syncError(false);
            self.diskAlert(false);
            self.versionAlert(false);
        };

        self.flashAnimation = function(el) {
            if (self.accounts().length > limit) {
                const targetEl = $(el).children()[0];
                flash_TR_element(targetEl);
            }
        };

        self.showSC14_3 = function(user) {
            params.vendorsList(self.vendors());
            params.selectedAccount(user);
            params.mode('detail');
        }

        fetchData({continue: false});
    },
    template: {
        element: "SC14_1-template"
    }
});
