using Do_an_II.Utilities.VNPay;

namespace Do_an_II.Services.VnPay
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string CreatePaymentUrl(VnPayRequestModel model, HttpContext context)
        {
            var tick = DateTime.Now.Ticks.ToString();
            var vnpay = new VnPayLibrary();
            var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];

            vnpay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            vnpay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());
            vnpay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng" + model.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", _configuration["Vnpay:PaymentBackReturnUrl"]);
            vnpay.AddRequestData("vnp_TxnRef", $"{model.OrderId}_{tick}");

            var paymentUrl =
                vnpay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

            return paymentUrl;

        }

        public VnPayResponseModel PaymentExcute(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in collections)
            {
                if(!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

           var vnp_orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef").Split('_')[0]);
            var vnp_orderInfo = vnpay.GetResponseData("vnp_OrderInfo");
            var vnp_responseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_TransactionId = vnpay.GetResponseData("vnp_TransactionNo");

            var vnp_secureHash = vnpay.GetResponseData("vnp_SecureHash");
            bool CheckSignature = vnpay.ValidateSignature(vnp_secureHash, _configuration["Vnpay:HashSecret"]);
            if (!CheckSignature)
            {
                return new VnPayResponseModel
                {
                    Success = false,
                    
                };
            }
            
            return new VnPayResponseModel
            {
                OrderDescription = vnp_orderInfo,
                TransactionId = vnp_TransactionId,
                OrderId = vnp_orderId.ToString(),
                PaymentMethod = "VnPay",
                PaymentId = vnpay.GetResponseData("vnp_PayDate"),
                Success = true,
                Token = vnp_secureHash,
                VnPayResponseCode = vnp_responseCode.ToString()
            };



        }
    }
}
