Project

enable-migrations - indiv
add-migration InitialModel
update-database

Create the following Domain Class

..public class Gig
    {
        public int Id { get; set; }
        public ApplicationUser Artist { get; set; }
        public DateTime DateTime { get; set; }      
        [Required]
        [StringLength(255)]
        public string Venue { get; set; }
        public Genre Genre { get; set; }
    }

..public class Genre  - add-migration
    {
        public byte Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
    }


 public class Attendance
    {

        public Gig Gig { get; set; }
        public ApplicationUser Attendee { get; set; }

        [Key]
        [Column(Order = 1)]
        public int GigId { get; set; }

        [Key]
        [Column(Order = 2)]
        public string AttendeeId { get; set; }

    }


..Application  DBSET - Add Following Code - add-migration

  public DbSet<Gig> Gigs { get; set; }
  public DbSet<Genre> Genres { get; set; }


   public DbSet<Attendance> Attendences { get; set; }
 

..Populating Database

  Sql("INSERT INTO Genres (Id, Name) VALUES (1, 'Jazz')");
  Sql("INSERT INTO Genres (Id, Name) VALUES (2, 'Blues')");
  Sql("INSERT INTO Genres (Id, Name) VALUES (3, 'Rock')");
  Sql("INSERT INTO Genres (Id, Name) VALUES (4, 'Country')");


..Create a view Model ViewModels
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

..Create Gig Controller - Change link on Navigation

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PostingBoard.Models;
using PostingBoard.ViewModels;
using Microsoft.AspNet.Identity;


public class PostingBoardController : Controller
    {
        private ApplicationDbContext _context;
        public PostingBoardController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: PostingBoard
        [Authorize]
        public ActionResult Create()
        {
            var viewModel = new PostingBoardViewModel
            {
                Genres = _context.Genres.ToList()
            };

            return View(viewModel);
        }
        [Authorize]
        [HttpPost]
        public ActionResult Create(PostingBoardViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Genres = _context.Genres.ToList();
                return View("Create", viewModel);
            }

            var artistId = User.Identity.GetUserId();

            var artist = _context.Users.Single(u => u.Id == artistId);
            var genre = _context.Genres.Single(g => g.Id == viewModel.Genre);

            var gid = new Gig
            {
                Artist = artist,
                DateTime = viewModel.GetDateTime(),
                Genre = genre,
                Venue = viewModel.Venue
            };

            _context.Gigs.Add(gid);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }

..Then create a View
@model PostingBoard.ViewModels.PostingBoardViewModel


@{
    ViewBag.Title = "Create";
    Layout = "~/Views/Shared/_Layout.cshtml";
}

<h2>Create</h2>
<p class="alert alert-info">All field are <strong>required</strong></p>

@using (Html.BeginForm("Create", "PostingBoard"))
{
    <div class="form-group">
        @Html.LabelFor(m => m.Venue)
        @Html.TextBoxFor(m => m.Venue, new { @class = "form-control" })
        @Html.ValidationMessageFor(m => m.Venue)
    </div>
    <div class="form-group">
        @Html.LabelFor(m => m.Date)
        @Html.TextBoxFor(m => m.Date, new { @class = "form-control" })
        @Html.ValidationMessageFor(m => m.Date)
    </div>
    <div class="form-group">
        @Html.LabelFor(m => m.Time)
        @Html.TextBoxFor(m => m.Time, new { @class = "form-control" })
        @Html.ValidationMessageFor(m => m.Time)
    </div>
    <div class="form-group">
        @Html.LabelFor(m => m.Genre)
        @Html.DropDownListFor(m => m.Genre, new SelectList(Model.Genres, "Id", "Name"), "", new { @class = "form-control" })
        @Html.ValidationMessageFor(m => m.Genre)
    </div>
    <button type="submit" class="btn btn-primary">Save</button>
}






..HOME CONTROLLER:

using System;
using System.Data.Entity;
using System.Linq;
using PostingBoard.Models;
using System.Web.Mvc;

namespace PostingBoard.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;
        

        public HomeController()
        {
            _context = new ApplicationDbContext();

        }

        public ActionResult Index()
        {

            var upcomingGigs = _context.Gigs.Include(g => g.Artist).Include(g => g.Genre).Include(g =>g.Genre)
                  .Where(g => g.DateTime > DateTime.Now);

            return View(upcomingGigs);
                    
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}


..Home View
@model IEnumerable<PostingBoard.Models.Gig>
    
@{
    ViewBag.Title = "Home Page";
}

<ul class="gigs">
    
    @foreach (var gig in Model)
    {
        <li>
            <div class="date">
                <div class="month">
                    @gig.DateTime.ToString("MMM")
                </div>
                <div class="day">
                    @gig.DateTime.ToString("d ")
                </div>
            </div>
            <div class="details">
                <span class="artist">
                    @gig.Artist.UserName
                </span>
                <span class="genre">
                    @gig.Genre.Name
                </span>
                <button data-gig-id="@gig.Id" class="btn btn-default btn-sm pull-right js-toggle-attendance">Going?</button>
            </div>
        </li>
    }

</ul>

@section scripts
{
    <script>
        $(document).ready(function () {
            $(".js-toggle-attendance").click(function (e) {
                var button = $(e.target);
                $.post("/api/attendences", { gigId: button.attr("data-gig-id") })
                    .done(function () {
                        button
                            .removeClass("btn-default")
                            .addClass("btn-info")
                            .text("Going");
                    })
                    .fail(function () {
                        alert("Something failed!");
                    });
            });
        });
    </script>
}

Create following API

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PostingBoard.Models;
using Microsoft.AspNet.Identity;
using PostingBoard.Dtos;

namespace PostingBoard.Controllers
{
   
    [Authorize]
    public class AttendencesController : ApiController
    {

        private ApplicationDbContext _context;
        public AttendencesController()
        {
            _context = new ApplicationDbContext();
        }
        [HttpPost]
        public IHttpActionResult Attend(AttendanceDto dto )
        {

            var userId = User.Identity.GetUserId();
            var exists = _context.Attendences.Any(a => a.AttendeeId == userId && a.GigId == dto.GigId);

            if (exists)
                return BadRequest("The attendance already exists");

            var attendance = new Attendance
            {
                GigId = dto.GigId,
                AttendeeId = userId
            };
            _context.Attendences.Add(attendance);
            _context.SaveChanges();

            return Ok();

        }

    }
}
Add the following file into Global.ASAX

  GlobalConfiguration.Configure(WebApiConfig.Register);



Dtos folder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PostingBoard.Dtos
{
    public class AttendanceDto
    {
        public int GigId { get; set; }

    }
}


Site css.

body {
    padding-top: 90px;
    padding-bottom: 20px;
}

/* Set padding to keep content from hitting the edges */
.body-content {
    padding-left: 15px;
    padding-right: 15px;
}

/* Override the default bootstrap behavior where horizontal description lists 
   will truncate terms that are too long to fit in the left column 
*/
.dl-horizontal dt {
    white-space: normal;
}

/* Set width on the form input elements since they're 100% wide by default */
input,
select,
textarea {
    max-width: 280px;
}

span.field-validation-error {
    color: red;
    font-weight: bold;
}

/* Bootstrap Overrides */
.navbar-inverse {
    background-color: #ff4342;
    border-color: #ff4342;
}

    .navbar-inverse .navbar-nav > li > a {
        color: #fff;
    }

    .navbar-inverse .navbar-brand {
        color: #fff;
    }

    .navbar-inverse .navbar-nav > .open > a, .navbar-inverse .navbar-nav > .open > a:hover, .navbar-inverse .navbar-nav > .open > a:focus {
        background-color: rgba(205, 40, 39, 0.55);
    }

.dropdown-menu {
    box-shadow: none;
    -webkit-box-shadow: none;
}

.navbar-inverse .navbar-nav > .dropdown > a .caret {
    border-top-color: #fff;
    border-bottom-color: #fff;
}

.navbar-brand {
    font-weight: 700;
}

body {
    font-size: 17px;
}

body,
h1,
h2,
h3,
h4,
h5,
h6,
.h1,
.h2,
.h3,
.h4,
.h5,
.h6 {
    font-family: Lato, "Helvetica Neue", Helvetica, Arial, sans-serif;
}

.form-group {
    margin-bottom: 20px;
}

.form-control {
    font-size: 17px;
    height: 44px;
    -ms-border-radius: 9px;
    border-radius: 9px;
}


.btn {
    font-size: 17px;
    padding: 7px 20px;
    -ms-border-radius: 9px;
    border-radius: 9px;
}

/* Page-level styles */
.gigs {
    list-style: none;
}

    .gigs > li {
        position: relative;
        margin-bottom: 30px;
    }

        .gigs > li .date {
            background: red;
            color: white;
            text-align: center;
            width: 60px;
            -ms-border-radius: 8px;
            border-radius: 8px;
        }

            .gigs > li .date .month {
                text-transform: uppercase;
                font-size: 14px;
                font-weight: bold;
                padding: 2px 6px;
            }

            .gigs > li .date .day {
                background: #f7f7f7;
                color: #333;
                font-size: 20px;
                padding: 6px 12px;
            }

        .gigs > li .details {
            position: absolute;
            top: 0;
            left: 70px;
        }

            .gigs > li .details .artist {
                font-weight: bold;
                display: block;
            }

            .gigs > li .details .genre {
                font-size: 15px;
            }


View Model Class

 public class FutureDate : ValidationAttribute
    {

        public override bool IsValid(object value)
        {
            DateTime dateTime;
            var isValid = DateTime.TryParseExact(Convert.ToString(value),
                "d MMM yyyy",
                CultureInfo.CurrentCulture,
                DateTimeStyles.None,
                out dateTime);

            return (isValid && dateTime > DateTime.Now);
        }
    }


namespace PostingBoard.ViewModels
{
    public class ValidTime : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime dateTime;
            var isValid = DateTime.TryParseExact(Convert.ToString(value),
                "HH:mm",
                CultureInfo.CurrentCulture,
                DateTimeStyles.None,
                out dateTime);

            return (isValid);
        }
    }
}


