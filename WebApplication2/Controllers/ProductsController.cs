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
using WebApplication2.Vendor;

namespace WebApplication2.Controllers
{
    public class ProductsController : ApiController
    {
        ProductsDBEntities db = new ProductsDBEntities();

        [HttpGet]
        public HttpResponseMessage GetAllProducts()
        {
            return Request.CreateResponse<Rest>(HttpStatusCode.OK, new Rest
            {
                code = HttpStatusCode.OK,
                status = HttpStatusCode.OK.ToString(),
                message = "Products data fetch",
                data = this.db.Products,
                errors = null
            });
        }

        [HttpGet]
        public HttpResponseMessage GetProduct(int id)
        {
            HttpStatusCode code = HttpStatusCode.OK;
            List<Errors> errors = new List<Errors>();

            var product = this.db.Products.FirstOrDefault((p) => p.id == id);
            if (product == null)
            {
                code = HttpStatusCode.NotFound;
                errors.Add(new Errors { key = "Product", value = "This Product does not exist" });
            }

            return Request.CreateResponse<Rest>(code, new Rest
            {
                code = code,
                status = code.ToString(),
                message = "Product data fetch",
                data = product,
                errors = errors
            });
        }
        
        [HttpPost]
        public HttpResponseMessage Add(Product item)
        {
            HttpStatusCode code = HttpStatusCode.OK;
            String message = "Your Product has been added";
            Product data = null;
            List<Errors> errors = new List<Errors>();

            if (item == null)
            {
                code = HttpStatusCode.BadRequest;
                message = "Product data";
                errors.Add(new Errors { key = "Product", value = "Your request is missing data" });
            }
            else
            {
                try
                {
                    this.db.Products.Add(item);
                    this.db.SaveChanges();

                    data = item;
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                            errors.Add(new Errors { key = validationError.PropertyName, value = validationError.ErrorMessage });
                        }
                    }

                    code = HttpStatusCode.BadRequest;
                    message = "Product data";
                }
            }

            return Request.CreateResponse<Rest>(code, new Rest
            {
                code = code,
                status = code.ToString(),
                message = message,
                data = data,
                errors = errors
            });
        }

        [HttpPut]
        public HttpResponseMessage Update(int id, Product input)
        {
            HttpStatusCode code = HttpStatusCode.OK;
            String message = "Product data";
            Product data = null;
            List<Errors> errors = new List<Errors>();

            Product item = this.db.Products.Find(id);
            if (item == null)
            {
                code = HttpStatusCode.NotFound;
                errors.Add(new Errors { key = "Product", value = "This Product does not exist" });
            }
            else
            {
                try
                {
                    item.Name = input.Name;
                    item.Category = input.Category;
                    item.Price = input.Price;

                    this.db.SaveChanges();

                    data = item;
                    message = "Your Product has been added";
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                            errors.Add(new Errors { key = validationError.PropertyName, value = validationError.ErrorMessage });
                        }
                    }

                    code = HttpStatusCode.BadRequest;
                }
                catch (InvalidOperationException iOEx)
                {
                    code = HttpStatusCode.Conflict;
                    errors.Add(new Errors { key = "Product", value = iOEx.Message });
                }
            }

            return Request.CreateResponse<Rest>(code, new Rest
            {
                code = code,
                status = code.ToString(),
                message = message,
                data = item,
                errors = errors
            });
        }

        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            HttpStatusCode code = HttpStatusCode.OK;
            String message = "Product has been deleted";
            List<Errors> errors = new List<Errors>();

            Product item = this.db.Products.Find(id);
            if (item == null)
            {
                code = HttpStatusCode.NotFound;
                message = "Product data";
                errors.Add(new Errors { key = "Product", value = "This Product does not exist" });
            }
            else
            {
                db.Entry(item).State = EntityState.Deleted;
                db.SaveChanges();
            }


            return Request.CreateResponse<Rest>(code, new Rest
            {
                code = code,
                status = code.ToString(),
                message = message,
                data = null,
                errors = errors
            });
        }
    }
}
