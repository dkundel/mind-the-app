(function() {
  'use strict';
  var createReminder;

  console.log('\'Allo \'Allo! Popup');

  document.addEventListener('polymer-ready', function() {
    var entry1, entry2;
    entry1 = {
      index: 0,
      model: {
        name: 'Foo',
        url: 'http://foo'
      }
    };
    entry2 = {
      index: 1,
      model: {
        name: 'Foo',
        url: 'http://foo'
      }
    };
    document.getElementById('reminderList').data = [entry1, entry2];
    return $('#createReminderBtn').click(createReminder);
  });

  createReminder = function() {
    var reminder, _ref, _ref1, _ref2;
    reminder = {
      name: (_ref = document.getElementById('nameInput')) != null ? _ref.value : void 0,
      url: (_ref1 = document.getElementById('urlInput')) != null ? _ref1.value : void 0,
      message: (_ref2 = document.getElementById('messageInput')) != null ? _ref2.value : void 0
    };
    return chrome.runtime.sendMessage({
      action: 'addReminder',
      reminder: reminder
    }, function(response) {
      return console.log('resp', response);
    });
  };

}).call(this);
