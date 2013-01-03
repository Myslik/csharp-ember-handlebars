
var compile = function (template) {
    return Ember.Handlebars.precompile(template).toString();
};
