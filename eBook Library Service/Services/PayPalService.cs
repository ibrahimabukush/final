using PayPal.Api;
using Microsoft.Extensions.Configuration;

namespace eBook_Library_Service.Services
{
    public class PayPalService
    {
        private readonly APIContext _apiContext;

        public PayPalService(IConfiguration configuration)
        {
            // Load PayPal configuration from appsettings.json
            var clientId = configuration["PayPal:ClientId"];
            var clientSecret = configuration["PayPal:ClientSecret"];
            var mode = configuration["PayPal:Mode"];

            // Set up PayPal API context
            var config = new Dictionary<string, string>
            {
                { "mode", mode } // "sandbox" or "live"
            };

            var accessToken = new OAuthTokenCredential(clientId, clientSecret, config).GetAccessToken();
            _apiContext = new APIContext(accessToken) { Config = config };
        }

        /// <summary>
        /// Creates a PayPal payment.
        /// </summary>
        /// <param name="amount">The total amount to charge.</param>
        /// <param name="returnUrl">The URL to redirect to after payment approval.</param>
        /// <param name="cancelUrl">The URL to redirect to if the payment is canceled.</param>
        /// <param name="intent">The payment intent (e.g., "sale" for immediate payment).</param>
        /// <returns>The created PayPal payment object.</returns>
        public Payment CreatePayment(decimal amount, string returnUrl, string cancelUrl, string intent = "sale")
        {
            var payment = new Payment
            {
                intent = intent,
                payer = new Payer { payment_method = "paypal" },
                transactions = new List<Transaction>
                {
                    new Transaction
                    {
                        amount = new Amount
                        {
                            currency = "USD",
                            total = amount.ToString("F2") // Format amount to 2 decimal places
                        },
                        description = "eBook Purchase"
                    }
                },
                redirect_urls = new RedirectUrls
                {
                    return_url = returnUrl,
                    cancel_url = cancelUrl
                }
            };

            return payment.Create(_apiContext);
        }

        /// <summary>
        /// Executes a PayPal payment after user approval.
        /// </summary>
        /// <param name="paymentId">The PayPal payment ID.</param>
        /// <param name="payerId">The PayPal payer ID.</param>
        /// <returns>The executed PayPal payment object.</returns>
        public Payment ExecutePayment(string paymentId, string payerId)
        {
            var paymentExecution = new PaymentExecution { payer_id = payerId };
            var payment = new Payment { id = paymentId };
            return payment.Execute(_apiContext, paymentExecution);
        }
    }
}