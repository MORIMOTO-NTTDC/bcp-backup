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
