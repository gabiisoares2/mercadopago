using MercadoPago.Client.Preference;

namespace service
{
    internal class BackUrls : PreferenceBackUrlsRequest
    {
        public string Success { get; set; }
        public string Failure { get; set; }
        public string Pending { get; set; }
    }
}