(function() {
  'use strict';
  var Client, CurrentUser, REMINDER_TABLENAME, ReminderUrls, Reminders, createCleanUrl, getReminders, handleLogin, handlePageSwitch, localforage, moment, requestSavedReminders, showNotification, storeReminder, syncData, _;

  localforage = require('localforage');

  _ = require('underscore');

  moment = require('moment');

  REMINDER_TABLENAME = 'Item';

  Client = new WindowsAzure.MobileServiceClient(connectionAzure.url, connectionAzure.id);

  CurrentUser = null;

  window.client = Client;

  localforage.setDriver(localforage.LOCALSTORAGE);

  ReminderUrls = null;

  Reminders = [];

  console.log('\'Allo \'Allo! Event Page for Browser Action');

  storeReminder = function(reminder, sendResponse) {
    reminder.user = CurrentUser ? CurrentUser.userId : null;
    reminder.type = 'browser';
    reminder._updatedAt = moment.utc().format();
    return localforage.setItem(reminder.url, reminder, function(err, value) {
      if (!err) {
        ReminderUrls.push(reminder.url);
        sendResponse({
          success: true
        });
        showNotification(reminder.name, 'Successfully saved');
        if (CurrentUser) {
          return client.getTable(REMINDER_TABLENAME).insert(reminder).then(function(value) {
            return console.log('TO THE CLOUD!');
          });
        }
      } else {
        return sendResponse({
          err: err
        });
      }
    });
  };

  showNotification = function(name, message, actionName) {
    var opt;
    opt = {
      type: 'basic',
      title: name + ' [Mind the App]',
      message: message,
      iconUrl: 'https://raw.githubusercontent.com/dkundel/mind-the-app/master/resources/Icon-xxhdpi.png'
    };
    if (actionName) {
      opt.buttons = [
        {
          title: actionName
        }
      ];
    }
    return chrome.notifications.create(name, opt, function(id) {
      return console.log('notification shown');
    });
  };

  requestSavedReminders = function(callback) {
    return localforage.keys(function(err, keys) {
      if (!err) {
        ReminderUrls = keys;
        return typeof callback === "function" ? callback() : void 0;
      } else {
        return console.log('Failed to get reminders');
      }
    });
  };

  getReminders = function() {
    Reminders = [];
    return localforage.iterate(function(value, key) {
      console.log('one more');
      if (value.type) {
        Reminders.push(value);
      }
    }, function() {
      return chrome.runtime.sendMessage({
        action: 'updateReminders',
        reminders: Reminders
      }, function() {
        return console.log('responded');
      });
    });
  };

  handlePageSwitch = function(url, sendResponse) {
    var checkMatches;
    console.log(url);
    url = createCleanUrl(url);
    checkMatches = function() {
      var matches;
      matches = _.filter(ReminderUrls, function(testUrl) {
        return url.indexOf(testUrl) === 0;
      });
      return _.each(matches, function(key) {
        return localforage.getItem(key, function(err, value) {
          if (!err) {
            if (value) {
              if (!value.repeating) {
                showNotification(value.name, value.message);
                return localforage.removeItem(key, function(err) {
                  return console.log(err);
                });
              } else {
                return showNotification(value.name, value.message);
              }
            }
          } else {
            return console.log(err);
          }
        });
      });
    };
    if (ReminderUrls === null) {
      return requestSavedReminders(checkMatches);
    } else {
      return checkMatches();
    }
  };

  createCleanUrl = function(dirtyUrl) {
    var cleanUrl;
    cleanUrl = dirtyUrl;
    return cleanUrl;
  };

  handleLogin = function(user) {
    CurrentUser = user;
    return client.currentUser = user;
  };

  syncData = function() {
    var query;
    if (CurrentUser) {
      console.log('SYNC');
      query = Client.getTable(REMINDER_TABLENAME);
      return localforage.getItem('LASTSYNC', function(err, value) {
        if (!err) {
          if (value) {
            query = query.where(function() {
              return moment(this._updatedAt).isAfter(value);
            });
          }
          return query.where({
            type: 'browser',
            user: CurrentUser.userId
          }).read().done(function(results) {
            _.each(results, function(result) {
              return localforage.setItem(result.url, result, function(err, value) {
                return ReminderUrls.push(result.url);
              });
            });
            return localforage.setItem('LASTSYNC', moment.utc().format(), function(err, value) {
              if (err) {
                return console.log('syncerr', err);
              }
            });
          });
        }
      });
    }
  };

  syncData();

  setInterval(syncData, 30000);

  chrome.runtime.onMessage.addListener(function(request, sender, sendResponse) {
    console.log(arguments);
    if ((request != null ? request.action : void 0) === 'pageTrigger') {
      console.log(ReminderUrls != null ? ReminderUrls.length : void 0);
      handlePageSwitch(sender.url, sendResponse);
    }
    if ((request != null ? request.action : void 0) === 'addReminder') {
      storeReminder(request.reminder, sendResponse);
    }
    if ((request != null ? request.action : void 0) === 'getReminders') {
      getReminders(sendResponse);
    }
    if ((request != null ? request.action : void 0) === 'getClient') {
      sendResponse(client);
    }
    if ((request != null ? request.action : void 0) === 'userLogin') {
      return handleLogin(request.user);
    }
  });

  chrome.notifications.onButtonClicked.addListener(function(notificationId, buttonIdx) {
    if (buttonIdx === 0) {
      return console.log('DISABLE' + notificationId);
    }
  });

  console.log('\'Allo \'Allo! Event Page for Browser Action');

}).call(this);
