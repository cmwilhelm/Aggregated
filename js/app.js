"use strict";

(function () {

  /* CUSTOM BINDING HANDLERS */
  ko.bindingHandlers.slideVisible = {
    init : function (element) {
      jQuery(element).hide();
    },
    update : function (element, valueAccessor) {
      // On update, slide up/down
      jQuery(element)['slide' + (valueAccessor() ? 'Down' : 'Up')]();
    }
  };
  
  /* MODELS */
  var ListingModel = function (title, date, blurb) {
    this.title = title;
    this.date = date;
    this.blurb = blurb;
    this.expanded = ko.observable(false);
  };
  
  /* VIEWMODELS */
  var AggregatedViewModel = new function () {
    var self = this;
    // data
    self.listings = ko.observableArray([
      new ListingModel('Article 1', 'Today', "Bacon ipsum dolor sit amet turkey biltong chuck corned beef jowl pig ham hock tri-tip ribeye leberkas pork loin. Boudin pork chop biltong filet mignon ground round. Doner filet mignon turducken strip steak. Pig salami strip steak tenderloin biltong short ribs, rump beef ribs shank turkey ground round beef ribeye ham tongue. Ham leberkas prosciutto meatloaf kielbasa."),
      new ListingModel('Article 2', 'Yesterday', "Fixie polaroid voluptate flannel aliqua, Banksy reprehenderit vero. Brunch voluptate kale chips, literally kitsch cardigan sint trust fund proident swag nulla mumblecore synth. Art party selvage assumenda, retro pitchfork officia eu Wes Anderson qui trust fund. Veniam +1 leggings flexitarian, Banksy irure twee Etsy you probably haven't heard of them. Nulla asymmetrical officia scenester delectus salvia duis chillwave, chambray Austin fashion axe occupy Bushwick letterpress. Roof party lo-fi freegan, aesthetic 90's ea butcher non tumblr occupy. High Life wayfarers id, artisan delectus church-key Austin pug irure ethical et.")
    ]);
    // utilities
    self.toggleExpand = function () {
      var listings = self.listings();
      for (var i = 0; i < listings.length; i += 1) {
        var current = listings[i];
        if (current === this) {
          current.expanded(!current.expanded());
        } else {
          current.expanded(false);
        }
      }
    };
  };
  
  /* INIT */
  ko.applyBindings(AggregatedViewModel);

}) ();