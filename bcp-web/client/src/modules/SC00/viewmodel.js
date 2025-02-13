ko.components.register("SC00", {
    viewModel: function (params) {

        var self = this;

        self.userId = ko.observable("");
        self.password = ko.observable("");
        self.mail = ko.observable("");
        self.isShowResetForm = ko.observable(false);

        self.login = () => {

            // 必須チェック
            if (!checkRequire(self.userId())) {
                alert(global.const.message.e00010.replace("%1", "ユーザIDまたはメールアドレス"));
                setTimeout(() => {
                    document.getElementById("user_id").focus();
                    document.getElementById("user_id").select();
                }, 200);
                return;
            }

            // 必須チェック
            if (!checkRequire(self.password())) {
                alert(global.const.message.e00010.replace("%1", "パスワード"));
                setTimeout(() => {
                    document.getElementById("password").focus();
                    document.getElementById("password").select();
                }, 200);
                return;
            }

            $.ajax({
                type: "get",
                url: "./csrf",
            }).done(() => {
                // ここから本番
                $.ajax({
                    url: "./login.do",
                    data: JSON.stringify({
                        userId: self.userId(),
                        password: self.password(),
                    }),
                    headers: {"X-XSRF-TOKEN": getCsrfTokenFromCookie()},
                }).done((data) => {
                    global.uvo.vendorId(data.vendorId);
                    global.uvo.vendorName(data.vendorName);
                    global.uvo.parentId(data.parentId);
                    global.uvo.userId(data.userId);
                    global.uvo.userName(data.userName);
                    global.uvo.userType(data.userType);
                    global.uvo.menuType(parseInt(data.userType, 10));
                    global.uvo.IsAdminUser((data.userType === global.const.code.userType.sysAdmin) ? true : ((data.userType === global.const.code.userType.vendorAdmin) ? true : false));
                    location.hash = "/SC11";
                }).fail((xhr) => {
                    if (xhr.status === 401 || xhr.status === 404) {
                        alert(global.const.message.e00030);
                        document.getElementById("user_id").focus();
                        document.getElementById("user_id").select();
                        return;
                    } else {
                        alert(global.const.message.e00001 + " http status:" + xhr.status);
                        return;
                    }
                });
            }).fail((xhr) => {
                alert(global.const.message.e00001 + " http status:" + xhr.status);
            });
    
        };

        self.resetPassword = () => {

            // 必須チェック
            if (!checkRequire(self.mail())) {
                alert(global.const.message.e00010.replace("%1", "メールアドレス"));
                setTimeout(() => {
                    document.getElementById("mailaddress").focus();
                    document.getElementById("mailaddress").select();
                }, 200);
                return;
            }

            // 形式チェック
            if (!isEmailAddress(self.mail())) {
                alert(global.const.message.e00018.replace("%1", "メールアドレス"));
                setTimeout(() => {
                    document.getElementById("mailaddress").focus();
                    document.getElementById("mailaddress").select();
                }, 200);
                return;
            }

            // 確認
            if (!confirm(global.const.message.i00005.replace("%1", "パスワードを初期化"))) {
                return;
            }

            $.ajax({
                type: "get",
                url: "./csrf",
            }).done(() => {
                // ここから本番
                $.ajax({
                    url: "./password_reset.do",
                    data: JSON.stringify({
                        mailAddress: self.mail(),
                    }),
                    headers: {"X-XSRF-TOKEN": getCsrfTokenFromCookie()},
                }).done(() => {
                    alert(global.const.message.i00012);
                    self.toggleResetForm();
                    self.password("");
                }).fail((xhr) => {
                    if (xhr.status === 404) {
                        alert(global.const.message.e00031);
                        return;
                    } else {
                        alert(global.const.message.e00001 + " http status:" + xhr.status);
                        return;
                    }
                });
            }).fail((xhr) => {
                alert(global.const.message.e00001 + " http status:" + xhr.status);
            });
        };

        self.toggleResetForm = () => {
            self.userId("");
            self.password("");
            self.mail("");
            self.isShowResetForm(!self.isShowResetForm());
            if (self.isShowResetForm()) {
                document.getElementById("mailaddress").focus();
            } else {
                document.getElementById("user_id").focus();
            }
        };

        // ユーザID入力欄でエンターキーを押下したら、パスワード入力欄にフォーカス
        self.checkEnterkeyOnUserId = function(_, e) {
            if (e.keyCode === 13) {
                document.getElementById("password").focus();
                document.getElementById("password").select();
            }
        };

        // パスワード入力欄でエンターキーを押下したら、ログイン処理
        self.checkEnterkeyOnPassword = function(_, e) {
            if (e.keyCode === 13) {
                self.login();
            }
        };

        // メールアドレス入力欄でエンターキーを押下したら、パスワード初期化
        self.checkEnterkeyOnMailaddress = function(_, e) {
            if (e.keyCode === 13) {
                self.resetPassword();
            }
        };

        // 初期フォーカスはユーザID入力欄
        document.getElementById("user_id").focus();


    },
    template: {
        element: "SC00-template"
    }
});
