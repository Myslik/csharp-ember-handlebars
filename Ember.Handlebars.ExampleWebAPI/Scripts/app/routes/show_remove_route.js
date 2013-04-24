App.ShowRemoveRoute = Em.Route.extend({
    activate: function () {
        this.controllerFor("application").set("notification", "");
        $(document).attr('title', 'Remove Show');
    },
    model: function () {
        return this.modelFor('show');
    },
    setupController: function (controller, model) {
        controller.set('content', model)
    },
    renderTemplate: function () {
        this.render('show/remove', { into: 'application' });
    },
    events: {

        confirmRemove: function (record) {
            record.deleteRecord();
            store = this.get('store');
            store.get('defaultTransaction').commit();
            this.controllerFor('application').set('notification', 'Show has been removed');
            this.transitionTo('shows.index');
        }

    }

});