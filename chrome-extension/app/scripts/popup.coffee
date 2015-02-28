'use strict';

console.log('\'Allo \'Allo! Popup')

document.addEventListener 'polymer-ready', () ->
  entry1 = 
    index: 0
    model: 
      name: 'Foo'
      url: 'http://foo'

  entry2 = 
    index: 1
    model: 
      name: 'Foo'
      url: 'http://foo'
  document.getElementById('reminderList').data = [entry1, entry2]

  $('#createReminderBtn').click(createReminder)



createReminder = () ->
  reminder = 
    name: document.getElementById('nameInput')?.value
    url: document.getElementById('urlInput')?.value
    message: document.getElementById('messageInput')?.value

  chrome.runtime.sendMessage({action: 'addReminder', reminder}, (response) ->
    console.log('resp', response)
  )
