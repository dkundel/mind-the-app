(function() {
  'use strict';
  var REMINDERS, client, createReminder, getReminders, loadDetailsReminder, logIn, update;

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
    $('.updateButton').click(update);
    $('#reminderList').on('click', '.row', function(e) {
      var idx;
      idx = $('#reminderList .row').index(this);
      return loadDetailsReminder(idx);
    });
    document.getElementById('conditionalInput').onchange = function(e) {
      if (!document.getElementById('conditionalInput').checked) {
        document.getElementById('fromInput').addAttribute('disabled', true);
        return document.getElementById('toInput').addAttribute('disabled', true);
      } else {
        document.getElementById('fromInput').removeAttribute('disabled');
        return document.getElementById('toInput').removeAttribute('disabled');
      }
    };
    return chrome.runtime.sendMessage({
      action: 'userStatus'
    }, function(response) {
      console.log('resp', response);
      if (response === !null) {
        $('#loginDialog').hide();
        return $('#countDialog').show();
      }
    });
  });

  update = function() {
    var reminder, _ref, _ref1, _ref2, _ref3;
    reminder = {
      name: (_ref = document.getElementById('nameDetails')) != null ? _ref.value : void 0,
      trigger: (_ref1 = document.getElementById('urlDetails')) != null ? _ref1.value : void 0,
      message: (_ref2 = document.getElementById('messageDetails')) != null ? _ref2.value : void 0,
      repeating: (_ref3 = document.getElementById('repeatingDetails')) != null ? _ref3.checked : void 0
    };
    return chrome.runtime.sendMessage({
      action: 'updateReminder',
      reminder: reminder
    }, function(response) {
      return console.log('resp', response);
    });
  };

  loadDetailsReminder = function(idx) {
    var reminder;
    reminder = REMINDERS[idx];
    if (reminder) {
      document.getElementById('nameDetails').value = reminder.name;
      document.getElementById('urlDetails').value = reminder.trigger;
      document.getElementById('messageDetails').value = reminder.message;
      document.getElementById('repeatingDetails').checked = reminder.repeating;
      return document.querySelector('core-pages').selected = 3;
    }
  };

  createReminder = function() {
    var reminder, _ref, _ref1, _ref2, _ref3, _ref4, _ref5, _ref6;
    reminder = {
      name: (_ref = document.getElementById('nameInput')) != null ? _ref.value : void 0,
      trigger: (_ref1 = document.getElementById('urlInput')) != null ? _ref1.value : void 0,
      message: (_ref2 = document.getElementById('messageInput')) != null ? _ref2.value : void 0,
      repeating: (_ref3 = document.getElementById('repeatingInput')) != null ? _ref3.checked : void 0
    };
    if ((_ref4 = document.getElementById('conditionalInput')) != null ? _ref4.checked : void 0) {
      reminder.conditions = {
        from: (_ref5 = document.getElementById('fromInput')) != null ? _ref5.value : void 0,
        to: (_ref6 = document.getElementById('toInput')) != null ? _ref6.value : void 0
      };
    }
    reminder.conditions = JSON.stringify(reminder.conditions);
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
    var tabsSetting;
    tabsSetting = {
      url: 'http://localhost:8000/app/options.html'
    };
    return chrome.tabs.create(tabsSetting, function(tab) {
      return console.log(tab);
    });
  };

  chrome.runtime.onMessage.addListener(function(request, sender, sendResponse) {
    if ((request != null ? request.action : void 0) === 'updateReminders') {
      $('#loadingReminders').hide();
      $('#reminderList').show();
      document.getElementById('reminderList').data = request.reminders;
      REMINDERS = request.reminders;
      $('#reminderCount').html(request.reminders.length);
    }
    if ((request != null ? request.action : void 0) === 'userLogin') {
      return handleLogin(request.user);
    }
  });

}).call(this);
