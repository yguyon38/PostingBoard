using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PostingBoard.Models
{
    public class Gig
    {

        public int Id { get; set; }
        
        public ApplicationUser Artist { get; set; }
        public DateTime DateTime { get; set; }


        [Required]
        [StringLength(255)]
        public string Venue { get; set; }
       
       
        public Genre Genre { get; set; }
    }
}