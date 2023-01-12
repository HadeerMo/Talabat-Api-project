using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.API.Errors;
using Talabat.repository.Data;

namespace Talabat.API.Controllers
{ 
    public class BuggyController : BaseApiController
    {
        private readonly StoreContext context;

        public BuggyController(StoreContext context)
        {
            this.context = context;
        }

        [HttpGet("NotFound")]
        public ActionResult GetNotFoundRequest()
        {
            var product = context.Products.Find(100);
            if (product == null)
                return NotFound(new ApiResponse(404));
            return Ok(product);
        }

        [HttpGet("ServerError")]
        public ActionResult GetServerError()
        {
            var product = context.Products.Find(100);
            var productToReturn = product.ToString();
            return Ok(productToReturn);
        }

        [HttpGet("BadRequest")]
        public ActionResult GetBadRequest()
        {
            return BadRequest(new ApiResponse(400));
        }
        [HttpGet("BadRequest/{id}")] //GET: Buggy/BadRequest/five
        public ActionResult GetBadRequest(int id)
        {
            return Ok();
        }
    }
}
