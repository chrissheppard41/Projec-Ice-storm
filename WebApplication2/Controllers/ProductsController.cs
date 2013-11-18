using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication2.Models;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Data.Entity;
using System.Web;

namespace WebApplication2.Controllers
{
    public class ProductsController : ApiController
    {
        ProductsDBEntities db = new ProductsDBEntities();

        [HttpGet]
        public HttpResponseMessage GetAllProducts()
        {
            return Request.CreateResponse<IEnumerable<Product>>(HttpStatusCode.OK, this.db.Products);
        }

        [HttpGet]
        public HttpResponseMessage GetProduct(int id)
        {
            var product = this.db.Products.FirstOrDefault((p) => p.id == id);
            if (product == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "This Product does not exist");
            }
            return Request.CreateResponse<Product>(HttpStatusCode.OK, product);
        }
        
        [HttpPost]
        public HttpResponseMessage Add(Product item)
        {
            try
            {
                this.db.Products.Add(item);
                this.db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                List<String> errors = new List<String>();
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        errors.Add("Property: "+validationError.PropertyName+" Error: {1}" +validationError.ErrorMessage);
                    }
                }

                return Request.CreateResponse<List<String>>(HttpStatusCode.BadRequest, errors);
            }

            return Request.CreateResponse<Product>(HttpStatusCode.OK, item);
        }

        [HttpPut]
        public HttpResponseMessage Update(int id, Product input)
        {
            Product item = this.db.Products.Find(id);
            if (item == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "This Product does not exist");
            }


            try
            {
                item.Name       = input.Name;
                item.Category   = input.Category;
                item.Price      = input.Price;

                this.db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                List<String> errors = new List<String>();
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        errors.Add("Property: " + validationError.PropertyName + " Error: {1}" + validationError.ErrorMessage);
                    }
                }

                return Request.CreateResponse<List<String>>(HttpStatusCode.BadRequest, errors);
            }
            catch (InvalidOperationException iOEx)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, iOEx.Message);
            }

            return Request.CreateResponse<Product>(HttpStatusCode.OK, item);
            //return item;
        }

        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            Product item = this.db.Products.Find(id);
            if (item == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "This Product does not exist");
            }

            db.Entry(item).State = EntityState.Deleted;
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, "Product has been deleted");
        }
    }
}
