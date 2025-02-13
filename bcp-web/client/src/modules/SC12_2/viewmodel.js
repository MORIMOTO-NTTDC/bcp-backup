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
