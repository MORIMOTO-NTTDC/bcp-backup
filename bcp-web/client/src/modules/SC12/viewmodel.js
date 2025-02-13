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
