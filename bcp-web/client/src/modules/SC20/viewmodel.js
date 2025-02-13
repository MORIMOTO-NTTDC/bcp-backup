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
