/* app/AggregatedViewModel */
/* main app ViewModel */

"use strict";
define(["knockout", "viewModels/ListingViewModel"], function (ko, ListingViewModel) {

  var AggregatedViewModel = new function () {
    var self = this;
    // data
    self.listings = ko.observableArray([
      new ListingViewModel('Article 1', 'Today', "Bacon ipsum dolor sit amet turkey biltong chuck corned beef jowl pig ham hock tri-tip ribeye leberkas pork loin. Boudin pork chop biltong filet mignon ground round. Doner filet mignon turducken strip steak. Pig salami strip steak tenderloin biltong short ribs, rump beef ribs shank turkey ground round beef ribeye ham tongue. Ham leberkas prosciutto meatloaf kielbasa."),
      new ListingViewModel('Article 2', 'Yesterday', "Fixie polaroid voluptate flannel aliqua, Banksy reprehenderit vero. Brunch voluptate kale chips, literally kitsch cardigan sint trust fund proident swag nulla mumblecore synth. Art party selvage assumenda, retro pitchfork officia eu Wes Anderson qui trust fund. Veniam +1 leggings flexitarian, Banksy irure twee Etsy you probably haven't heard of them. Nulla asymmetrical officia scenester delectus salvia duis chillwave, chambray Austin fashion axe occupy Bushwick letterpress. Roof party lo-fi freegan, aesthetic 90's ea butcher non tumblr occupy. High Life wayfarers id, artisan delectus church-key Austin pug irure ethical et."),
      new ListingViewModel('Article 3', 'Last Week', "Mauris ac diam libero. Donec tincidunt auctor augue vel dapibus. Quisque dictum ultricies malesuada. Donec tristique iaculis lobortis. Nam ut ante a justo laoreet luctus eget vitae ipsum. Mauris lorem dolor, bibendum quis hendrerit non, volutpat quis urna. Donec bibendum lacus at volutpat pellentesque. Nulla sit amet felis consequat, fermentum metus eleifend, vestibulum lorem. Aliquam erat volutpat. Donec in est at tellus molestie tempor sit amet sed justo. ")
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

  return AggregatedViewModel;

});
