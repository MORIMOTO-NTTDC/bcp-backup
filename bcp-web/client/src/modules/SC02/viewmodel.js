ko.components.register("SC02", {
    viewModel: function (params) {
        var self = this;

        self.newPpassword = ko.observable('');
        self.confirmPpassword = ko.observable('');
        self.newPpassword.errorMessage = ko.observable('');
        self.confirmPpassword.errorMessage = ko.observable('');

        function checkinputData() {
            self.newPpassword.errorMessage('');
            self.confirmPpassword.errorMessage('');

            var isOK = true;

            // input check
            if (!checkRequire(self.newPpassword())) {
                self.newPpassword.errorMessage(global.const.message.e00010.replace("%1", "新しいパスワード"));
                isOK = false;
            } else if (!alphaNumericSymbol(self.newPpassword())) {
                self.newPpassword.errorMessage(global.const.message.e00014.replace("%1", "新しいパスワード"));
                isOK = false;
            } else if (!minLength(self.newPpassword(), 8)) {
                self.newPpassword.errorMessage(global.const.message.e00017.replace("%1", "新しいパスワード").replace("%2", "８"));
                isOK = false;
            } else if (!mixAlphaNumericSymbol(self.newPpassword(), 3)) {
                self.newPpassword.errorMessage(global.const.message.e00016.replace("%1", "新しいパスワード").replace("%2", "３"));
                isOK = false;
            } else if (self.newPpassword() === global.uvo.userId()) {
                self.newPpassword.errorMessage(global.const.message.e00033.replace("%1", "新しいパスワード").replace("%2", "ユーザID"));
                isOK = false;
            }

            if (!checkRequire(self.confirmPpassword())) {
                self.confirmPpassword.errorMessage(global.const.message.e00010.replace("%1", "確認用パスワード"));
                isOK = false;
            }

            if (isOK && self.newPpassword() !== self.confirmPpassword()) {
                self.confirmPpassword.errorMessage(global.const.message.e00032.replace("%1", "新しいパスワード").replace("%2", "確認用パスワード"));
                isOK = false;
            }

            return isOK;
        }

        function updateData() {

            if (!checkinputData()) {
                alert(global.const.message.e00005);
                return;
            }

            if (!confirm(global.const.message.i00005.replace("%1", "パスワードを変更"))) {
                return;
            }

            $.ajax({
                url: "./password_update.do",
                data: JSON.stringify({
                    password: self.newPpassword(),
                }),
                headers: {"X-XSRF-TOKEN": getCsrfTokenFromCookie()},
            }).done(function (data,status,xhr) {
                alert(global.const.message.i00004.replace("%1", "パスワード"));
                self.newPpassword("");
                self.confirmPpassword("");
                document.getElementById("password1").focus();
            }).fail(function (xhr) {
                if (xhr.status === 401) {
                    alert(global.const.message.e00002);
                    location.hash = "/SC00";
                } else {
                    alert(global.const.message.e00001 + " http status:" + xhr.status);
                }
            });
        }

        self.submit = function() {
            updateData();
        };

        // パスワード入力欄でエンターキーを押下したら、確認パスワード入力欄にフォーカス
        self.checkEnterkeyOnPassword1 = function(_, e) {
            if (e.keyCode === 13) {
                document.getElementById("password2").focus();
                document.getElementById("password2").select();
            }
        };

        // 確認パスワード入力欄でエンターキーを押下したら、パスワード変更処理
        self.checkEnterkeyOnPassword2 = function(_, e) {
            if (e.keyCode === 13) {
                updateData();
            }
        };

        document.getElementById("password1").focus();

    },
    template: {
        element: "SC02-template"
    }
});
