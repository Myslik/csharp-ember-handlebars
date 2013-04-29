using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ember.Handlebars.ExampleWebAPI.Models {
    
    public class Show {

        [Key]
        public int ShowId { get; set; }
        [StringLength(50)]
        public string Title { get; set; }
        [StringLength( 50 )]
        public string Category { get; set; }
        [StringLength( 50 )]
        public string Network { get; set; }
        public int Year { get; set; }

    }
}