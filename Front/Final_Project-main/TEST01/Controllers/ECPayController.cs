using FifthGroup_front.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;

namespace FifthGroup_Backstage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ECPayController : ControllerBase
    {
        [HttpPost("AddOrders")]
        public string AddOrders([FromBody]get_localStorage json)
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
                Orders.TradeAmt = Int32.Parse(json.TotalAmount);
                Orders.PaymentDate = Convert.ToDateTime(json.MerchantTradeDate);
                Orders.PaymentType = json.PaymentType;
                Orders.PaymentTypeChargeFee = "0";
                Orders.TradeDate = json.MerchantTradeDate;
                Orders.SimulatePaid = 1;
                context.EcpayOrders.Add(Orders);
                var pm = context.Payments.FirstOrDefault(s=>s.PaymentCode == Int32.Parse(json.CustomField1));
                pm.MerchantTradeNo = json.MerchantTradeNo;

                context.SaveChanges();


                num = "OK";
            }
            catch (Exception ex)
            {
                num = ex.ToString();
            }
            return num;
        }


        //public string AddPayInfo([FromBody] get_localStorage json)
        //{

        //    string result = "";
        //    DbHouseContext context = new DbHouseContext();
        //    try
        //    {
        //        EcpayOrder Orders = new EcpayOrder();
        //        Orders.MemberId = json.MerchantID;
        //        Orders.MerchantTradeNo = json.MerchantTradeNo;
        //        Orders.RtnCode = 0; //未付款
        //        Orders.RtnMsg = "訂單成功尚未付款";
        //        Orders.TradeNo = json.MerchantID.ToString();
        //        Orders.TradeAmt = Int32.Parse(json.TotalAmount);
        //        Orders.PaymentDate = Convert.ToDateTime(json.MerchantTradeDate);
        //        Orders.PaymentType = json.PaymentType;
        //        Orders.PaymentTypeChargeFee = "0";
        //        Orders.TradeDate = json.MerchantTradeDate;
        //        Orders.SimulatePaid = json.;
        //        context.EcpayOrders.Add(Orders);
        //        var pm = context.Payments.FirstOrDefault(s => s.PaymentCode == Int32.Parse(json.CustomField1));
        //        pm.MerchantTradeNo = json.MerchantTradeNo;

        //        context.SaveChanges();


        //        result = "1|OK";
        //    }
        //    catch (Exception ex)
        //    {
        //        result = ex.ToString();
        //    }
        //    return result;
        //}
        [HttpPost("PayInfo")]
        public string PayInfo([FromBody]FormCollection id)
        {
            var data = new Dictionary<string, string>();
            var result = "";
            foreach (string key in id.Keys)
            {
                data.Add(key, id[key]);
            }
            DbHouseContext db = new DbHouseContext();
            string temp = id["MerchantTradeNo"]; //寫在LINQ(下一行)會出錯，
            var ecpayOrder = db.EcpayOrders.Where(m => m.MerchantTradeNo == temp).FirstOrDefault();
            if (ecpayOrder != null)
            {
                ecpayOrder.RtnCode = int.Parse(id["RtnCode"]);
                if (id["RtnMsg"] == "Succeeded") ecpayOrder.RtnMsg = "已付款";
                ecpayOrder.PaymentDate = Convert.ToDateTime(id["PaymentDate"]);
                ecpayOrder.SimulatePaid = int.Parse(id["SimulatePaid"]);
                db.SaveChanges();
                result = "1|OK";
            }
            return result;
        }
        //public HttpResponseMessage AddPayInfo(JObject info)
        //{
        //    try
        //    {
        //        var cache = MemoryCache.Default;
        //        cache.Set(info.Value<string>("MerchantTradeNo"), info, DateTime.Now.AddMinutes(60));
        //        return ResponseOK();
        //    }
        //    catch (Exception e)
        //    {
        //        return ResponseError();
        //    }
        //}





    }
}
