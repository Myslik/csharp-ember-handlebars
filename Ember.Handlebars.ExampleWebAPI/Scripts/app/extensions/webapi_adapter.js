//require("ember-data/core");
//require('ember-data/system/adapter');
//require('ember-data/adapater/rest_adapter');
//require('ember-data/serializers/webapi_serializer');
/*global jQuery*/

var get = Ember.get;

/**
  The WebAPI adapter allows your store to communicate with a REST server 
  powered by ASP.NET MVC WebAPI. 

  This adapter is designed to work with ASP.NET MVC WebAPI which is open data compatible.

  ## JSON Structure

  The WebAPI adapter expects the JSON returned from your server to follow
  ASP.NET WebAPI conventions.

  ### Object Root should not present

  The JSON payload should be an object that contains the record without a
  root property. For example, in response to a `GET` request for
  `/posts/1`, the JSON should look like this:

  ```js
  {
    title: "I'm Running to Reform the W3C's Tag",
    author: "Yehuda Katz"
  }
  ```

  ### No Conventional Names required

  Attribute names in your JSON payload should not be the underscored versions of
  the attributes in your Ember.js models.

  For example, if you have a `Person` model:

  ```js
  App.Person = DS.Model.extend({
    firstName: DS.attr('string'),
    lastName: DS.attr('string'),
    occupation: DS.attr('string')
  });
  ```

  The JSON returned should look like this:

  ```js
  {
    "firstName": "Barack",
    "lastName": "Obama",
    "occupation": "President"
  }
  ```
  
  ## Major differences with default RESTAdapter implementation
  
  ### Parent updates do not handle children updates by default
  
  ### Create a new object, JSON does not send primary key field
    
  ### Delete object, ignore the returned object if it has the same primary key as the object we passed
      Delete parent does not dirty the children.

  ### Update object, handle failure via commit and set error attribute, in order to prevent future failures

  ### if antiForgeryTokenSelector is defined, will include it in the ajax request sent

  ## Usage
  
  ```js
    window.App = Em.Application.create();

    DS.WebAPIAdapter.map('App.TodoList', {
        // Web API server may not handling reference update/delete, so use "load" instead of "always"
        todos: { embedded: 'load' } 
    });

    var adapter = DS.WebAPIAdapter.create({
        namespace: "api",
        bulkCommit: false,
        antiForgeryTokenSelector: "#antiForgeryToken"
    });

    var serializer = Ember.get(adapter, 'serializer');
    serializer.configure('App.TodoList', {
        sideloadAs: "todoList",
        primaryKey: "todoListId"
    });
    serializer.configure('App.Todo', {
        sideloadAs: "todo",
        primaryKey: "todoItemId"
    });

    App.store = DS.Store.create({
        adapter: adapter,
        revision: 11
    });
  ```
*/
DS.WebAPIAdapter = DS.RESTAdapter.extend({
    serializer: DS.WebAPISerializer,
    antiForgeryTokenSelector: null,

    shouldSave: function (record) {
        // By default Web API doesn't handle children update from parent.
        return true;
    },

    // Delete parent records does not dirty the children records
    dirtyRecordsForBelongsToChange: null,

    createRecord: function (store, type, record) {
        var root = this.rootForType(type);

        var data = this.serialize(record, { includeId: false });

        // need to remove the primaryKey field
        var config = get(this, 'serializer').configurationForType(type),
            primaryKey = config && config.primaryKey;

        if (primaryKey) {
            delete data[primaryKey];
        }

        this.ajax(this.buildURL(root), "POST", {
            data: data,
            context: this,
            success: function (json) {
                Ember.run(this, function () {
                    this.didCreateRecord(store, type, record, json);
                });
            },
            error: function (xhr) {
                this.didError(store, type, record, xhr);
            }
        });
    },

    updateRecord: function (store, type, record) {
        var id = get(record, 'id');
        var root = this.rootForType(type);

        data = this.serialize(record, { includeId: true });

        this.ajax(this.buildURL(root, id), "PUT", {
            data: data,
            context: this,
            success: function (json) {
                Ember.run(this, function () {
                    this.didSaveRecord(store, type, record, json);
                });
                record.set("error", "");
            },
            error: function (xhr) {
                // Act on client side as if it is successful, then set model's error attribute.
                // This ensures an erroneous object does not cause the future commits to fail
                Ember.run(this, function () {
                    this.didSaveRecord(store, type, record);
                });

                record.set("error", "Server update failed");
            }
        }, "text");
    },

    deleteRecord: function (store, type, record) {
        var id = get(record, 'id');
        var root = this.rootForType(type);

        var config = get(this, 'serializer').configurationForType(type),
            primaryKey = config && config.primaryKey;

        this.ajax(this.buildURL(root, id), "DELETE", {
            context: this,
            success: function (json) {
                Ember.run(this, function () {
                    if (json[primaryKey] == id) {
                        // webAPI delete will just return the original record, shouldn't save it back
                        // ignore the returned json object
                        this.didSaveRecord(store, type, record);
                    }
                    else {
                        this.didSaveRecord(store, type, record, json);
                    }
                });
            }
        });
    },

    ajax: function (url, type, hash, dataType) {
        hash.url = url;
        hash.type = type;
        hash.dataType = dataType || 'json';
        hash.contentType = 'application/json; charset=utf-8';
        hash.context = this;

        if (hash.data && type !== 'GET') {
            hash.data = JSON.stringify(hash.data);
        }

        // if antiForgeryTokenSelector attribute exists, pass it in the hearder
        var antiForgeryTokenElemSelector = get(this, 'antiForgeryTokenSelector');
        if (antiForgeryTokenElemSelector) {
            var antiForgeryToken = $(antiForgeryTokenElemSelector).val();
            if (antiForgeryToken) {
                hash.headers = {
                    'RequestVerificationToken': antiForgeryToken
                }
            }
        }

        jQuery.ajax(hash);
    },

    pluralize: function (string) {
        return string;
    },

});
