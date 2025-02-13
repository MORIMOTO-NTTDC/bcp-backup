ko.components.register("SC13_3", {
    viewModel: function (params) {
        var self = this;

        self.vendors = params.vendorsList;
        self.ajaxing = ko.observable(false);

        self.vendorId = ko.observable();
        self.userId = ko.observable();
        self.userName = ko.observable();
        self.admin = ko.observable(false);
        self.updateDate = ko.observable();
        self.mail = ko.observable();
        self.version = ko.observable();

        self.vendorId.errorMessage = ko.observable('');
        self.userId.errorMessage = ko.observable('');
        self.userName.errorMessage = ko.observable('');
        self.mail.errorMessage = ko.observable('');

        function fetchData() {

            self.ajaxing(true);

            $.ajax({
                url: "./sc13_detail.do",
                data: JSON.stringify({
                    key: params.selectedUser().key,
                }),
                headers: {"X-XSRF-TOKEN": getCsrfTokenFromCookie()},
            }).done((data) => {
                self.vendorId(data.vendorId);
                self.userId(data.userId);
                self.userName(data.userName);
                self.updateDate(data.updateDate);
                self.admin(data.admin === "true" ? true : false);
                self.mail(data.mailAddress);
                self.version(data.version);
            }).fail((xhr) => {
                if (xhr.status === 401) {
                    alert(global.const.message.e00002);
                    location.hash = "/SC00";
                } else if (xhr.status === 404) {
                    alert(JSON.parse(xhr.responseText).messages);
                    params.mode("list");
                } else {
                    alert(global.const.message.e00001 + " http status:" + xhr.status);
                    params.mode("list");
                }
            }).always(() => {
                self.ajaxing(false);
            });

        }

        function checkinputData() {

            self.vendorId.errorMessage("");
            self.userId.errorMessage("");
            self.userName.errorMessage("");
            self.mail.errorMessage("");

            var isOK = true;

            // input check
            if (!checkRequire(self.vendorId())) {
                self.vendorId.errorMessage(global.const.message.e00010.replace("%1", "所属ベンダ"));
                isOK = false;
            }

            if (!checkRequire(self.userId())) {
                self.userId.errorMessage(global.const.message.e00010.replace("%1", "ユーザID"));
                isOK = false;
            } else if (!alphaNumeric(self.userId())) {
                self.userId.errorMessage(global.const.message.e00013.replace("%1", "ユーザID"));
                isOK = false;
            }

            if (!checkRequire(self.userName())) {
                self.userName.errorMessage(global.const.message.e00010.replace("%1", "ユーザ名"));
                isOK = false;
            } else {
                var c = ngChar(self.userName());
                if (c) {
                    self.userName.errorMessage(global.const.message.e00015.replace("%1", "ユーザ名").replace("%2", c));
                    isOK = false;
                }
            }

            if (!checkRequire(self.mail())) {
                self.mail.errorMessage(global.const.message.e00010.replace("%1", "メールアドレス"));
                isOK = false;
            } else if (!isEmailAddress(self.mail())) {
                self.mail.errorMessage(global.const.message.e00018.replace("%1", "メールアドレス"));
                isOK = false;
            }

            return isOK;
        }

        function updateData(msg) {

            if (msg === "更新") {
                if (!checkinputData()) {
                    alert(global.const.message.e00005);
                    return;
                }
            }

            if (!confirm(global.const.message.i00005.replace("%1", "このユーザを" + msg))) {
                return;
            }

            self.ajaxing(true);

            const data = {};
            data.key = params.selectedUser().key;
            data.version = self.version();
            if (msg === "更新") {
                data.mode = "0";
                data.vendorId = self.vendorId();
                data.userId = self.userId();
                data.userName = self.userName();
                data.mailAddress = self.mail();
                data.admin = self.admin();
            } else {
                data.mode = "1";
            }

            $.ajax({
                url: "./sc13_modify.do",
                data: JSON.stringify(data),
                headers: {"X-XSRF-TOKEN": getCsrfTokenFromCookie()},
            }).done(() => {
                alert(msg + "しました。");
                params.mode("list");
            }).fail((xhr) => {
                if (xhr.status === 401) {
                    alert(global.const.message.e00002);
                    location.hash = "/SC00";
                } else if (xhr.status === 404) {
                    alert(global.const.message.e00036.replace("%1", msg));
                } else if (xhr.status === 406) {
                    alert(global.const.message.e00035.replace("%1", "ユーザID").replace("%2", "ユーザID"));
                } else if (xhr.status === 409) {
                    alert(global.const.message.e00035.replace("%1", "メールアドレス").replace("%2", "メールアドレス"));
                } else if (xhr.status === 412) {
                    alert(global.const.message.e00037.replace("%1", msg));
                } else {
                    alert(global.const.message.e00001 + " http status:" + xhr.status);
                }
            }).always(() => {
                self.ajaxing(false);
            });

        }

        // ベンダ名入力可否
        self.enableVendorField = ko.computed(function() {
            return((global.uvo.userType() === "0")? true : false);
        }, self);

        // ユーザID入力可否
        self.enableUserIdField = ko.computed(function() {
            if (self.vendorId() === "0000") {
                return false;
            }
            return true;
        }, self);

        // 権限入力可否
        self.enableAdminField = ko.computed(function() {
            if (self.vendorId() === "0000") {
                self.admin(true);
                return false;
            }
            self.admin(false);
            return true;
        }, self);

        self.update = function() {
            updateData("更新");
        };

        params.mode.subscribe(function(componentName) {
            if (componentName === 'detail') {
                self.vendorId("");
                self.userId("");
                self.userName("");
                self.updateDate("");
                self.admin(false);
                self.mail("");
                self.vendorId.errorMessage("");
                self.userId.errorMessage("");
                self.userName.errorMessage("");
                self.mail.errorMessage("");

                fetchData();
            }
        });

        self.remove = function() {
//            if (!confirm(global.const.message.i00005.replace("%1", "このユーザを削除"))) {
//                return;
//            }
            updateData("削除");
        };

        self.cancel = function() {
            params.mode("list");
        }
    },
    template: {
        element: "SC13_3-template"
    }
});
