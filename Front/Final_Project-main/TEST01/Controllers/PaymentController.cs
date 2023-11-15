using FifthGroup_front.Models;
using FifthGroup_front.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text;
using System.Web;
//using XSystem.Security.Cryptography;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FifthGroup_front.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;
using System.Globalization;

namespace FifthGroup_front.Controllers
{
    [WithoutAuthentication]
    public class PaymentController : Controller
    {
        public IActionResult Main()
        {
            DbHouseContext context = new DbHouseContext();



            IEnumerable<Payment> Res = (from i in context.Payments where i.HouseholdCode == HttpContext.Session.GetString("UserHouseholdCode") && i.MerchantTradeNo == null select i).ToList();
            //IEnumerable<Payment> House_P = (IEnumerable<Payment>)Res.Select(r =>r.PaymentCode in context.PaymentItems.Select(p => p.PaymentItemCode == r.PaymentItemCode &&p.ItemClassificationCode ==1));


            PaymentViewModel Paymodel = new PaymentViewModel()
            {
                House_P = Res.Where(r => context.PaymentItems.Any(p => p.PaymentItemCode == r.PaymentItemCode && p.ItemClassificationCode == 1)).ToList(),
                Manage_P = Res.Where(r => context.PaymentItems.Any(p => p.PaymentItemCode == r.PaymentItemCode && p.ItemClassificationCode == 2)).ToList(),
                Else_P = Res.Where(r => context.PaymentItems.Any(p => p.PaymentItemCode == r.PaymentItemCode && p.ItemClassificationCode == 3)).ToList(),
                PaymentItems = context.PaymentItems.ToList()

            };


            if (Paymodel.House_P.Count() == 0)
            {
                return View(Paymodel);
            }
            else
            {
                //List<string> date = new List<string>();
                //foreach(Payment i in r)
                //{
                //    date.Add(i.PayDay);

                //}

                //int House_mix_payment = Paymodel.House_P.Sum(Payment => Payment.Amount);
                //Payment mix_payment = (from i in context.Payments where i.HouseholdCode == HttpContext.Session.GetString("UserName") && i.Paid == false orderby i.PayDay select i).FirstOrDefault();
                //Payment mix_payment = Paymodel.House_P.OrderByDescending(p => p.PaymentItemCode).FirstOrDefault();

                //Paymodel.House_P = (IEnumerable<Payment>)mix_payment;
                return View(Paymodel);
            }


        }
        public IActionResult HousePayment()
        {
            DbHouseContext context = new DbHouseContext();
            var HousePay = from i in context.Payments where i.MerchantTradeNo == "" && i.HouseholdCode == HttpContext.Session.GetString("UserHouseholdCode") select i;

            if (HousePay == null)
                return RedirectToAction("Main");
            return View(HousePay);
        }

        //step1 : 網頁導入傳值到前端
        public async Task<IActionResult> ECPAY(int? id)
        {
            var orderId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
            //var orderId = "1235614sasdwa";
            //需填入你的網址
            //var website = $"https://localhost:5223/";
            var website = $"https://silencehouse2905.azurewebsites.net/";
            DbHouseContext context = new DbHouseContext();


            var PM = context.Payments.FirstOrDefault(s => s.PaymentCode == id);
            var PMItem = context.PaymentItems.FirstOrDefault(s => s.PaymentItemCode == PM.PaymentItemCode);

            
            var order = new Dictionary<string, string>
    {
        //綠界需要的參數
        { "MerchantTradeNo",  orderId},
        { "MerchantTradeDate",  DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")},
        { "TotalAmount",  $"{PMItem.Amount}"},
        { "TradeDesc",  PMItem.Remark},
        { "ItemName",  PMItem.PaymentName+" "+PMItem.Remark},
        //{ "ExpireDate",  ""},
        { "CustomField1",  $"{PM.PaymentCode}"},
        { "CustomField2",  ""},
        { "CustomField3",  ""},
        { "CustomField4",  ""},
        { "ReturnURL",  $"{website}api/ECPay/PayInfo"},
        { "OrderResultURL", $"{website}Payment/PayInfo"},
        {"ClientBackURL",$"{website}Payment/Main" },
        { "PaymentInfoURL",  $"{website}api/ECPay/AddAccountInfo"},
        { "ClientRedirectURL",  $"{website}Payment/Main"},
        { "MerchantID",  "2000132"},
        { "IgnorePayment", "" },
        { "PaymentType",  "aio"},
        { "ChoosePayment",  "ALL"},
        { "EncryptType",  "1"},
    };

            //檢查碼
            order["CheckMacValue"] = GetCheckMacValue(order);

            //string jsonOrder = JsonConvert.SerializeObject(order);

            //using (HttpClient client = new HttpClient())
            //{
            //    // 设置请求地址
            //    string apiUrl = "https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5";

            //    // 构建请求内容
            //    StringContent content = new StringContent(jsonOrder, System.Text.Encoding.UTF8, "application/json");

            //    // 发送 POST 请求
            //    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

            //    // 检查响应状态
            //    if (response.IsSuccessStatusCode)
            //    {
            //        // 处理成功响应，如果需要的话
            //        // 例如，您可以在这里获取响应内容并处理
            //        string responseBody = await response.Content.ReadAsStringAsync();

            //        // 在这里进行任何进一步的处理
            //        return Content(responseBody, "application/json"); // 这一行仅用于调试，显示响应内容
            //    }
            //    else
            //    {
            //        // 处理错误响应，如果需要的话
            //        // 例如，您可以在这里获取错误信息并处理
            //        string errorResponse = await response.Content.ReadAsStringAsync();

            //        // 在这里进行任何进一步的处理
            //        return Content(errorResponse, "application/json"); // 这一行仅用于调试，显示错误信息
            //    }
            //}

            return View(order);
        }
        private string GetCheckMacValue(Dictionary<string, string> order)
        {
            var param = order.Keys.OrderBy(x => x).Select(key => key + "=" + order[key]).ToList();
            var checkValue = string.Join("&", param);
            //測試用的 HashKey
            var hashKey = "5294y06JbISpM5x9";
            //測試用的 HashIV
            var HashIV = "v77hoKGq4kWxNNIS";
            checkValue = $"HashKey={hashKey}" + "&" + checkValue + $"&HashIV={HashIV}";
            checkValue = HttpUtility.UrlEncode(checkValue).ToLower();
            checkValue = GetSHA256(checkValue);
            return checkValue.ToUpper();
        }
        private string GetSHA256(string value)
        {
            var result = new StringBuilder();
            var sha256 = SHA256Managed.Create();
            var bts = Encoding.UTF8.GetBytes(value);
            var hash = sha256.ComputeHash(bts);
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }

        [HttpPost]
        public IActionResult PayInfo([FromForm] IFormCollection formData)
        {
            //_logger.LogInformation("Received payment information: {0}", formData);

            string temp = formData["MerchantTradeNo"];
            //string temp = "1235614";
            DbHouseContext context = new DbHouseContext();
            var ecpayOrder = context.EcpayOrders.FirstOrDefault(m => m.MerchantTradeNo == temp);
            var pm = context.Payments.FirstOrDefault(p => p.MerchantTradeNo == temp);
            if (ecpayOrder != null)
            {
                ecpayOrder.RtnCode = int.Parse(formData["RtnCode"]);
                if (formData["RtnMsg"] == "Succeeded") ecpayOrder.RtnMsg = "已付款";
                ecpayOrder.PaymentDate = Convert.ToDateTime(formData["PaymentDate"]);
                ecpayOrder.SimulatePaid = int.Parse(formData["SimulatePaid"]);
                pm.Paid = true;
                pm.PayDay = DateTime.Parse(formData["PaymentDate"]);
                context.SaveChanges();
            }

            return RedirectToAction("Main");
        }

        public IActionResult History(int? page , int? itemID = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            // 

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            string userID = HttpContext.Session.GetString("UserHouseholdCode");





            DbHouseContext context = new DbHouseContext();


           




            IQueryable <Payment> P = (from i in context.Payments where i.HouseholdCode == userID && i.Paid == true    select i); 
            
            if (startDate.HasValue && endDate.HasValue)
            {
                DateTime parsedStartDate = startDate.Value;
                DateTime parsedEndDate = endDate.Value;

                ViewBag.StartDate = parsedStartDate.ToString("yyyy-MM-dd");
                ViewBag.EndDate = parsedEndDate.ToString("yyyy-MM-dd");

                P = P.Where(p => p.PayDay >= startDate && p.PayDay <= endDate);
            }
            if (itemID.HasValue)
            {
                var Item = itemID.Value;

                
;
                var PItem = context.PaymentItems.Where(p => p.ItemClassificationCode == Item);
                P = from p in P
                    join pItem in PItem
                    on p.PaymentItemCode equals pItem.PaymentItemCode
                    select p;
            }
            ViewBag.itemID = itemID;

            P = P.OrderByDescending(p => p.PaymentCode);

            PaymentHistory PH = new PaymentHistory()
            {
                PagedPayments = P.ToPagedList(pageNumber, pageSize),
                Payments = (from i in context.Payments where i.HouseholdCode == userID && i.Paid == true select i).ToList(),
                PaymentItems = context.PaymentItems.ToList(),
                PaymentItemsName = context.PaymentItemsNames.ToList()

            };

           
               
            return View(PH);
            
        }


        //public IActionResult AA()
        //{
        //    return View();
        //}


        //[HttpPost("AccountInfo")]
        //public IActionResult AccountInfo(FormCollection id)
        //{
        //    var data = new Dictionary<string, string>();
        //    foreach (string key in id.Keys)
        //    {
        //        data.Add(key, id[key]);
        //    }
        //    DbHouseContext db = new DbHouseContext();
        //    //string temp = id["MerchantTradeNo"]; //寫在LINQ會出錯
        //    string temp = "1235614";
        //    var ecpayOrder = db.EcpayOrders.Where(m => m.MerchantTradeNo == temp).FirstOrDefault();
        //    //if (ecpayOrder != null)
        //    //{
        //        ecpayOrder.RtnCode = int.Parse(id["RtnCode"]);
        //        if (id["RtnMsg"] == "Succeeded") ecpayOrder.RtnMsg = "已付款";
        //        ecpayOrder.PaymentDate = Convert.ToDateTime(id["PaymentDate"]);
        //        ecpayOrder.SimulatePaid = int.Parse(id["SimulatePaid"]);
        //        db.SaveChanges();
        //    //}
        //    return View("EcpayView", data);
        //}


        //private readonly ILogger<PaymentController> _logger;

        //public PaymentController(ILogger<PaymentController> logger)
        //{
        //    _logger = logger;
        //}

    }
}
