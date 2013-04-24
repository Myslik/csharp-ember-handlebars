/// <reference path="../../ember-1.0.0-rc.2.js" />
/// <reference path="../App.js" />

App.Router.map(function () {
    this.resource('app', function () {
        this.route('home'),
        this.route('about'),
        this.route('contact')
    });

    this.resource('shows', function () {

        this.route('add')

        this.resource('show', { path: ':show_id' }, function () {
            this.route('edit')
            this.route('remove')
        })

    })

});