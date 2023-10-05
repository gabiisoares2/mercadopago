using MercadoPago.Client;
using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource;
using MercadoPago.Resource.Payment;
using MercadoPago.Resource.Preference;
using MercadoPago.Resource.PaymentMethod;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MercadoPago.Resource.AdvancedPayment;
using MercadoPago.Http;
using MercadoPago.Client.AdvancedPayment;
using MercadoPago.Client.Customer;
using MercadoPago.Client.MerchantOrder;
using MercadoPago.Resource.MerchantOrder;

namespace service
{
    public class MercadoPagoService : IMercadoPagoService
    {
        const string acesstoken = "your token";

        public async Task<string> MercadoPagoPrePaymentLink(PreferenceRequest request)
        {
            MercadoPagoConfig.AccessToken = acesstoken;

            var client = new PreferenceClient();
            Preference preference = await client.CreateAsync(request);

            return preference.InitPoint;
        }

        public async Task<IEnumerable<MerchantOrder>> MercadoPagoCapturePaymentByReference(string externalReference)
        {

            MercadoPagoConfig.AccessToken = acesstoken;
            PaymentClient payment = new PaymentClient();
            MerchantOrderClient cc = new MerchantOrderClient();                     

            SearchRequest searchRequest = new SearchRequest();
            var dictFilter = new Dictionary<string, object>
            {
                { "sort", "date_created" },
                { "criteria", "desc" },
                { "external_reference", externalReference }
            };

            searchRequest.Filters = dictFilter;
            searchRequest.Limit = 10;
            searchRequest.Offset = 0;

            var requestOptions = new RequestOptions()
            {
                AccessToken = acesstoken
            };

            return cc.SearchAsync(searchRequest, requestOptions).Result.Elements.OrderByDescending(x => x.DateCreated);
        }

        public async Task<Payment> MercadoPagoCapturePayment(long id, CancellationToken ctx)
        {
            RequestOptions requestOptions = new RequestOptions()
            {
                AccessToken = acesstoken
            };
            
            var paymentClient = new PaymentClient();
            var obtemCodigoReferencia = new Payment();
            obtemCodigoReferencia = await paymentClient.CaptureAsync(id, requestOptions, ctx);

            MercadoPagoConfig.AccessToken = acesstoken;
            SearchRequest searchRequest = new SearchRequest();

            var dictFilter = new Dictionary<string, object>
            {
                 { "sort", "date_created" },
                 { "criteria", "desc" },
                 { "external_reference", obtemCodigoReferencia.ExternalReference }
            };
            searchRequest.Filters = dictFilter;
            searchRequest.Limit = 10;
            searchRequest.Offset = 0;          

            var paymentsComplete = await paymentClient.SearchAsync(searchRequest, requestOptions);

            if (paymentsComplete != null && paymentsComplete.Results.Count > 0)
            {
                var paymentsFinal = paymentsComplete.Results.Where(x => x.Id.HasValue && x.Status == "approved");
                if (paymentsFinal != null)
                {
                    decimal valorTotal = 0;
                    foreach (var item in paymentsFinal)
                    {
                        Payment payment = await paymentClient.CaptureAsync(item.Id.Value);
                        if (payment != null && payment.Status != "refunded")
                        {
                            valorTotal += payment.TransactionAmount.Value;
                        }
                    }
                    return paymentsFinal.FirstOrDefault();
                }
            }
            return null;
        }

        public async Task MercadoPagoCancel(long id)
        {
            try
            {
            MercadoPagoConfig.AccessToken = acesstoken;
            
            var client = new PaymentClient();
            RequestOptions requestOptions = new RequestOptions()
            {
                AccessToken = acesstoken
            }; 
            var payment = client.Cancel(id, requestOptions);
            }
            catch
            {
                throw;
            }
        }

        public async Task MercadoPagoRefund(long id, CancellationToken ctx)
        {
            try
            {
                MercadoPagoConfig.AccessToken = acesstoken;

                var client = new PaymentClient();
                RequestOptions requestOptions = new RequestOptions()
                {
                    AccessToken = acesstoken
                };
                var payment = client.RefundAsync(id, requestOptions, ctx);
            }
            catch
            {
                throw;
            }
        }
    }
}