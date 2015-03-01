'use strict';

localforage = require('localforage')
_ = require('underscore')
moment = require('moment')

REMINDER_TABLENAME = 'Item'

Client = new WindowsAzure.MobileServiceClient(
  connectionAzure.url, connectionAzure.id
);

CurrentUser = null

window.client = Client

localforage.setDriver localforage.LOCALSTORAGE

ReminderUrls = null
Reminders = []

console.log('\'Allo \'Allo! Event Page for Browser Action')

# END OF TESTING

storeReminder = (reminder, sendResponse) ->
  reminder.user = if CurrentUser then CurrentUser.userId else null
  reminder.type = 'browser'
  reminder._updatedAt = moment.utc().format()

  localforage.setItem reminder.url, reminder, (err, value) ->
    unless err 
      ReminderUrls.push reminder.url
      sendResponse {success: true}
      showNotification reminder.name, 'Successfully saved'
      if CurrentUser
        client.getTable(REMINDER_TABLENAME).insert(reminder).then (value) ->
          console.log 'TO THE CLOUD!'
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
    if value.type
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
              showNotification value.name, value.message
        else
          console.log err

  if ReminderUrls is null
    requestSavedReminders(checkMatches)
  else
    checkMatches()


createCleanUrl = (dirtyUrl) ->
  cleanUrl = dirtyUrl

  return cleanUrl


handleLogin = (user) ->
  CurrentUser = user 
  client.currentUser = user


syncData = () ->
  if CurrentUser
    console.log 'SYNC'
    query = Client.getTable REMINDER_TABLENAME
    localforage.getItem 'LASTSYNC', (err, value) ->
      unless err
        if value
          query = query.where () ->
            return moment(this._updatedAt).isAfter(value)
        query.where(
          type: 'browser'
          user: CurrentUser.userId
        ).read().done((results) ->
          _.each(results, (result) ->
            localforage.setItem result.url, result, (err, value) ->
              ReminderUrls.push(result.url)
          )
          localforage.setItem 'LASTSYNC', moment.utc().format(), (err, value) ->
            if err
              console.log 'syncerr', err
        )



syncData()
setInterval syncData, 30000


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

  if request?.action is 'getClient'
    sendResponse(client)

  if request?.action is 'userLogin'
    handleLogin request.user


chrome.notifications.onButtonClicked.addListener (notificationId, buttonIdx) ->
  if buttonIdx is 0
    console.log 'DISABLE' + notificationId

console.log('\'Allo \'Allo! Event Page for Browser Action')