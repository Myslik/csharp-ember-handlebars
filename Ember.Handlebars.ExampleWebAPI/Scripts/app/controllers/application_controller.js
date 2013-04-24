App.ApplicationController = Em.Controller.extend({

    notification: '',
    hasNotification: function () {
        return this.get('notification').length > 0;
    }.property('notification')

});