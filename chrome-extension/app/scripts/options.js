(function() {
  'use strict';
  var handleLoggedIn, logIn;

  console.log('\'Allo \'Allo! Option');

  window.client = new WindowsAzure.MobileServiceClient(connectionAzure.url, connectionAzure.id);

  $(function() {
    return $('button').click(logIn);
  });

  handleLoggedIn = function(currentUser) {
    console.log(arguments);
    return window.postMessage({
      type: "FBLOGIN",
      user: currentUser
    }, "*");
  };

  logIn = function() {
    alert('foo');
    return client.login('facebook').then(handleLoggedIn, function(err) {
      return console.log('err', err);
    });
  };

}).call(this);
