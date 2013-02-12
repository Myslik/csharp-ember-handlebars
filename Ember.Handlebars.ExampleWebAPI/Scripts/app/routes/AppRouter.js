/// <reference path="../App.js" />
/// <reference path="../../vendor/ember.js" />

App.Router.map(function () {
    this.route("about", { path: "/about" });
    this.route("contact", { path: "/contact" });
});

App.HomeRoute = Em.Route.extend();
App.AboutRoute = Em.Route.extend();
App.ContactRoute = Em.Route.extend();

