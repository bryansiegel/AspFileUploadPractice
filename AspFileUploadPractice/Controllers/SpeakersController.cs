using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AspFileUploadPractice.Data;
using AspFileUploadPractice.Models;
using AspFileUploadPractice.ViewModels;
using Microsoft.AspNetCore.Hosting;

namespace AspFileUploadPractice.Controllers
{
    public class SpeakersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SpeakersController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
        }

        // GET: Speakers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Speaker.ToListAsync());
        }

        // GET: Speakers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var speaker = await _context.Speaker
                .FirstOrDefaultAsync(m => m.Id == id);

            var speakerViewModel = new SpeakerViewModel()
            {
                Id = speaker.Id,
                SpeakerName = speaker.SpeakerName,
                Qualification = speaker.Qualification,
                Experience = speaker.Experience,
                SpeakingDate = speaker.SpeakingDate,
                SpeakingTime = speaker.SpeakingTime,
                Venue = speaker.Venue,
                ExistingImage = speaker.ProfilePicture
            };

            if (speaker == null)
            {
                return NotFound();
            }

            return View(speaker);
        }

        // GET: Speakers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Speakers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SpeakerName,Qualification,Experience,SpeakingDate,SpeakingTime,Venue,ProfilePicture")] SpeakerViewModel model)
        {
            //if (ModelState.IsValid)
            //{
            //    _context.Add(speaker);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}

            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(model);
                Speaker speaker = new Speaker
                {
                    SpeakerName = model.SpeakerName,
                    Qualification = model.Qualification,
                    Experience = model.Experience,
                    SpeakingDate = model.SpeakingDate,
                    SpeakingTime = model.SpeakingTime,
                    Venue = model.Venue,
                    ProfilePicture = uniqueFileName
                };
                _context.Add(speaker);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Speakers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var speaker = await _context.Speaker.FindAsync(id);


            var speakerViewModel = new SpeakerViewModel()
            {
                Id = speaker.Id,
                SpeakerName = speaker.SpeakerName,
                Qualification = speaker.Qualification,
                Experience = speaker.Experience,
                SpeakingDate = speaker.SpeakingDate,
                SpeakingTime = speaker.SpeakingTime,
                Venue = speaker.Venue,
                ExistingImage = speaker.ProfilePicture
            };

            if (speaker == null)
            {
                return NotFound();
            }
            return View(speakerViewModel);
        }

        // POST: Speakers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SpeakerName,Qualification,Experience,SpeakingDate,SpeakingTime,Venue,ProfilePicture")] SpeakerViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var speaker = await _context.Speaker.FindAsync(model.Id);

                speaker.SpeakerName = model.SpeakerName;
                speaker.Qualification = model.Qualification;
                speaker.Experience = model.Experience;
                speaker.SpeakingDate = model.SpeakingDate;
                speaker.SpeakingTime = model.SpeakingTime;
                speaker.Venue = model.Venue;

                if (model.SpeakerPicture  != null)
                {
                    if (model.ExistingImage != null)
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", model.ExistingImage);
                        System.IO.File.Delete(filePath);
                    }

                    speaker.ProfilePicture = ProcessUploadedFile(model);
                }
                _context.Update(speaker);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        // GET: Speakers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var speaker = await _context.Speaker
                .FirstOrDefaultAsync(m => m.Id == id);
            if (speaker == null)
            {
                return NotFound();
            }

            return View(speaker);
        }

        // POST: Speakers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var speaker = await _context.Speaker
                .FirstOrDefaultAsync(m => m.Id == id);

            var speakerViewModel = new SpeakerViewModel()
            {
                Id = speaker.Id,
                SpeakerName = speaker.SpeakerName,
                Qualification = speaker.Qualification,
                Experience = speaker.Experience,
                SpeakingDate = speaker.SpeakingDate,
                SpeakingTime = speaker.SpeakingTime,
                Venue = speaker.Venue,
                ExistingImage = speaker.ProfilePicture
            };
            if (speaker == null)
            {
                return NotFound();
            }

            return View(speakerViewModel);
        }

        private bool SpeakerExists(int id)
        {
            return _context.Speaker.Any(e => e.Id == id);
        }
        private string ProcessUploadedFile(SpeakerViewModel model)
        {
            string uniqueFileName = null;

            if (model.SpeakerPicture != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.SpeakerPicture.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.SpeakerPicture.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }


}
