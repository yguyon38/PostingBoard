using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PostingBoard.Models;
using System.ComponentModel.DataAnnotations;

namespace PostingBoard.ViewModels
{
    public class PostingBoardViewModel
    {
        [Required]
        public string Venue { get; set; }

        [Required]
        [FutureDate]
        public string Date { get; set; }

        [Required]
        [ValidTime]
        public string Time { get; set; }
        public byte Genre { get; set; }
        public IEnumerable<Genre> Genres { get; set; }

        public DateTime GetDateTime()
        {
            return DateTime.Parse(string.Format("{0} {1}", Date, Time));
        }
    }
}