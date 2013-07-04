/* app/viewModels/ListingViewModel */

/* INDIVIDUAL FEED LISTING */

define(["knockout"], function (ko) {

  var ListingViewModel = function (title, date, blurb) {
    this.data = {
      title : title,
      date : date,
      blurb : blurb
    };
    this.expanded = ko.observable(false);
  };
  return ListingViewModel;
});
  
