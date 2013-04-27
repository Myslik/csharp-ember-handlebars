App.ShowRoute = Em.Route.extend( {
    model: function ( params ) {
        return App.Show.find(params.show_id);
    }
} );


App.ShowIndexRoute = Em.Route.extend( {
    activate: function () {
        this.controllerFor("application").set("notification", "");
        $(document).attr('title', 'Show Details');
    },
    model: function (params) {
        return this.modelFor('show');
    },
    setupController: function (controller, model) {
        controller.set( 'content', model );
        console.log( 'a' );
    },
    renderTemplate: function () {
        this.render('show/index', { into: 'application' });
    }
});