(function() {
  'use strict';
  var createReminder, getReminders;

  console.log('\'Allo \'Allo! Popup');

  document.addEventListener('polymer-ready', function() {
    getReminders();
    $('#createReminderBtn').click(createReminder);
    return $('paper-tabs').children().each(function(idx) {
      var $tab;
      $tab = $(this);
      return $tab.click(function(evt) {
        console.log(idx);
        return document.querySelector('core-pages').selected = idx;
      });
    });
  });

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

  chrome.runtime.onMessage.addListener(function(request, sender, sendResponse) {
    if ((request != null ? request.action : void 0) === 'updateReminders') {
      $('#loadingReminders').hide();
      $('#reminderList').show();
      return document.getElementById('reminderList').data = request.reminders;
    }
  });

}).call(this);
