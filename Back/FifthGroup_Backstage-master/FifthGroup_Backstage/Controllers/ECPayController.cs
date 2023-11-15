using FifthGroup_Backstage.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FifthGroup_Backstage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ECPayController : ControllerBase
    {
        [HttpPost]
        public string AddOrders(get_localStorage json)
        {

            string num = "0";
            DbHouseContext context = new DbHouseContext();
            try
            {
                EcpayOrder Orders = new EcpayOrder();
                Orders.MemberId = json.MerchantID;
                Orders.MerchantTradeNo = json.MerchantTradeNo;
                Orders.RtnCode = 0; //未付款
                Orders.RtnMsg = "訂單成功尚未付款";
                Orders.TradeNo = json.MerchantID.ToString();
                Orders.TradeAmt = json.TotalAmount;
                Orders.PaymentDate = Convert.ToDateTime(json.MerchantTradeDate);
                Orders.PaymentType = json.PaymentType;
                Orders.PaymentTypeChargeFee = "0";
                Orders.TradeDate = json.MerchantTradeDate;
                Orders.SimulatePaid = 0;
                context.EcpayOrders.Add(Orders);
                context.SaveChanges();
                num = "OK";
            }
            catch (Exception ex)
            {
                num = ex.ToString();
            }
            return num;
        }



    }
}
