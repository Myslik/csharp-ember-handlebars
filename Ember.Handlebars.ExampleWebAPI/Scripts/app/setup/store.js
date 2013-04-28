/// <reference path="../../ember-1.0.0-rc.2.js" />
/// <reference path="../../ember-data.js" />
/// <reference path="../App.js" />
/// <reference path="../extensions/webapi_serializer.js" />
/// <reference path="../extensions/webapi_adapter.js" />

var adapter = DS.WebAPIAdapter.create( {
    namespace: "api",
    bulkCommit: false,
    antiForgeryTokenSelector: "#antiForgeryToken"
} );

var serializer = Ember.get( adapter, 'serializer' );

serializer.configure( 'App.Show', {
    sideloadAs: 'shows',
    primaryKey: 'showId'
} );

App.store = DS.Store.create( {
    adapter: adapter,
    revision: 12
} );