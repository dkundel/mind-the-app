'use strict';

console.log('\'Allo \'Allo! Popup')

document.addEventListener 'polymer-ready', () ->
  # entry1 = 
  #   name: 'Foo'
  #   url: 'http://foo'

  # entry2 = 
  #   name: 'Foo'
  #   url: 'http://foo'
  # document.getElementById('reminderList').data = [entry1, entry2]

  getReminders()

  $('#createReminderBtn').click(createReminder)

  $('paper-tabs').children().each((idx) -> 
    $tab = $(@)
    $tab.click (evt) ->
      console.log idx
      document.querySelector('core-pages').selected = idx
  )



createReminder = () ->
  reminder = 
    name: document.getElementById('nameInput')?.value
    url: document.getElementById('urlInput')?.value
    message: document.getElementById('messageInput')?.value
    repeating: document.getElementById('repeatingInput')?.checked

  chrome.runtime.sendMessage({action: 'addReminder', reminder}, (response) ->
    console.log('resp', response)
  )

getReminders = () ->
  $('#reminderList').hide()
  $('#loadingReminders').show()
  chrome.runtime.sendMessage({action: 'getReminders'}, (response) ->
    console.log 'Sent'
  )

chrome.runtime.onMessage.addListener (request, sender, sendResponse) ->
  if request?.action is 'updateReminders'
    $('#loadingReminders').hide()
    $('#reminderList').show()
    document.getElementById('reminderList').data = request.reminders
