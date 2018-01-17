﻿using System;
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
