App.ShowsAddRoute = Em.Route.extend({
    activate: function() {
        this.controllerFor("application").set("notification", "");
        $(document).attr('title', 'Add Show');
    },
    model: function () {
        store = this.get('store');
        return store.createRecord(App.Show, {});
    },
    setupController: function (controller, model) {
        controller.set('content', model)
    },
    renderTemplate: function () {
        this.render('shows/add', { into: 'application' });
    },
    events: {

        confirmAdd: function (record) {
            store = this.get('store');
            store.get('defaultTransaction').commit();
            this.controllerFor('application').set('notification', 'Show has been added');
            this.transitionTo('shows.index');

        },
        cancelAdd: function (record) {
            store = this.get('store');
            store.get('defaultTransaction').rollback();
            record.destroy();
            this.transitionTo('shows.index');
        }

    }

});