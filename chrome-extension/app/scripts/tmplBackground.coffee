'use strict';

localforage = require('localforage')
_ = require('underscore')

localforage.setDriver localforage.LOCALSTORAGE

ReminderUrls = null
Reminders = []

console.log('\'Allo \'Allo! Event Page for Browser Action')

# END OF TESTING

storeReminder = (reminder, sendResponse) ->
  reminder.user = null
  reminder.repeating = true #TODO!!!

  localforage.setItem reminder.url, reminder, (err, value) ->
    unless err 
      ReminderUrls.push reminder.url
      sendResponse {success: true}
      showNotification reminder.name, 'Successfully saved'
    else
      sendResponse {err}


showNotification = (name, message, actionName) ->
  opt = 
    type: 'basic'
    title: name + ' [Mind the App]'
    message: message
    iconUrl: 'https://raw.githubusercontent.com/dkundel/mind-the-app/master/resources/Icon-xxhdpi.png'

  if actionName
    opt.buttons = [{title: actionName}]

  chrome.notifications.create name, opt, (id) ->
    console.log 'notification shown'


requestSavedReminders = (callback) ->
  localforage.keys (err, keys) ->
    unless err
      ReminderUrls = keys
      callback?()
    else
      console.log('Failed to get reminders') 

getReminders = () ->
  Reminders = []
  localforage.iterate (value, key) ->
    console.log 'one more'
    Reminders.push value
    return
  , () ->
    chrome.runtime.sendMessage {action: 'updateReminders', reminders: Reminders}, () ->
      console.log 'responded'

handlePageSwitch = (url, sendResponse) ->
  console.log url
  url = createCleanUrl url

  checkMatches = () ->
    matches = _.filter ReminderUrls, (testUrl) ->
      return url.indexOf(testUrl) is 0

    _.each matches, (key) ->
      localforage.getItem key, (err, value) ->
        unless err
          if value
            unless value.repeating
              showNotification value.name, value.message
              localforage.removeItem key, (err) ->
                console.log err
            else
              showNotification value.name, value.message, 'Disable'
        else
          console.log err

  if ReminderUrls is null
    requestSavedReminders(checkMatches)
  else
    checkMatches()


createCleanUrl = (dirtyUrl) ->
  cleanUrl = dirtyUrl

  return cleanUrl


requestSavedReminders()


chrome.runtime.onMessage.addListener (request, sender, sendResponse) ->
  console.log arguments
  # console.log("from a content script:" + sender.tab.url)
  if request?.action is 'pageTrigger'
    console.log ReminderUrls?.length
    handlePageSwitch sender.url, sendResponse

  if request?.action is 'addReminder'
    storeReminder request.reminder, sendResponse

  if request?.action is 'getReminders'
    getReminders sendResponse


chrome.notifications.onButtonClicked.addListener (notificationId, buttonIdx) ->
  if buttonIdx is 0
    console.log 'DISABLE' + notificationId

console.log('\'Allo \'Allo! Event Page for Browser Action')