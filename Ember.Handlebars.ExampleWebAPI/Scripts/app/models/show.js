/// <reference path="../../ember-1.0.0-rc.2.min.js" />
/// <reference path="../../ember-data.js" />
/// <reference path="../app.js" />

App.Show = DS.Model.extend({
    title: DS.attr('string'),
    network: DS.attr('string'),
    category: DS.attr('string'),
    year: DS.attr('number')    
});