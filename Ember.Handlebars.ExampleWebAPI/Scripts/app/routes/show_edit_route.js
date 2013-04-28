App.ShowEditRoute = Em.Route.extend({
    activate: function () {
        this.controllerFor("application").set("notification", "");
        $(document).attr('title', 'Edit Show');
    },
    model: function () {
        return this.modelFor('show');
    },
    setupController: function (controller, model) {
        controller.set('content', model)
    },
    renderTemplate: function () {
        this.render('show/edit', { into: 'application' });
    },
    events: {

        confirmEdit: function (record) {
            App.store.get('defaultTransaction').commit();
            this.controllerFor('application').set('notification', 'Show has been updated');
            this.transitionTo('shows.index');
        },
        cancelEdit: function (record) {
            App.store.get('defaultTransaction').rollback();
            this.transitionTo('shows.index');
        }

    }

});