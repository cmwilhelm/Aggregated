/* app/ListingViewModel */
/* INDIVIDUAL FEED LISTING */

"use strict";
define(["knockout"], function (ko) {

  var ListingViewModel = function (title, date, blurb) {
    var self = this;
    self.data = {
      title : title,
      date : date,
      blurb : blurb
    };
    self.expanded = ko.observable(false);
  };
  return ListingViewModel;
});
  
