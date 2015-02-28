(function() {
  'use strict';
  console.log('\'Allo \'Allo! Content scri!pt');

  chrome.runtime.sendMessage({
    action: 'pageTrigger'
  }, function(response) {
    return console.log(response);
  });

}).call(this);
