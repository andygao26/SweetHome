using FifthGroup_front.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace FifthGroup_front.Controllers
{
    public class ResidentController : Controller
    {
        public IActionResult ResidentData()
        {
            return View();
        }
        public IActionResult ResidentLogin()
        {
            return View();
        }
        public IActionResult UserLogin()
        {
            return View();
        }
        [HttpPost]
        public IActionResult UserLogin(Resident r)
        {
            //Resident user = (new DbHouseContext()).Residents.FirstOrDefault(
            //t => t.HouseholdCode.Equals(r.HouseholdCode) && t.Password.Equals(r.Password));

            ResidentRegister user = (new DbHouseContext()).ResidentRegisters.FirstOrDefault(
            t => t.Email.Equals(r.Email) && t.Password.Equals(r.Password));

            // 登
            if (user != null)
            {
                string json = JsonSerializer.Serialize(user);
                HttpContext.Session.SetString(CDictionary.SK_LOINGED_USER, json);
                //return RedirectToAction("ResidentData", new { id = user.RegisterCode });//個人資訊 暫定
                return RedirectToAction("Index");
            }
          
            ModelState.AddModelError("PersonId", "帳號或密碼錯誤。");
            return View(r);
        }
        public string ToMD5(string strs)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.Default.GetBytes(strs);
            byte[] encryptdata = md5.ComputeHash(bytes);
            return Convert.ToBase64String(encryptdata);
        }

        public IActionResult Register(ResidentRegister re)
        {
            ResidentRegister user = (new DbHouseContext()).ResidentRegisters.FirstOrDefault(
            t => t.Email.Equals(re.Email));
            if (user != null)
            {
                ViewBag.ShowAlert = true;
                return View();
            }
            DbHouseContext db = new DbHouseContext();
            if (re.Password != null)
            {
                string Password = ToMD5(re.Password);
                re.RegisterCode = re.RegisterCode;
                re.Name = re.Name;
                re.PersonId = re.PersonId;
                re.Phone = re.Phone;
                re.Password = Password;
                re.Email = re.Email;
                re.VerifyCode = re.VerifyCode;
                ////re.Hide = "value";
                db.ResidentRegisters.Add(re);
                db.SaveChanges();            
            }
           return View();
        }
    }
}

