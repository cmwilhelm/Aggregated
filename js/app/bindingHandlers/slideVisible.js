/* app/bindingHandlers/slideVisible */

/* VISIBILITY TOGGLE WITH SLIDE IN/OUT EFFECT */

"use strict";
define(["jquery", "knockout"], function ($, ko) {

  // SLIDING VISIBILITY TOGGLE
  ko.bindingHandlers.slideVisible = {
    init : function (element) {
      $(element).hide();
    },
    update : function (element, valueAccessor) {
      // On update, slide up/down
      $(element)['slide' + (valueAccessor() ? 'Down' : 'Up')]();
    }
  };
  
});
