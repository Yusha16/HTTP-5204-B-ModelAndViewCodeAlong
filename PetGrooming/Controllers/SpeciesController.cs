using System;
using System.Collections.Generic;
using System.Data;
//required for SqlParameter class
using System.Data.SqlClient;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PetGrooming.Data;
using PetGrooming.Models;
using System.Diagnostics;

namespace PetGrooming.Controllers
{
    public class SpeciesController : Controller
    {
        private PetGroomingContext db = new PetGroomingContext();
        // GET: Species
        public ActionResult Index()
        {
            return View();
        }

        //TODO: Each line should be a separate method in this class
        // List
        public ActionResult List()
        {
            //what data do we need?
            List<Species> myspecies = db.Species.SqlQuery("Select * from species").ToList();

            return View(myspecies);
        }

        // Show
        // Add
        // [HttpPost] Add
        // Update
        // [HttpPost] Update
        // (optional) delete
        // [HttpPost] Delete

        public ActionResult Show(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Species species = db.Species.Find(id); //EF 6 technique

            //Query statement to get the specific Species
            string query = "select * from species where speciesid=@SpeciesID";
            SqlParameter sqlparam = new SqlParameter("@SpeciesID", id);

            Debug.WriteLine(query);

            //Get the Specific Species
            Species selectedSpecies = db.Species.SqlQuery(query, sqlparam).FirstOrDefault();

            if (selectedSpecies == null)
            {
                return HttpNotFound();
            }

            //TODO:
            //Must add a way to list out the Pets this Species is related to

            //Show the result
            return View(selectedSpecies);
        }

        //Server handling the code to show the cshtml of Add
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(string SpeciesName)
        {
            Debug.WriteLine("Add Method is called (User click on submit button) and SpeciesName=" + SpeciesName);

            //The query to add a new Species
            string query = "insert into species (Name) values (@SpeciesName)";
            SqlParameter sqlparam = new SqlParameter("@SpeciesName", SpeciesName);

            Debug.WriteLine(query);

            //Run the sql command
            db.Database.ExecuteSqlCommand(query, sqlparam);

            //Go back to the list of Species to see the added Species
            return RedirectToAction("List");
        }

        public ActionResult Update(int id)
        {
            Debug.WriteLine("Update Method is called and id=" + id);
            //Query statement to select the specific Species
            string query = "select * from species where SpeciesID = @SpeciesID";
            SqlParameter sqlparam = new SqlParameter("@SpeciesID", id);

            Debug.WriteLine(query);

            //The query is returning a list, so we only want the first one
            Species selectedspecies = db.Species.SqlQuery(query, sqlparam).FirstOrDefault();

            //read the species data
            return View(selectedspecies);
        }

        [HttpPost]
        public ActionResult Update(int id, string SpeciesName)
        {
            Debug.WriteLine("Update Method is called (Click the submit button) id=" + id + " SpeciesName= " + SpeciesName);

            //Query statement to update the specific Species
            string query = "update species";
            query += " set Name = @SpeciesName";
            query += " where SpeciesID = @SpeciesID";

            SqlParameter[] sqlparams = new SqlParameter[2];
            sqlparams[0] = new SqlParameter("@SpeciesName", SpeciesName);
            sqlparams[1] = new SqlParameter("@SpeciesID", id);

            Debug.WriteLine(query);

            //Execute query command
            db.Database.ExecuteSqlCommand(query, sqlparams);

            //Go back to the list of Species to see our changes
            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            //Debug Purpose to see if we are getting the id
            Debug.WriteLine("I'm pulling data of " + id.ToString());

            //Query statement to delete the specific Species
            string query = "delete from species";
            query += " where SpeciesID = @SpeciesID";

            SqlParameter sqlparam = new SqlParameter("@SpeciesID", id);

            Debug.WriteLine(query);

            //Execute query command
            db.Database.ExecuteSqlCommand(query, sqlparam);

            //Go back to List of Species
            return RedirectToAction("List");
        }
    }
}