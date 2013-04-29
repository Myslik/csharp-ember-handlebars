using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Ember.Handlebars.ExampleWebAPI.Models;
using System.Web.Mvc;

namespace Ember.Handlebars.ExampleWebAPI.Controllers
{
    public class ShowsController : ApiController
    {
        private AppContext db = new AppContext();

        // GET api/Shows
        public IEnumerable<Show> GetShows()
        {
            return db.Shows.AsEnumerable();
        }

        // GET api/Shows/5
        public Show GetShow(int id)
        {
            Show show = db.Shows.Find(id);
            if (show == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NoContent));
            }

            return show;
        }

        // PUT api/Shows/5
        [ValidateAntiForgeryToken]
        public HttpResponseMessage PutShow(int id, Show show)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != show.ShowId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(show).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/Shows
        [ValidateAntiForgeryToken]
        public HttpResponseMessage PostShow( Show show )
        {
            if (ModelState.IsValid)
            {
                db.Shows.Add(show);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, show);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = show.ShowId }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Shows/5
        [ValidateAntiForgeryToken]
        public HttpResponseMessage DeleteShow( int id )
        {
            Show show = db.Shows.Find(id);
            if (show == null)
            {
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }

            db.Shows.Remove(show);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, show);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}