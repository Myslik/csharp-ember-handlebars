/// <reference path="../../vendor/ember.js" />

var _router = Em.Router.map(function (match) {
    match('/').to('home');
    match('/about').to('about');
});
