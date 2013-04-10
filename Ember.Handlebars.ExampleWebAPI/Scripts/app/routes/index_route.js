App.IndexRoute = Em.Route.extend({
    redirect: function () {
        this.transitionTo('app.home');
    }
});