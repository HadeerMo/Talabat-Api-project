using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.IO;
using System;
using System.Threading.Tasks;
using Talabat.API.Errors;
using Talabat.core.Entities;
using Talabat.core.IServices;
using Talabat.core.Entities.OrderAggregate;
using Microsoft.Extensions.Logging;

namespace Talabat.API.Controllers
{
    //payment
    public class PaymentController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;
        private const string _whSecret = "whsec_f3160051dc0fac76a001cfeaa4ec7798463a42d35fa06e4db0ff4c43d9fe6a0d";
        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntentAsync(basketId);
            if (basket == null) return BadRequest(new ApiResponse(400, "A problem with your basket"));
            return Ok(basket);
        }

        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var stripeEvent = EventUtility.ConstructEvent(json,
                Request.Headers["Stripe-Signature"], _whSecret);


            PaymentIntent intent;
            Order order;
            // Handle the event
            switch (stripeEvent.Type)
            {
                case Events.PaymentIntentSucceeded:
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    order = await _paymentService.UpdatePaymentIntentSucceededOrFailed(intent.Id, true);
                    _logger.LogInformation("Payment Succeeded");
                    break;

                case Events.PaymentIntentPaymentFailed:
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    order = await _paymentService.UpdatePaymentIntentSucceededOrFailed(intent.Id, false);
                    _logger.LogInformation("Payment Failed");
                    break;
            }


            return new EmptyResult();

        }
    }
}
