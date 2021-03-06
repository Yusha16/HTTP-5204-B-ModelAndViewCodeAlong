﻿using System;
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
using PetGrooming.Models.ViewModels;
using System.Diagnostics;

namespace PetGrooming.Controllers
{
    public class PetController : Controller
    {
        /*
        These reading resources will help you understand and navigate the MVC environment
 
        Q: What is an MVC controller?

        - https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions-1/controllers-and-routing/aspnet-mvc-controllers-overview-cs

        Q: What does it mean to "Pass Data" from the Controller to the View?

        - http://www.webdevelopmenthelp.net/2014/06/using-model-pass-data-asp-net-mvc.html

        Q: What is an SQL injection attack?

        - https://www.w3schools.com/sql/sql_injection.asp

        Q: How can we prevent SQL injection attacks?

        - https://www.completecsharptutorial.com/ado-net/insert-records-using-simple-and-parameterized-query-c-sql.php

        Q: How can I run an SQL query against a database inside a controller file?

        - https://www.entityframeworktutorial.net/EntityFramework4.3/raw-sql-query-in-entity-framework.aspx
 
         */
        private PetGroomingContext db = new PetGroomingContext();

        // GET: Pet
        public ActionResult List()
        {
            //How could we modify this to include a search bar?
            List<Pet> pets = db.Pets.SqlQuery("Select * from Pets").ToList();
            return View(pets);
           
        }

        // GET: Pet/Details/5
        public ActionResult Show(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Pet pet = db.Pets.Find(id); //EF 6 technique
            Pet pet = db.Pets.SqlQuery("select * from pets where petid=@PetID", new SqlParameter("@PetID",id)).FirstOrDefault();
            if (pet == null)
            {
                return HttpNotFound();
            }

            //TODO:
            //Must add a way to list out the Owners this Pet is related to
            //Must add a way to list out the GroomBookings this Pet is related to
            //Can also make the Species Name be a link to the Show Species

            return View(pet);
        }

        //THE [HttpPost] Means that this method will only be activated on a POST form submit to the following URL
        //URL: /Pet/Add
        [HttpPost]
        public ActionResult Add(string PetName, Double PetWeight, String PetColor, int SpeciesID, string PetNotes)
        {
            //STEP 1: PULL DATA! The data is access as arguments to the method. Make sure the datatype is correct!
            //The variable name  MUST match the name attribute described in Views/Pet/Add.cshtml

            //Tests are very useful to determining if you are pulling data correctly!
            Debug.WriteLine("Want to create a pet with name " + PetName + " and weight " + PetWeight.ToString()) ;

            //STEP 2: FORMAT QUERY! the query will look something like "insert into () values ()"...
            string query = "insert into pets (PetName, Weight, color, SpeciesID, Notes) values (@PetName,@PetWeight,@PetColor,@SpeciesID,@PetNotes)";
            SqlParameter[] sqlparams = new SqlParameter[5]; //0,1,2,3,4 pieces of information to add
            //each piece of information is a key and value pair
            sqlparams[0] = new SqlParameter("@PetName",PetName);
            sqlparams[1] = new SqlParameter("@PetWeight", PetWeight);
            sqlparams[2] = new SqlParameter("@PetColor", PetColor);
            sqlparams[3] = new SqlParameter("@SpeciesID", SpeciesID);
            sqlparams[4] = new SqlParameter("@PetNotes",PetNotes);

            Debug.WriteLine(query);

            //db.Database.ExecuteSqlCommand will run insert, update, delete statements
            //db.Pets.SqlCommand will run a select statement, for example.
            db.Database.ExecuteSqlCommand(query, sqlparams);
            
            //run the list method to return to a list of pets so we can see our new one!
            return RedirectToAction("List");
        }


        public ActionResult New()
        {
            //STEP 1: PUSH DATA!
            //What data does the Add.cshtml page need to display the interface?
            //A list of species to choose for a pet

            //alternative way of writing SQL -- will learn more about this week 4
            //List<Species> Species = db.Species.ToList();

            Debug.WriteLine("User click on the Add new pet link");

            List<Species> species = db.Species.SqlQuery("select * from Species").ToList();

            return View(species);
        }

        public ActionResult Update(int id)
        {
            Debug.WriteLine("User click on the Update link id=" + id);

            //need information about a particular pet
            Pet selectedPet = db.Pets.SqlQuery("select * from pets where petid = @id", new SqlParameter("@id",id)).FirstOrDefault();

            //need information about all species
            string query = "select * from species";
            List<Species> selectedSpecies = db.Species.SqlQuery(query).ToList();

            Debug.WriteLine(query);

            //create an instance of our ViewModel
            UpdatePet viewModel = new UpdatePet();
            viewModel.pet = selectedPet;
            viewModel.species = selectedSpecies;

            return View(viewModel);
        }

        //TODO:
        //Update
        //[HttpPost] Update
        //[HttpPost] Delete
        //(optional) Delete

        //MVC will know to look for id
        [HttpPost]
        public ActionResult Update(int id, string PetName, string PetColor, string PetNotes, double PetWeight)
        {
            //Debug Purpose to see if we grab the data properly
            Debug.WriteLine("I am trying to edit a pet's name to " + PetName + " and change the weight to " + PetWeight.ToString());

            //Query statement to update the specific Pet
            string query = "update pets";
            query += " set PetName = @PetName, Weight = @PetWeight, color = @PetColor, Notes = @PetNotes";
            query += " where PetID = @PetID";

            SqlParameter[] sqlparams = new SqlParameter[5]; //5 pieces of info to pass

            //each piece of information is a key and value pair
            sqlparams[0] = new SqlParameter("@PetName", PetName);
            sqlparams[1] = new SqlParameter("@PetWeight", PetWeight);
            sqlparams[2] = new SqlParameter("@PetColor", PetColor);
            sqlparams[3] = new SqlParameter("@PetNotes", PetNotes);
            sqlparams[4] = new SqlParameter("@PetID", id);

            Debug.WriteLine(query);

            //Execute query command
            db.Database.ExecuteSqlCommand(query, sqlparams);

            //logic for updating the pet in the database goes here
            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            //Debug purpose to see if we grab the id correctly
            Debug.WriteLine("I'm pulling data of " + id.ToString());

            //Query statement to delete the specific Pet
            string query = "delete from pets";
            query += " where PetID = @PetID";

            SqlParameter sqlparam = new SqlParameter("@PetID", id);

            Debug.WriteLine(query);

            //Execute query command
            db.Database.ExecuteSqlCommand(query, sqlparam);

            //Go back to list of pet
            return RedirectToAction("List");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
