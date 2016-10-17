using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Treehouse.FitnessFrog.Data;
using Treehouse.FitnessFrog.Models;

namespace Treehouse.FitnessFrog.Controllers
{
    public class EntriesController : Controller
    {
        private EntriesRepository _entriesRepository = null;

        public EntriesController()
        {
            _entriesRepository = new EntriesRepository();
        }

        public ActionResult Index()
        {
            List<Entry> entries = _entriesRepository.GetEntries();

            // Calculate the total activity.
            double totalActivity = entries
                .Where(e => e.Exclude == false)
                .Sum(e => e.Duration);

            // Determine the number of days that have entries.
            int numberOfActiveDays = entries
                .Select(e => e.Date)
                .Distinct()
                .Count();

            ViewBag.TotalActivity = totalActivity;
            ViewBag.AverageDailyActivity = (totalActivity / (double)numberOfActiveDays);

            return View(entries);
        }

        public ActionResult Add()
        {
            var entry = new Entry
            {
                Date = DateTime.Today,
            };

            SetupActivitiesSelectListItem();

            return View(entry);
        }

        [HttpPost]
        public ActionResult Add(Entry entry)
        {
            ValidateEntry(entry);

            if (ModelState.IsValid)
            {
                //save the model state
                _entriesRepository.AddEntry(entry);

                //display the entries list page
                return RedirectToAction("Index");
            }

            SetupActivitiesSelectListItem();

            return View(entry);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
         
            Entry entry = _entriesRepository.GetEntry((int)id);
           
            if(entry == null)
            {
                return HttpNotFound();
            }

            SetupActivitiesSelectListItem();
            return View(entry);
        }

        [HttpPost]
        public ActionResult Edit(Entry entry)
        {
            ValidateEntry(entry);

            if (ModelState.IsValid)
            {
                _entriesRepository.UpdateEntry(entry);

                return RedirectToAction("Index");
            }
            //TODO populate the activites select list items ViewBag property
            SetupActivitiesSelectListItem();

            return View(entry);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View();
        }

        private void ValidateEntry(Entry entry)
        {
            //if no duration field errors, make sure duration is greater than 0
            if (ModelState.IsValidField("Duration") && entry.Duration <= 0)
            {
                ModelState.AddModelError("Duration", "The Duration field value must be greater than 0");
            }
        }

        private void SetupActivitiesSelectListItem()
        {
            ViewBag.ActivitiesSelectListItems = new SelectList(
                           Data.Data.Activities, "Id", "Name");
        }
    }
}