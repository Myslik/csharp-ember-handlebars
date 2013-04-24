App.AppRoute = Em.Route.extend({

    activate: function () {
        this.controllerFor("application").set("notification", "");
        $(document).attr('title', 'Home');
    },

});