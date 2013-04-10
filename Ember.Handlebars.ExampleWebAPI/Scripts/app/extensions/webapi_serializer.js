//require('ember-data/serializers/json_serializer');
var get = Ember.get;

DS.WebAPISerializer = DS.JSONSerializer.extend({
    keyForAttributeName: function (type, name) {
        // do not do decamelize
        return name;
    },

    extractMany: function (loader, json, type, records) {
        var root = this.rootForType(type);
        root = this.pluralize(root);
        var objects;

        // detect if returned json is Array
        if (json instanceof Array) {
            objects = json;
        }
        else {
            this.sideload(loader, type, json, root);
            this.extractMeta(loader, type, json);
            objects = json[root];
        }

        if (objects) {
            var references = [];
            if (records) { records = records.toArray(); }

            for (var i = 0; i < objects.length; i++) {
                if (records) { loader.updateId(records[i], objects[i]); }
                var reference = this.extractRecordRepresentation(loader, type, objects[i]);
                references.push(reference);
            }

            loader.populateArray(references);
        }
    },

    extract: function (loader, json, type, record) {
        // don't have json[root] in the returned json data
        if (record) loader.updateId(record, json);
        this.extractRecordRepresentation(loader, type, json);
    },

    rootForType: function (type) {
        var typeString = type.toString();

        Ember.assert("Your model must not be anonymous. It was " + type, typeString.charAt(0) !== '(');

        // use the last part of the name as the URL
        var parts = typeString.split(".");
        var name = parts[parts.length - 1];

        // don't do capital case replacement
        return name.toLowerCase();
    },

});
