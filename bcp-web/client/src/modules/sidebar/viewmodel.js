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
