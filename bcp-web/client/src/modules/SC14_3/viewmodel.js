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
