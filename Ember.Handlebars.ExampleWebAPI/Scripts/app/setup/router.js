/// <reference path="../../ember-1.0.0-rc.2.js" />
/// <reference path="../App.js" />

App.Router.map(function () {
    this.resource('app', function () {
        this.route('home'),
        this.route('about'),
        this.route('contact')
    });
});