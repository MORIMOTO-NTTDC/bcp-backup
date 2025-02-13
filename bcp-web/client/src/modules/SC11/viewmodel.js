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
