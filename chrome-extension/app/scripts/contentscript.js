(function() {
  'use strict';
  console.log('\'Allo \'Allo! Content scri!pt');

  chrome.runtime.sendMessage({
    action: 'pageTrigger'
  }, function(response) {
    return console.log(response);
  });

  window.addEventListener('message', function(event) {
    var _ref;
    if (event.source !== window) {
      return;
    }
    if (((_ref = event.data) != null ? _ref.type : void 0) === "FBLOGIN") {
      return chrome.runtime.sendMessage({
        action: 'userLogin',
        user: event.data.user
      });
    }
  }, false);

}).call(this);
