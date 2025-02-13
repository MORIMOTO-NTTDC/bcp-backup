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
