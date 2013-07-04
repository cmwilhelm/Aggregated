/* app/bindingHandlers */
/* DEFINES CUSTOM KO BINDING HANDLERS FOR THE APP */

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
