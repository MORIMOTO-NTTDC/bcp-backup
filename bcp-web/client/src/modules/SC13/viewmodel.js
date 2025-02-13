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
