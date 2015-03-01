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

  $('.updateButton').click(update)

  $('#reminderList').on 'click', '.row', (e) ->
    idx = $('#reminderList .row').index(this)
    loadDetailsReminder(idx)

  document.getElementById('conditionalInput').onchange = (e) ->
    unless document.getElementById('conditionalInput').checked
      document.getElementById('fromInput').addAttribute('disabled', true)
      document.getElementById('toInput').addAttribute('disabled', true)
    else
      document.getElementById('fromInput').removeAttribute('disabled')
      document.getElementById('toInput').removeAttribute('disabled')

  chrome.runtime.sendMessage({action: 'userStatus'}, (response) ->
    console.log 'resp', response
    if response is not null
      $('#loginDialog').hide()
      $('#countDialog').show()
  )


update = () ->
  reminder = 
    name: document.getElementById('nameDetails')?.value
    trigger: document.getElementById('urlDetails')?.value
    message: document.getElementById('messageDetails')?.value
    repeating: document.getElementById('repeatingDetails')?.checked

  chrome.runtime.sendMessage({action: 'updateReminder', reminder}, (response) ->
    console.log('resp', response)
  )

loadDetailsReminder = (idx) ->
  reminder = REMINDERS[idx]
  if (reminder)
    document.getElementById('nameDetails').value = reminder.name
    document.getElementById('urlDetails').value = reminder.trigger
    document.getElementById('messageDetails').value = reminder.message
    document.getElementById('repeatingDetails').checked = reminder.repeating
    document.querySelector('core-pages').selected = 3


createReminder = () ->
  reminder = 
    name: document.getElementById('nameInput')?.value
    trigger: document.getElementById('urlInput')?.value
    message: document.getElementById('messageInput')?.value
    repeating: document.getElementById('repeatingInput')?.checked
  
  if document.getElementById('conditionalInput')?.checked
    reminder.conditions = 
      from: document.getElementById('fromInput')?.value
      to: document.getElementById('toInput')?.value

  reminder.conditions = JSON.stringify(reminder.conditions)

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
  tabsSetting = 
    url: 'http://localhost:8000/app/options.html'

  chrome.tabs.create tabsSetting, (tab) ->
    console.log tab

chrome.runtime.onMessage.addListener (request, sender, sendResponse) ->
  if request?.action is 'updateReminders'
    $('#loadingReminders').hide()
    $('#reminderList').show()
    document.getElementById('reminderList').data = request.reminders
    REMINDERS = request.reminders
    $('#reminderCount').html(request.reminders.length)

  if request?.action is 'userLogin'
    handleLogin request.user
