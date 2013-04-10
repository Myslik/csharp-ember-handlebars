/// <reference path="../../ember-1.0.0-rc.2.js" />
/// <reference path="../../ember-data.js" />
/// <reference path="../App.js" />
/// <reference path="../extensions/webapi_serializer.js" />
/// <reference path="../extensions/webapi_adapter.js" />

App.Store = DS.Store.extend({
    revision: 12,
    adapter: 'DS.FixtureAdapter'
});