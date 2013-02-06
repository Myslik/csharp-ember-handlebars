var jQuery = function () { return jQuery };
jQuery.ready = function () { return jQuery };
jQuery.inArray = function () { return jQuery };
jQuery.jquery = "1.8.3";
jQuery.event = { fixHooks: {} };
var document = {
    createRange: false,
    createElement: function () {
        var element = {
            firstChild: function () { return element },
            innerHTML: function () { return element },
            childNodes: ['','','']
        }
        return element;
    }
};
var console = {};
var window = {
    document: document
};
var $ = jQuery;
