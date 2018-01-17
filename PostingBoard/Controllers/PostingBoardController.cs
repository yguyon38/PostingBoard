using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PostingBoard.Models;
using PostingBoard.ViewModels;
using Microsoft.AspNet.Identity;

namespace PostingBoard.Controllers
{
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

}