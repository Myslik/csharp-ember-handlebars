/// <reference path="../vendor/ember.js" />
/// <reference path="views/ApplicationView.js" />
/// <reference path="controllers/ApplicationController.js" />

App = Em.Application.create({

    name: 'Cool App',

    ApplicationController: _appController,
    ApplicationView: _appView

});

