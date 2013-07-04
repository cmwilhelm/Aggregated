"use strict";
requirejs.config({
  baseUrl: 'js/libs',
  paths: {
    jquery: ["http://ajax.googleapis.com/ajax/libs/jquery/2.0.2/jquery.min", "jquery"],
    app: '../app',
    viewModels: '../app/viewModels'
  }
});

require(["jquery", "knockout", "viewModels/AggregatedViewModel", "app/bindingHandlers"],
  function ($, ko, AggregatedViewModel) {
    /* INIT */
    ko.applyBindings(AggregatedViewModel);
  });