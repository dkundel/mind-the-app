'use strict';

console.log('\'Allo \'Allo! Popup')

client = new WindowsAzure.MobileServiceClient(
  connectionAzure.url, connectionAzure.id
);

REMINDERS = []

document.addEventListener 'polymer-ready', () ->
  # entry1 = 
  #   name: 'Foo'
  #   url: 'http://foo'

  # entry2 = 
  #   name: 'Foo'
  #   url: 'http://foo'
  # document.getElementById('reminderList').data = [entry1, entry2]

  $('#createReminderBtn').click(createReminder)

  $('paper-tabs').children().each((idx) -> 
    $tab = $(@)
    $tab.click (evt) ->
      console.log idx
      document.querySelector('core-pages').selected = idx
      if idx is 1
        getReminders()
  )

  $('.facebookLogin').click(logIn)

  $('#reminderList').on 'click', '.row', (e) ->
    idx = $('#reminderList .row').index(this)
    loadDetailsReminder(idx)


loadDetailsReminder = (idx) ->
  reminder = REMINDERS[idx]
  if (reminder)
    document.getElementById('nameDetails').value = reminder.name
    document.getElementById('urlDetails').value = reminder.url
    document.getElementById('messageDetails').value = reminder.message
    document.getElementById('repeatingDetails').checked = reminder.repeating
    document.querySelector('core-pages').selected = 3


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

logIn = () ->
  client.login('facebook').then(handleLoggedIn, (err) ->
    console.log 'err', err
  )

handleLoggedIn = () ->
  console.log 'logged in'
  console.log arguments

chrome.runtime.onMessage.addListener (request, sender, sendResponse) ->
  if request?.action is 'updateReminders'
    $('#loadingReminders').hide()
    $('#reminderList').show()
    document.getElementById('reminderList').data = request.reminders
    REMINDERS = request.reminders
    $('#reminderCount').html(request.reminders.length)
