using Do_an_II.Utilities.VNPay;

namespace Do_an_II.Services.VnPay
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(VnPayRequestModel paymentInformation, HttpContext httpContext);
        VnPayResponseModel PaymentExcute(IQueryCollection collections);
    }
}
