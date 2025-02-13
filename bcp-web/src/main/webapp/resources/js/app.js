ko.components.register("BLANK", {
    viewModel: function (params) {
    },
    template: {
        element: "BLANK-template"
    }
});
ko.components.register("navbar", {
    viewModel: function (params) {
        var self = this;

        self.logout = () => {

            if (!confirm(global.const.message.i00005.replace("%1", "ログアウト"))) {
                return;
            }

            $.ajax({
                url: "./logout.do",
                headers: {"X-XSRF-TOKEN": getCsrfTokenFromCookie()},
            }).always(() => {
                alert(global.const.message.i00011);
                location.hash = "";
            });
        };

        self.passChange = () => {
            location.hash = "/SC02";
        };

    },
    template: {
        element: "navbar-template"
    }
});
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
ko.components.register("SC11", {
    viewModel: function(params) {
        var self = this;

        self.loading = ko.observable(false);
        self.downloading = ko.observable(false);

        self.infos = ko.observableArray();
        self.accountTotal = ko.observable();
        self.accountError = ko.observable();
        self.accountDisk = ko.observable();
        self.accountSoft = ko.observable();

        fetchData();

        function fetchData() {

            self.loading(true);

            $.ajax({
                url: "./sc11.do",
                data: "{}",
                headers: {"X-XSRF-TOKEN": getCsrfTokenFromCookie()},
            }).done((data) => {
                self.infos(data.infos);
                self.accountTotal(data.accountsSummary.total);
                self.accountError(data.accountsSummary.error);
                self.accountDisk(data.accountsSummary.disk);
                self.accountSoft(data.accountsSummary.soft);
            }).fail((xhr) => {
                if (xhr.status === 401) {
                    alert(global.const.message.e00002);
                    location.hash = "/SC00";
                } else {
                    alert(global.const.message.e00001 + " http status:" + xhr.status);
                }
            }).always(() => {
                self.loading(false);
            });

        }

        self.nextSC14 = (mode) => {
            if (mode === "1" && self.accountError() === "0") { return; }
            if (mode === "2" && self.accountDisk() === "0") { return; }
            if (mode === "3" && self.accountSoft() === "0") { return; }
            global.uvo.sc14mode(mode);
            location.hash = "/SC14";
        }

        self.downloadAttachedFile = (r) => {

            self.loading(true);
            self.downloading(true);

            $.ajax({
                url: "./sc11_download.do",
                data: JSON.stringify({
                    key: r.key
                }),
                headers: {"X-XSRF-TOKEN": getCsrfTokenFromCookie()},
            }).done((data) => {
               let el = document.createElement('a');
               el.href = encodeURI(data.attachedfileData);
               el.download = data.attachedfileName;
               el.target = '_blank';
               el.click();
            }).fail((xhr) => {
                if (xhr.status === 401) {
                    alert(global.const.message.e00002);
                    location.hash = "/SC00";
                } else if (xhr.status === 404) {
                    alert(global.const.message.e00036.replace("%1", "ダウンロード"));
                } else {
                    alert(global.const.message.e00001 + " http status:" + xhr.status);
                }
            }).always(() => {
                self.loading(false);
                self.downloading(false);
            });
        }
    },
    template: {
        element: "SC11-template"
    }
});
ko.components.register("SC12", {
    viewModel: function(params) {
        const self = this;

        self.mode = ko.observable("list");
        self.selectedInfo = ko.observable();

    },
    template: {
        element: "SC12-template"
    }
});
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
ko.components.register("SC12_2", {
    viewModel: function (params) {
        var self = this;

        self.ajaxing = ko.observable(false);

        self.startDate = ko.observable();
        self.endDate = ko.observable();
        self.title = ko.observable();
        self.article = ko.observable();

        self.attachedfile = ko.observable();
        self.fileName = ko.observable();
        self.fileData = ko.observable();

        self.startDate.errorMessage = ko.observable('');
        self.endDate.errorMessage = ko.observable('');
        self.title.errorMessage = ko.observable('');
        self.article.errorMessage = ko.observable('');

        function checkinputData() {

            self.startDate.errorMessage("");
            self.endDate.errorMessage("");
            self.title.errorMessage("");
            self.article.errorMessage("");

            var isOK = true;

            // input check
            if (!checkRequire(self.startDate())) {
                self.startDate.errorMessage(global.const.message.e00010.replace("%1", "掲載開始日"));
                isOK = false;
            } else if (!isDate(self.startDate())) {
                self.startDate.errorMessage(global.const.message.e00019.replace("%1", "掲載開始日"));
                isOK = false;
            }

            if (!isDate(self.endDate())) {
                self.endDate.errorMessage(global.const.message.e00019.replace("%1", "掲載終了日"));
                isOK = false;
            } else if (self.endDate() && (self.endDate() < self.startDate())) {
                self.endDate.errorMessage(global.const.message.e00021.replace("%1", "掲載終了日").replace("%2", "掲載開始日"));
                isOK = false;
            }

            if (!checkRequire(self.title())) {
                self.title.errorMessage(global.const.message.e00010.replace("%1", "タイトル"));
                isOK = false;
            } else {
                var c = ngChar(self.title());
                if (c) {
                    self.title.errorMessage(global.const.message.e00015.replace("%1", "タイトル").replace("%2", c));
                    isOK = false;
                }
            }

            if (!checkRequire(self.article())) {
                self.article.errorMessage(global.const.message.e00010.replace("%1", "記事"));
                isOK = false;
            } else {
                var c = ngChar(self.article());
                if (c) {
                    self.article.errorMessage(global.const.message.e00015.replace("%1", "記事").replace("%2", c));
                    isOK = false;
                }
            }
            return isOK;
        }

        function registData() {

            if (!checkinputData()) {
                alert(global.const.message.e00005);
                return;
            }

            self.ajaxing(true);

            const files = [];
            if (self.fileName()) {
                files.push({
                    attachedfileName: self.fileName(),
                    attachedfileData: self.fileData()
                })
            }

            $.ajax({
                url: "./sc12_regist.do",
                data: JSON.stringify({
                    startDate: (self.startDate().length === 10)? self.startDate().replaceAll('-', '/') : '',
                    endDate: (self.endDate().length === 10)? self.endDate().replaceAll('-', '/') : '',
                    title: self.title(),
                    article: self.article(),
                    files: files
                }),
                headers: {"X-XSRF-TOKEN": getCsrfTokenFromCookie()},
            }).done(() => {
                alert(global.const.message.i00001);
                params.mode("list");
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

		self.attachedfile.subscribe((_) => {
            if (!self.attachedfile()) {
                return;
            }
            var attacheFile = document.querySelector('#addUploadfile');
            var file = attacheFile.files[0];
            self.fileName(file.name);

            var reader = new FileReader();
            reader.onload = (event) => {
                self.fileData(event.currentTarget.result);
            }
            reader.readAsDataURL(file);
		});

        self.regist = function() {
            registData();
        };

        self.cancel = function() {
            params.mode("list");
        }

        params.mode.subscribe(function(componentName) {

            if (componentName === 'regist') {

                var getToday = new Date();
                var m = getToday.getMonth() + 1;
                var today = getToday.getFullYear() + "-" + m.toString().padStart(2,'0') + "-" + getToday.getDate().toString().padStart(2,'0');

                self.startDate(today);
                self.endDate("");
                self.title("");
                self.article("");
                self.attachedfile("");
                self.fileName("");
                self.fileData("");

                self.startDate.errorMessage("");
                self.endDate.errorMessage("");
                self.title.errorMessage("");
                self.article.errorMessage("");
            }
        });
    },
    template: {
        element: "SC12_2-template"
    }
});
ko.components.register("SC12_3", {
    viewModel: function (params) {
        var self = this;

        self.ajaxing = ko.observable(false);
        self.startDate = ko.observable();
        self.endDate = ko.observable();
        self.title = ko.observable();
        self.article = ko.observable();
        self.version = ko.observable();

        self.startDate.errorMessage = ko.observable('');
        self.endDate.errorMessage = ko.observable('');
        self.title.errorMessage = ko.observable('');
        self.article.errorMessage = ko.observable('');

        function fetchData() {

            self.ajaxing(true);

            $.ajax({
                url: "./sc12_detail.do",
                data: JSON.stringify({
                    key: params.selectedInfo().key,
                }),
                headers: {"X-XSRF-TOKEN": getCsrfTokenFromCookie()},
            }).done((data) => {
                self.startDate((data.startDate.length === 10)? data.startDate.replaceAll('/', '-') : '');
                self.endDate((data.endDate.length === 10)? data.endDate.replaceAll('/', '-') : '');
                self.title(data.title);
                self.article(data.article);
                self.nowAttachedfile(data.files.length === 0 ? 'なし' : data.files[0].attachedfileName);
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

        self.attachedfile = ko.observable();
        self.nowAttachedfile = ko.observable();
        self.fileName = ko.observable();
        self.fileData = ko.observable();

        function checkinputData() {

            self.startDate.errorMessage("");
            self.endDate.errorMessage("");
            self.title.errorMessage("");
            self.article.errorMessage("");

            var isOK = true;

            // input check
            if (!checkRequire(self.startDate())) {
                self.startDate.errorMessage(global.const.message.e00010.replace("%1", "掲載開始日"));
                isOK = false;
            } else if (!isDate(self.startDate())) {
                self.startDate.errorMessage(global.const.message.e00019.replace("%1", "掲載開始日"));
                isOK = false;
            }

            if (!isDate(self.endDate())) {
                self.endDate.errorMessage(global.const.message.e00019.replace("%1", "掲載終了日"));
                isOK = false;
            } else if (self.endDate() && (self.endDate() < self.startDate())) {
                self.endDate.errorMessage(global.const.message.e00021.replace("%1", "掲載終了日").replace("%2", "掲載開始日"));
                isOK = false;
            }

            if (!checkRequire(self.title())) {
                self.title.errorMessage(global.const.message.e00010.replace("%1", "タイトル"));
                isOK = false;
            } else {
                var c = ngChar(self.title());
                if (c) {
                    self.title.errorMessage(global.const.message.e00015.replace("%1", "タイトル").replace("%2", c));
                    isOK = false;
                }
            }

            if (!checkRequire(self.article())) {
                self.article.errorMessage(global.const.message.e00010.replace("%1", "記事"));
                isOK = false;
            } else {
                var c = ngChar(self.article());
                if (c) {
                    self.article.errorMessage(global.const.message.e00015.replace("%1", "記事").replace("%2", c));
                    isOK = false;
                }
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

            if (!confirm(global.const.message.i00005.replace("%1", "このお知らせを" + msg))) {
                return;
            }

            self.ajaxing(true);

            const files = [];
            if (self.fileName()) {
                files.push({
                    attachedfileName: self.fileName(),
                    attachedfileData: self.fileData()
                })
            }

            const data = {};
            data.key = params.selectedInfo().key;
            data.version = self.version();
            if (msg === "更新") {
                data.mode = "0";
                data.startDate = (self.startDate().length === 10) ? self.startDate().replaceAll('-', '/') : '';
                data.endDate = (self.endDate().length === 10) ? self.endDate().replaceAll('-', '/') : '';
                data.title = self.title();
                data.article = self.article();
                data.files = files;
            } else {
                data.mode = "1";
            }

            $.ajax({
                url: "./sc12_modify.do",
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
                } else if (xhr.status === 412) {
                    alert(global.const.message.e00037.replace("%1", msg));
                } else {
                    alert(global.const.message.e00001 + " http status:" + xhr.status);
                }
            }).always(()=> {
                self.ajaxing(false);
            });

        }

		self.attachedfile.subscribe((_) => {
            if (!self.attachedfile()) {
                return;
            }
            var attacheFile = document.querySelector('#modUploadfile');
            var file = attacheFile.files[0];
            self.fileName(file.name);

            var reader = new FileReader();
            reader.onload = (event) => {
                self.fileData(event.currentTarget.result);
            }
            reader.readAsDataURL(file);
		});

        self.update = function() {
            updateData("更新");
        };

        params.mode.subscribe(function(componentName) {
            if (componentName === 'detail') {
                self.startDate("");
                self.endDate("");
                self.title("");
                self.article("");
                self.attachedfile("");
                self.nowAttachedfile("");
                self.fileName("");
                self.fileData("");

                self.startDate.errorMessage("");
                self.endDate.errorMessage("");
                self.title.errorMessage("");
                self.article.errorMessage("");

                fetchData();
            }
        });

        self.remove = function () {
            updateData("削除");
        };

        self.cancel = function() {
            params.mode("list");
        }
    },
    template: {
        element: "SC12_3-template"
    }
});
ko.components.register("SC13", {
    viewModel: function(params) {
        const self = this;

        self.mode = ko.observable("list");
        self.selectedUser = ko.observable();
        self.vendorsList = ko.observableArray();
    },
    template: {
        element: "SC13-template"
    }
});
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
ko.components.register("SC14", {
    viewModel: function(params) {
        const self = this;

        self.mode = ko.observable("list");
        self.selectedAccount = ko.observable();
        self.vendorsList = ko.observableArray();

    },
    template: {
        element: "SC14-template"
    }
});
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
ko.components.register("SC14_3", {
    viewModel: function (params) {
        var self = this;

        self.vendors = params.vendorsList;
        self.ajaxing = ko.observable(false);

        self.id = ko.observable();
        self.name = ko.observable();
        self.address = ko.observable();
        self.usedSize = ko.observable();
        self.backupCapa = ko.observable();
        self.softVersion = ko.observable();
        self.lastdate = ko.observable();
        self.status = ko.observable();
        self.errorInfo = ko.observable();
        self.uploadEnable = ko.observable();
        self.uploadEnableBK = "";
        self.uploadTiming = ko.observable();
        self.downloadEnable = ko.observable();
        self.downloadEnableBK = "";
        self.vendorId = ko.observable();
        self.s3backet = ko.observable();
        self.localDir = ko.observable();
        self.version = ko.observable();

        self.backupCapa.errorMessage = ko.observable('');
        self.uploadTiming.errorMessage = ko.observable('');
        self.vendorId.errorMessage = ko.observable('');

        function fetchData() {

            self.ajaxing(true);

            $.ajax({
                url: "./sc14_detail.do",
                data: JSON.stringify({
                    accountId: params.selectedAccount().accountId,
                }),
                headers: {"X-XSRF-TOKEN": getCsrfTokenFromCookie()},
            }).done((data) => {
                self.id(data.accountId);
                self.name(data.accountName);
                self.address(data.accountAddress);
                self.usedSize(data.usedSize);
                self.softVersion(data.softVersion);
                self.lastdate(data.lastdate);
                self.status(data.status);
                self.errorInfo(data.errorInfo ? data.errorInfo : "なし");
                self.backupCapa(data.backupCapa);
                self.uploadEnable(data.uploadEnable === "true");
                self.uploadEnableBK = self.uploadEnable();
                self.uploadTiming(data.uploadTiming);
                self.downloadEnable(data.downloadEnable === "true");
                self.downloadEnableBK = self.downloadEnable();
                self.vendorId(data.vendorId);
                self.changeVendor();
                self.s3backet(data.s3backet);
                self.localDir(data.localDir);
                self.version(data.version);
            }).fail((xhr) => {
                if (xhr.status === 401) {
                    alert(global.const.message.e00002);
                    location.hash = "/SC00";
                } else if (xhr.status === 404) {
                    alert(JSON.parse(xhr.responseText).messages);
                    params.mode("list");
                } else {
                    alert(global.const.message.e00001 + " httpstatsu:" + xhr.status);
                    params.mode("list");
                }
            }).always(() => {
                self.ajaxing(false);
            });

        }

        function checkinputData() {

            self.backupCapa.errorMessage("");
            self.uploadTiming.errorMessage("");
            self.vendorId.errorMessage("");

            var isOK = true;

            // input check
            if (self.uploadEnable() && !checkRequire(self.uploadTiming())) {
                self.uploadTiming.errorMessage(global.const.message.e00010.replace("%1", "アップロード頻度"));
                isOK = false;
            }

            if (!checkRequire(self.vendorId())) {
                self.vendorId.errorMessage(global.const.message.e00010.replace("%1", "所属"));
                isOK = false;
            }

            return isOK;
        }

        function updateData() {

            if (!checkinputData()) {
                alert(global.const.message.e00005);
                return;
            }

            if (!self.uploadEnable() && self.uploadEnableBK) {
                if (!confirm(global.const.message.i00015)) {
                    return;
                }
            }

            if (self.downloadEnable() && !self.downloadEnableBK) {
                if (!confirm(global.const.message.i00016)) {
                    return;
                }
            }

            if (!confirm(global.const.message.i00005.replace("%1", "このアカウントを更新"))) {
                return;
            }

            self.ajaxing(true);

            $.ajax({
                url: "./sc14_update.do",
                data: JSON.stringify({
                    accountId: self.id(),
                    uploadEnable: self.uploadEnable(),
                    uploadTiming: self.uploadTiming(),
                    downloadEnable: self.downloadEnable(),
                    vendorId: self.vendorId(),
                    version: self.version()
                }),
                headers: {"X-XSRF-TOKEN": getCsrfTokenFromCookie()},
            }).done(() => {
                alert(global.const.message.i00002);
                params.mode("list");
            }).fail((xhr) => {
                if (xhr.status === 401) {
                    alert(global.const.message.e00002);
                    location.hash = "/SC00";
                } else if (xhr.status === 404) {
                    alert(global.const.message.e00036.replace("%1", "更新"));
                } else if (xhr.status === 412) {
                    alert(global.const.message.e00037.replace("%1", "更新"));
                } else {
                    alert(global.const.message.e00001 + " http status:" + xhr.status);
                }
            }).always(() => {
                self.ajaxing(false);
            });

        }

        // アップロード実行変更
        self.changeUploadEnable = function () {
            if (!self.uploadEnable() && self.uploadEnableBK) {
                if (!confirm(global.const.message.i00013)) {
                    self.uploadEnable(true);
                }
            }
        };

        // ダウンロード実行変更
        self.changeDownloadEnable = function () {
            if (self.downloadEnable() && !self.downloadEnableBK) {
                if (!confirm(global.const.message.i00014)) {
                    self.downloadEnable(false);
                }
            }
        };

        // ベンダ名変更
        self.changeVendor = function () {
            var vendor = self.vendors().find(function(v) {
                return v.id === self.vendorId();
            });
            self.s3backet((vendor)? vendor.s3backet : '');
        };

        // ベンダ名入力可否
        self.enableVendorField = ko.computed(function() {
            return((global.uvo.userType() === "0")? true : false);
        }, self);

        self.update = function() {
            updateData();
        };

        params.mode.subscribe(function(componentName) {
            if (componentName === 'detail') {

                self.id("");
                self.name("");
                self.address("");
                self.usedSize("");
                self.backupCapa("");
                self.softVersion("");
                self.lastdate("");
                self.status("");
                self.errorInfo("");
                self.uploadEnable("");
                self.uploadEnableBK = "";
                self.uploadTiming("");
                self.downloadEnable("");
                self.downloadEnableBK = "";
                self.vendorId("");
                self.s3backet("");
                self.localDir("");
                self.version("");

                self.backupCapa.errorMessage("");
                self.uploadTiming.errorMessage("");
                self.vendorId.errorMessage("");

                fetchData();
            }
        });

        self.cancel = function() {
            params.mode("list");
        }
    },
    template: {
        element: "SC14_3-template"
    }
});
ko.components.register("SC20", {
    viewModel: function (params) {
        var self = this;

        self.links = ko.observableArray();
        self.ajaxing = ko.observable(false);

        fetchData();

        function fetchData() {

            self.ajaxing(true);

            $.ajax({
                url: "./sc20_list.do",
                data: "{}",
                headers: {"X-XSRF-TOKEN": getCsrfTokenFromCookie()},
            }).done((data) => {
                self.links(data.links);
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
    },
    template: {
        element: "SC20-template"
    }
});
ko.components.register("sidebar", {
    viewModel: function (params) {
        var self = this;
    },
    template: {
        element: "sidebar-template"
    }
});
ko.components.register("sidebar-menu-item", {
    viewModel: function (params) {

        var self = this;

        self.text = ko.observable(params.text);
        self.isActive = ko.observable(params.componentName === currentComponentName());
        self.componentName = ko.observable("#/" + params.componentName);

        currentComponentName.subscribe(function() {

            if (params.componentName === currentComponentName()) {
                self.isActive(true);
            } else {
                self.isActive(false);
            }
            let myOffCanvas = document.getElementById('offcanvasSidebar');
            let openedCanvas = bootstrap.Offcanvas.getInstance(myOffCanvas);
            if (openedCanvas) {
                openedCanvas.hide();
            }
        });

    },
    template: {
        element: "sidebar-menu-item-template"
    }
});
ko.components.register("sidebar-offcanvas", {
    viewModel: function (params) {
        var self = this;
    },
    template: {
        element: "sidebar-offcanvas-template"
    }
});
const appVersion = "1.00";

const currentComponentName = ko.observable("SC00");

function setComponentName(name) {
    currentComponentName(name);
}

const global = {
    uvo: {
        vendorId: ko.observable(""),
        vendorName: ko.observable(""),
        parentId: ko.observable(""),
        userId: ko.observable(""),
        userName: ko.observable(""),
        userType: ko.observable(""),
        menuType: ko.observable(0),
        IsAdminUser: ko.observable(false),
        sc14mode: ko.observable(0),
    },
    const: {
        message: {
            i00001: "登録しました。",
            i00002: "更新しました。",
            i00003: "削除しました。",
            i00004: "%1を変更しました。",
            i00005: "%1します。よろしいですか？",

            i00011: "ログアウトしました。ログイン画面に遷移します。",
            i00012: "パスワードを初期化しました。初期化後のパスワードはメールで確認してください。",
            i00013: "アップロード実行をOFFにします。\n自動でファイルのアップロードが行われなくなります。\n本当によろしいですか？",
            i00014: "ダウンロード実行をONにします。\n端末側のフォルダの中身が、前回バックアップ時に置き換わります。\n本当によろしいですか？",
            i00015: "アップロード実行をOFFにされました。\n自動でファイルのアップロードが行われなくなります。\n本当によろしいですか？",
            i00016: "ダウンロード実行をONにされました。\n端末側のフォルダの中身が、前回バックアップ時に置き換わります。\n本当によろしいですか？",

            e00001: "予期しないエラーが発生しました。",
            e00002: "ログインが必要です。ログイン画面に遷移します。",
            e00003: "実行権限がありません。",
            e00004: "不正なリクエストです。",
            e00005: "入力エラーがあります。",

            e00010: "%1 は入力必須です。",
            e00011: "%1 には数字を入力してください。",
            e00012: "%1 には数値を入力してください。",
            e00013: "%1 には半角英数字で入力してください。",
            e00014: "%1 には半角英数字（記号含む）で入力してください。",
            e00015: "%1 に入力禁止文字\'%2\'が含まれています。",
            e00016: "%1 には、%2種類以上の文字種別を含めてください。",
            e00017: "%1 には%2文字以上で入力してください。",
            e00018: "%1 にはｅメールアドレスの形式で入力してください。",
            e00019: "%1 には正しい日付を入力してください。",
            e00020: "%1 には%2から%3までの範囲で入力してください。",
            e00021: "%1 には%2以降の日付を入力してください。",

            e00030: "ユーザID、メールアドレスまたはパスワードが間違っています。",
            e00031: "入力したメールアドレスは登録されていないため、パスワードを初期化できません。",
            e00032: "%1 と %2 の値が異なっています。同じ値を入力してください。",
            e00033: "%1 には %2 と異なる値を入力してください。",

            e00034: "%1 は見つかりませんでした。",
            e00035: "この%1はすでに使用されています。別の%2を入力してください。",
            e00036: "別のユーザ等により、すでに削除されているため、%1できません。",
            e00037: "別のユーザ等により、すでに更新されているため、%1できません。",
            e00038: "%1には%2 を入力してください。",
            e00039: "%1が間違っています。正しい%1を入力してください。",
            e00040: "バックアップ容量は使用容量より大きい値（GB）を選択してください。",

        },
        code: {
            userType: {
                sysAdmin: "0",      // システム管理者
                vendorAdmin: "1",   // ベンダ管理者
                vendorStaff: "2",   // ベンダ担当者
            },
            adminAuthority: {
                yes: "1",           // 管理者権限あり
                no: "0",            // なし
            },
            checkStatus: {
                valid: "1",         // 有効
                invalid: "0",       // 無効
            },
			displayStatus : {
				'0': '未実行',
				'1': 'ＵＬ成功',
				'2': 'ＤＬ成功',
				'3': 'ＵＬ失敗',
				'4': 'ＤＬ失敗',
				'5': '同期不可',
			},
			displayStatusLong : {
				'0': '未実行',
				'1': 'アップロード成功',
				'2': 'ダウンロード成功',
				'3': 'アップロード失敗',
				'4': 'ダウンロード失敗',
				'5': '同期不可',
			},
        }
    }
};

const menuList = [
	[
		// システム管理者用
	    {
	        name: "ホーム",
	        componentName: "SC11",
	    },{
	        name: "お知らせ管理",
	        componentName: "SC12",
	    },{
	        name: "ユーザ管理",
	        componentName: "SC13",
	    },{
	        name: "アカウント管理",
	        componentName: "SC14",
	    },{
	        name: "外部リンク",
	        componentName: "SC20",
	    }
	],
	[
		// ベンダ管理者用
	    {
	        name: "ホーム",
	        componentName: "SC11",
	    },{
	        name: "ユーザ管理",
	        componentName: "SC13",
	    },{
	        name: "アカウント管理",
	        componentName: "SC14",
	    },{
	        name: "外部リンク",
	        componentName: "SC20",
	    }
	],
	[
		// ベンダ担当者用
	    {
	        name: "ホーム",
	        componentName: "SC11",
	    },{
	        name: "アカウント管理",
	        componentName: "SC14",
	    },{
	        name: "外部リンク",
	        componentName: "SC20",
	    }
	]
];

function ajaxSetting() {
    $.ajaxSetup({
        type: "post",           // 全てPOSTメソッドで統一
        headers: {
            "X-Requested-With": "XMLHttpRequest"
        },
        dataType: 'json',       // レスポンスは全てJSON形式
        cache: false,           // キャッシュしない
        timeout: 1000 * 30,     // タイムアウト30秒
        dataType: "json",
        contentType: "application/json",
        dataFilter: function(data) {
            if (!data) {
                return null;
            }
            try {
                // 全てのプロパティの型をStringに統一する
                function castJsonPropertiesToString(obj) {
                    for (var key in obj) {
                        if (obj[key] instanceof Array || obj[key] instanceof Object) {
                            castJsonPropertiesToString(obj[key]);
                        } else {
                            if (obj[key] === null) {
                                obj[key] = '';
                            } else {
                                obj[key] = String(obj[key]);
                            }
                        }
                    }
                    return obj;
                }
                json = castJsonPropertiesToString(JSON.parse(data));
                return JSON.stringify(json);
            } catch (e) {
                return data;
            }
        }
    });

    // ajax global events
    $(document)
        .ajaxSend(function () { $('#loading').show();$('#loading>img').show(); })
        .ajaxStop(function () {
            $('#loading>img').hide();
            setTimeout(function() {
                $('#loading').hide();
            }, 100);
        });
}

// routing
var routingList = [
    { hash: "SC00", component: "SC00", loginCheck: false},
    { hash: "SC02", component: "SC02", loginCheck: true},
    { hash: "SC11", component: "SC11", loginCheck: true},
    { hash: "SC12", component: "SC12", loginCheck: true},
    { hash: "SC13", component: "SC13", loginCheck: true},
    { hash: "SC14", component: "SC14", loginCheck: true},
    { hash: "SC20", component: "SC20", loginCheck: true},
];

// Determine the component name from url hash.
function routing() {

    var hashName = location.hash.split("/")[1];

    // ex. index.html
    if (hashName === undefined) {
        currentComponentName("SC00");
        return;
    };

    var routings = routingList.filter((root) => {
        if (hashName.split("@").length > 1) {
            // ex. index.html#/SC21@1
            return root.hash === hashName.split("@")[0];
        }
        return root.hash === hashName;
    });

    if (routings.length === 0) {
        currentComponentName("SC11");
        return;
    }

    // When page was reloaded, fetch UVO from server.
    if (global.uvo.userId() === "" && routings[0].loginCheck === true) {

        // ユーザ情報を取得するまでの間は、空白ページを表示しておく
        currentComponentName("BLANK");

        $.ajax({
            url: "./userinfo.do",
            headers: {"X-XSRF-TOKEN": getCsrfTokenFromCookie()},
        }).done((data) => {
            global.uvo.vendorId(data.vendorId);
            global.uvo.parentId(data.parentId);
            global.uvo.vendorName(data.vendorName);
            global.uvo.userId(data.userId);
            global.uvo.userName(data.userName);
            global.uvo.userType(data.userType);
            global.uvo.menuType(parseInt(data.userType, 10));
            global.uvo.IsAdminUser((data.userType === global.const.code.userType.sysAdmin) ? true : ((data.userType === global.const.code.userType.vendorAdmin) ? true : false));
            currentComponentName(routings[0].component);
        }).fail((xhr) => {
            if (xhr.status === 401) {
                alert(global.const.message.e00003);
                location.hash = "/SC00";
            } else {
                alert(global.const.message.e00001 + " http status:" + xhr.status);
            }
        });
    } else {
        currentComponentName(routings[0].component);
    }
}

window.onhashchange = function () {
    routing();
};

ajaxSetting();

routing();

ko.applyBindings({});

// input check
// 必須チェック（message.e00010）
function checkRequire(target) {
    if (target === null || typeof target === 'undefined'){
        return false;
    }
    return target.trim().length > 0;
}
// 整数チェック（message.e00011）
function isNumber(target) {
    if (target === null || target.length === 0) {
        return true;
    }
    if (!target.match(/^([1-9]\d*|0)$/)) {
        return false;
    }
    return true;
}
// 数値チェック（message.e00012）
function isNumeric(target) {
    if (target === null || target.length === 0) {
        return true;
    }
    if (!target.match(/^[-]?([1-9]\d*|0)(\.\d+)?$/)) {
        return false;
    }
    return true;
}
// 英数チェック（message.e00013）
function alphaNumeric(target) {
    if (target === null || target.length === 0) {
        return true;
    }
    if (target.match(/[^a-zA-Z0-9]+/)) {
        return false;
    }
    return true;
}
// 半角英数記号（禁止文字除く）チェック（message.e00014）
function alphaNumericSymbol(target) {
    if (target === null || target.length === 0) {
        return true;
    }
    // 半角スペースは除く半角文字
    if (!target.match(/^[\x20-\x7e]+$/)){
        return false;
    }
    var ngChars = /[\\/:*?"<>|]/.exec(target);
    if (ngChars !== null) {
        return false;
    }
    return true;
}
// 有効文字チェック（message.e00015）
// 入力禁止文字があれば、その文字を返却。なければ空文字
function ngCharCode(target) {
    if (target === null || target.length === 0) {
        return '';
    }
    var str2array = function (str) {
        var array = [], i, il = str.length;
        for (i = 0; i < il; i++) {
            array.push(str.charCodeAt(i));
        }
        return array;
    };
    const target2 = Array.from(target);
    for (var i = 0; i < target2.length; i++) {
        var c = target2[i], array = str2array(c);
        let sjis_array;
        try {
	        sjis_array = Encoding.convert(array, {to:'SJIS', from:'UNICODE', fallback: 'error'});
        } catch(e) {
        	return c;
        }
        buf = sjis_array[0].toString(16);
        if (sjis_array.length > 1) {
            buf = buf + sjis_array[1].toString(16);
        }
        var sjis_code = parseInt(buf, 16);
        if ((0xed40 <= sjis_code && sjis_code <= 0xeefc) || (0xfa40 <= sjis_code && sjis_code <= 0xfc4b) || (0xf040 <= sjis_code && sjis_code <= 0xf9fc) || (0x8540 <= sjis_code && sjis_code <= 0x889e) || (0xeaa5 <= sjis_code && sjis_code <= 0xfcfc)) {
            // 入力禁止文字を返却
            return c;
        }
    }
    return '';
}
// 入力禁止文字チェック（message.e00015）
// 入力禁止文字があれば、その文字を返却。なければ空文字
function ngChar(target) {
    if (target === null || target.length === 0) {
        return '';
    }
    // 環境依存文字チェック
    var c = ngCharCode(target);
    if (c) {
        // 入力禁止文字を返却
        return c;
    }
    var ngChars = /[\\/:*?"<>|]/.exec(target);
    if (ngChars !== null) {
        // 入力禁止文字を返却
        return ngChars[0];
    }
    return '';
}
// 半角英（a～z,A～Z）数（0～9）記号混在チェック（message.e00016）
function mixAlphaNumericSymbol(target, words) {
    var cnt = 0;
    if (target.match(/[0-9]/)) {
        cnt ++;
    }
    if (target.match(/[a-z]/)) {
        cnt ++;
    }
    if (target.match(/[A-Z]/)) {
        cnt ++;
    }
    const reg = new RegExp(/[!#$%&'()\+\-\.,;=@\[\]^_`{}~]/g);
    if (reg.test(target)) {
        cnt ++;
    }
    if (cnt < words) {
        return false;
    }
    return true;
}
// 最小文字数チェック（message.e00017）
function minLength(target, words) {
    if (target === null || target.length === 0) {
        return true;
    }
    if (target.length < parseInt(words, 10)) {
        return false;
    }
    return true;
}
// eメールアドレス形式チェック（message.e00018）
function isEmailAddress(target) {
    if (target === null || target.length === 0) {
        return true;
    }
    if(!target.match(/.+@.+\..+/)){
        return false;
    }
    return true;
}
// 日付チェック（message.e00019）
function isDate(target) {
    if (target === null || target.length === 0) {
        return true;
    }
    var date = new Date(target);
    if (isNaN(date.getDate())) {
        return false;
    }
    return true;
}
// ファイルパスチェック（message.e00020）
// 下記の禁止文字をチェック
// ⁄	スラッシュ	パスの区切り記号
// *	アスタリスク	ワイルドカード記号
// ?	疑問符	ワイルドカード記号
// "	2重引用符	パスを囲む
// >	大なり不等号	リダイレクト記号
// <	小なり不等号	リダイレクト記号
// |	縦棒、バー	パイプ記号
// ※　\（円マーク）はパスの区切り記号で使用するため許可
// ※　:（コロン）はドライブ文字記号で使用するため許可
function isFilePath(target) {
    if (target === null || target.length === 0) {
        return '';
    }
    var ngChars = /[\/*?"<>|]/.exec(target);
    if (ngChars !== null) {
        // 入力禁止文字を返却
        return ngChars[0];
    }
    return '';
}
// 改行を<BR>に変換
function newLinesToBR(text) {
    const escapedText = text.replace(/</g, '&lt;').replace(/>/g, '&gt;');
    return escapedText.replace(/\r?\n/g, '<br>');
};

// datepicker
ko.bindingHandlers.datepicker = {
	init: function (element, valueAccessor, allBindingsAccessor) {
		var options = allBindingsAccessor().datepickerOptions || {}, $el = $(element);
		options.autoclose = true;
		options.format = 'yyyy/mm/dd';
		options.language = 'ja';
		options.forceParse = false;
		options.keyboardNavigation = false;
		options.todayHighlight = true;
		$el.datepicker(options);
		ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
			$el.datepicker('destroy');
		});
	},
	 update: function (element, valueAccessor, allBindingsAccessor) {
        $(element).datepicker('setDate', ko.unwrap(valueAccessor()));
	}
};

function flash_TR_element(elem) {
    $(elem).children().addClass("flash").addClass("hightlight");
    setTimeout(function() {
        $(elem).children().removeClass("hightlight");
    }, 50);
}

function flash_CARD_element(elem) {
    $(elem).children().addClass("flash").addClass("hightlight");
    setTimeout(function() {
        $(elem).children().removeClass("hightlight");
    }, 50);
}

const smartPhone = ko.observable(false);

function isSmartPhone() {
    if (document.body.clientWidth < 576) {
        smartPhone(true);
    } else {
        smartPhone(false);
    }
}
$(window).resize(function() {
    isSmartPhone();
});
isSmartPhone();

// cookieから、Spring Security の XSRF-TOKEN の文字列を取り出す
function getCsrfTokenFromCookie() {
    const token = document.cookie
        .split(/;\s*/)
        .find(row => {
            [key, value] = row.split('=')
            return key === 'XSRF-TOKEN';
        });
    if (token) {
        return token.split('=')[1];
    }
    return "";
}
