App.ShowsAddRoute = Em.Route.extend({
    activate: function() {
        this.controllerFor("application").set("notification", "");
        $(document).attr('title', 'Add Show');
    },
    model: function () {
        return App.store.get('defaultTransaction').createRecord( App.Show, {} );
    },
    setupController: function ( controller, model ) {
        controller.set('content', model)
    },
    renderTemplate: function () {
        this.render('shows/add', { into: 'application' });
    },
    events: {

        confirmAdd: function (record) {
            App.store.get('defaultTransaction').commit();
            this.controllerFor('application').set('notification', 'Show has been added');
            this.transitionTo('shows.index');

        },
        cancelAdd: function (record) {
            App.store.get('defaultTransaction').rollback();
            record.destroy();
            this.transitionTo('shows.index');
        }

    }

});