'use strict';

console.log('\'Allo \'Allo! Option')

window.client = new WindowsAzure.MobileServiceClient(
  connectionAzure.url, connectionAzure.id
);

$(() ->
  $('button').click(logIn)
)

handleLoggedIn = (currentUser) ->
  console.log arguments
  window.postMessage({ type: "FBLOGIN", user: currentUser }, "*")

logIn = () ->
  client.login('facebook').then(handleLoggedIn, (err) ->
    console.log 'err', err
  )