App.ShowsRoute = Em.Route.extend({
    activate: function() {
        $(document).attr('title', 'Shows List');
    },
    model: function () {
        return App.Show.find();
    },
    setupController: function (controller, model) {
        controller.set('content', model)
    },
    renderTemplate: function () {
        this.render('shows/index', { into: 'application' });
    }
});

App.ShowsIndexRoute = App.ShowsRoute.extend();