using MercadoPago.Client.Preference;
using MercadoPago.Resource.MerchantOrder;
using MercadoPago.Resource.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace service
{
    public interface IMercadoPagoService 
    {
        Task<IEnumerable<MerchantOrder>> MercadoPagoCapturePaymentByReference(string externalReference);
        Task<string> MercadoPagoPrePaymentLink(PreferenceRequest request);
        Task<Payment> MercadoPagoCapturePayment(long id, CancellationToken ctx);
        Task MercadoPagoCancel(long id);
        Task MercadoPagoRefund(long id, CancellationToken ctx);
    }
}
