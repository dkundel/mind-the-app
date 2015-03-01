'use strict';

console.log('\'Allo \'Allo! Content scri!pt')

chrome.runtime.sendMessage {action: 'pageTrigger'}, (response) ->
  console.log(response)

window.addEventListener 'message', (event) ->
  if event.source != window
    return

  if event.data?.type is "FBLOGIN"
    chrome.runtime.sendMessage {action: 'userLogin', user: event.data.user}
, false


