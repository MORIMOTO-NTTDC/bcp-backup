ko.components.register("SC13_2", {
    viewModel: function (params) {
        var self = this;

        self.vendors = params.vendorsList;
        self.ajaxing = ko.observable(false);

        self.vendorId = ko.observable();
        self.userId = ko.observable();
        self.userName = ko.observable();
        self.password = ko.observable();
        self.mail = ko.observable();
        self.admin = ko.observable(false);

        self.attachedfile = ko.observable();
        self.nowAttachedfile = ko.observable();
        self.fileName = ko.observable();
        self.fileData = ko.observable();

        self.vendorId.errorMessage = ko.observable();
        self.userId.errorMessage = ko.observable();
        self.userName.errorMessage = ko.observable();
        self.password.errorMessage = ko.observable();
        self.mail.errorMessage = ko.observable();

        function checkinputData() {

            self.vendorId.errorMessage("");
            self.userId.errorMessage("");
            self.userName.errorMessage("");
            self.password.errorMessage("");
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

            if (!checkRequire(self.password())) {
                self.password.errorMessage(global.const.message.e00010.replace("%1", "パスワード"));
                isOK = false;
            } else if (!alphaNumericSymbol(self.password())) {
                self.password.errorMessage(global.const.message.e00014.replace("%1", "パスワード"));
                isOK = false;
            } else if (!minLength(self.password(), 8)) {
                self.password.errorMessage(global.const.message.e00017.replace("%1", "パスワード").replace("%2", "８"));
                isOK = false;
            } else if (!mixAlphaNumericSymbol(self.password(), 3)) {
                self.password.errorMessage(global.const.message.e00016.replace("%1", "パスワード").replace("%2", "３"));
                isOK = false;
            } else if (self.password() === self.userId()) {
                self.password.errorMessage(global.const.message.e00033.replace("%1", "パスワード").replace("%2", "ユーザID"));
                isOK = false;
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

        function registData() {

            if (!checkinputData()) {
                alert(global.const.message.e00005);
                return;
            }

            self.ajaxing(true);

            $.ajax({
                url: "./sc13_regist.do",
                data: JSON.stringify({
                    vendorId: self.vendorId(),
                    userId: self.userId(),
                    userName: self.userName(),
                    password: self.password(),
                    mailAddress: self.mail(),
                    admin: self.admin()
                }),
                headers: {"X-XSRF-TOKEN": getCsrfTokenFromCookie()},
            }).done(() => {
                alert(global.const.message.i00001);
                params.mode("list");
            }).fail((xhr) => {
                if (xhr.status === 401) {
                    alert(global.const.message.e00002);
                    location.hash = "/SC00";
                } else if (xhr.status === 406) {
                    alert(global.const.message.e00035.replace("%1", "ユーザID").replace("%2", "ユーザID"));
                } else if (xhr.status === 409) {
                    alert(global.const.message.e00035.replace("%1", "メールアドレス").replace("%2", "メールアドレス"));
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

        // 権限入力可否
        self.enableAdminField = ko.computed(function() {
            if (self.vendorId() === "0000") {
                self.admin(true);
                return false;
            }
            self.admin(false);
            return true;
        }, self);

        self.regist = function() {
            registData();
        };

        self.cancel = function() {
            params.mode("list");
        }

        params.mode.subscribe(function(componentName) {
            if (componentName === 'regist') {
                self.vendorId((global.uvo.vendorId() === "0000")? "" : global.uvo.vendorId());
                self.userId("");
                self.userName("");
                self.password("");
                self.mail("");
                self.admin(false);
                self.vendorId.errorMessage("");
                self.userId.errorMessage("");
                self.userName.errorMessage("");
                self.password.errorMessage("");
                self.mail.errorMessage("");

                self.admin(false);
            }
        });
    },
    template: {
        element: "SC13_2-template"
    }
});
