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
