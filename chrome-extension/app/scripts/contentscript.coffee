'use strict';

console.log('\'Allo \'Allo! Content scri!pt')

chrome.runtime.sendMessage {action: 'pageTrigger'}, (response) ->
  console.log(response)