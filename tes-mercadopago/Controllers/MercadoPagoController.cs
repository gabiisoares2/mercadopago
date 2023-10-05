using MercadoPago.Client.Preference;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using service;
using System.Threading;

namespace tes_mercadopago.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MercadoPagoController : ControllerBase
    {
        private readonly IMercadoPagoService _mercadoPagoService;
        public MercadoPagoController(IMercadoPagoService mercadoPagoService)
        {
            _mercadoPagoService = mercadoPagoService;
        }

        [HttpGet]
        [Route("prepaymentlink")]
        public async Task<IActionResult> PrePaymentLink(PreferenceRequest request)
        {
            var linkPrePayment = await _mercadoPagoService.MercadoPagoPrePaymentLink(request);
            return Ok(linkPrePayment);
        }

        [HttpGet]
        [Route("capture-payment-by-reference")]
        public async Task<IActionResult> CapturePaymentByReference(string externalReference)
        {
            var merchantOrders = await _mercadoPagoService.MercadoPagoCapturePaymentByReference(externalReference);
            return Ok(merchantOrders);
        }

        [HttpGet]
        [Route("capture-payment")]
        public async Task<IActionResult> CapturePayment(long id, CancellationToken ctx)
        {
            var payment = await _mercadoPagoService.MercadoPagoCapturePayment(id, ctx);
            return Ok(payment);
        }

        [HttpGet]
        [Route("refund")]
        public async Task<IActionResult> Refund(long id, CancellationToken ctx)
        {
            await _mercadoPagoService.MercadoPagoRefund(id, ctx);
            return Ok();
        }

        [HttpGet]
        [Route("cancel")]
        public async Task<IActionResult> Cancel(long id)
        {
            await _mercadoPagoService.MercadoPagoCancel(id);
            return Ok();
        }
    }
}
