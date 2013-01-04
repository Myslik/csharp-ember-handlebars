/// <reference path="../App.js" />
/// <reference path="../../vendor/ember.js" />

App.Router.map(function (match) {
    match("/").to("home");
    match("/about").to("about");
    match("/contact").to("contact");
});

App.HomeRoute = Em.Route.extend();
App.AboutRoute = Em.Route.extend();
App.ContactRoute = Em.Route.extend();

