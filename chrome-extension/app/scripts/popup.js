(function() {
  'use strict';
  var REMINDERS, client, createReminder, getReminders, handleLoggedIn, loadDetailsReminder, logIn;

  console.log('\'Allo \'Allo! Popup');

  client = new WindowsAzure.MobileServiceClient(connectionAzure.url, connectionAzure.id);

  REMINDERS = [];

  document.addEventListener('polymer-ready', function() {
    $('#createReminderBtn').click(createReminder);
    $('paper-tabs').children().each(function(idx) {
      var $tab;
      $tab = $(this);
      return $tab.click(function(evt) {
        console.log(idx);
        document.querySelector('core-pages').selected = idx;
        if (idx === 1) {
          return getReminders();
        }
      });
    });
    $('.facebookLogin').click(logIn);
    return $('#reminderList').on('click', '.row', function(e) {
      var idx;
      idx = $('#reminderList .row').index(this);
      return loadDetailsReminder(idx);
    });
  });

  loadDetailsReminder = function(idx) {
    var reminder;
    reminder = REMINDERS[idx];
    if (reminder) {
      document.getElementById('nameDetails').value = reminder.name;
      document.getElementById('urlDetails').value = reminder.url;
      document.getElementById('messageDetails').value = reminder.message;
      document.getElementById('repeatingDetails').checked = reminder.repeating;
      return document.querySelector('core-pages').selected = 3;
    }
  };

  createReminder = function() {
    var reminder, _ref, _ref1, _ref2, _ref3;
    reminder = {
      name: (_ref = document.getElementById('nameInput')) != null ? _ref.value : void 0,
      url: (_ref1 = document.getElementById('urlInput')) != null ? _ref1.value : void 0,
      message: (_ref2 = document.getElementById('messageInput')) != null ? _ref2.value : void 0,
      repeating: (_ref3 = document.getElementById('repeatingInput')) != null ? _ref3.checked : void 0
    };
    return chrome.runtime.sendMessage({
      action: 'addReminder',
      reminder: reminder
    }, function(response) {
      return console.log('resp', response);
    });
  };

  getReminders = function() {
    $('#reminderList').hide();
    $('#loadingReminders').show();
    return chrome.runtime.sendMessage({
      action: 'getReminders'
    }, function(response) {
      return console.log('Sent');
    });
  };

  logIn = function() {
    return client.login('facebook').then(handleLoggedIn, function(err) {
      return console.log('err', err);
    });
  };

  handleLoggedIn = function() {
    console.log('logged in');
    return console.log(arguments);
  };

  chrome.runtime.onMessage.addListener(function(request, sender, sendResponse) {
    if ((request != null ? request.action : void 0) === 'updateReminders') {
      $('#loadingReminders').hide();
      $('#reminderList').show();
      document.getElementById('reminderList').data = request.reminders;
      REMINDERS = request.reminders;
      return $('#reminderCount').html(request.reminders.length);
    }
  });

}).call(this);
