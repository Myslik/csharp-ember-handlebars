App.ShowRoute = Em.Route.extend({
    activate: function () {
        this.controllerFor("application").set("notification", "");
        $(document).attr('title', 'Show Details');
    },
    model: function (params) {
        return App.Show.find(params.show_id);
    },
    setupController: function (controller, model) {
        controller.set('content', model);
    },
    renderTemplate: function () {
        this.render('show', { into: 'application' });
    }

});