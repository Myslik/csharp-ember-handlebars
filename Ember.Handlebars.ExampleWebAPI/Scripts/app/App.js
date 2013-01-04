/// <reference path="../vendor/ember.js" />
/// <reference path="views/ApplicationView.js" />
/// <reference path="controllers/ApplicationController.js" />
/// <reference path="views/NavBarView.js" />
/// <reference path="routes/AppRouter.js" />
/// <reference path="views/HomeView.js" />

App = Em.Application.create({
    autoinit: false,
    name: 'Cool App',
    author: 'Your name here',

    ApplicationController: _appController,
    ApplicationView: _appView,

    NavbarView: _navbarView,

    HomeView: _homeView,
    AboutView: _aboutView,
    ContactView: _contactView

});

App.Router.map(function (match) {
    match("/").to("home");
    match("/about").to("about");
    match("/contact").to("contact");
});

App.HomeRoute = Em.Route.extend();
App.AboutRoute = Em.Route.extend();
App.ContactRoute = Em.Route.extend();

App.initialize();

