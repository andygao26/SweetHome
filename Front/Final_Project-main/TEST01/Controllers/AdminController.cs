using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using FifthGroup_front.Attributes;
using FifthGroup_front.Models;
using Microsoft.EntityFrameworkCore;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using FifthGroup_front.ViewModels;
using Microsoft.AspNetCore.Http;//
using Microsoft.CodeAnalysis.Scripting;
using XAct.Users;
using FifthGroup_front.Interfaces;
using System.Runtime;

namespace FifthGroup_front.Controllers
{
    public class AdminController : Controller
    {
        private readonly IWebHostEnvironment _enviro;
        private readonly EmailService _emailService;
        private readonly IEmailService emailService;
        private readonly IPasswordHasher<Resident> _passwordHasher;
        private readonly DbHouseContext dbHouseContext;        
        public AdminController(IPasswordHasher<Resident> passwordHasher, IWebHostEnvironment p, EmailService
emailService, IEmailService ALLemailService)
        {
            _passwordHasher = passwordHasher;
            _enviro = p;
            _emailService = emailService;
            this.emailService = ALLemailService;
            this.dbHouseContext = dbHouseContext;//
        }
        public void ConfigureServices(IServiceCollection services)//
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IWebHostEnvironment>(env => env.GetRequiredService<IWebHostEnvironment>());

        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Admin");
        }

        [WithoutAuthentication]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string Email, string Password)/*, string Password2*/
        {
            using (var db = new DbHouseContext())
            {
                var user = db.Residents.FirstOrDefault(r => r.Email == Email);

                if (user == null)
                {
                    ViewBag.ErrorMessage = "帳號或密碼輸入錯誤";
                    return View();
                }
                //string hashedPassword = ToMD5(Password);

                if (user.Password == ToMD5(Password))/* && ToMD5(Password) == ToMD5(Password2)*/
                {
                    HttpContext.Session.SetString("UserHouseholdCode", user.HouseholdCode);
                    HttpContext.Session.SetString("UserName", user.Name);
                    HttpContext.Session.SetString("UserPhone", user.Phone);
                    HttpContext.Session.SetString("UserEmail", user.Email);
                    HttpContext.Session.SetString("UserPassword", user.Password);
                    
                    string imagePath = "/img/" + user.Headshot;
                    HttpContext.Session.SetString("MemberImg", imagePath);

                    HttpContext.Session.SetString("LogIn", "true");
                    return RedirectToAction("Index", "Home");
                }
            }
            ViewBag.ErrorMessage = "帳號或密碼輸入錯誤";
            return View();
        }
        public string ToMD5(string strs)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.Default.GetBytes(strs);
            byte[] encryptdata = md5.ComputeHash(bytes);
            return Convert.ToBase64String(encryptdata);
        }
        [WithoutAuthentication]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(Resident model)//AdminRegisterViewModel
        {
            using (var db = new DbHouseContext())
            {            
                    var RealUser = db.Residents.FirstOrDefault(r => r.Email == model.Email || r.HouseholdCode == model.HouseholdCode);
                    if (RealUser != null)
                    {
                        ViewBag.ErrorMessage = "帳號已被註冊，請直接登入";
                        return View("Login", model);
                    }
                    string hashedPassword = ToMD5(model.Password);
                    var newUser = new Resident
                    {
                        HouseholdCode = model.HouseholdCode,
                        CommunityBuildingId = model.CommunityBuildingId,
                        FloorNumber = model.FloorNumber,
                        UnitNumber = model.UnitNumber,
                        Name = model.Name,
                        Phone = model.Phone,
                        Password = hashedPassword,
                        Email = model.Email,
                        //VerifyCode = model.VerifyCode,                      
                        Headshot = model.Headshot,
                        
                        //Status = model.Status
                    };
                   
                    db.Residents.Add(newUser);
                    db.SaveChanges();
                   
                    ViewBag.SuccessMessage = "註冊成功";
                    return View("Login");
            }
        }
        [HttpPost]
        public JsonResult GetBuildingDetails(int cb)
        {
            var search = dbHouseContext.CommunityBuildings
                   .Where(s => s.CommunityBuildingId == cb)
                   .Select(s => new { s.FloorNumber, s.UnitNumber })
                   .FirstOrDefault();
            return Json(search);
        }
        public IActionResult ForgetPwd()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ForgetPwd(string Email)
        {         
            DbHouseContext db = new DbHouseContext();
            Resident user = db.Residents.FirstOrDefault(t => t.Email == Email);
            if (user == null)
            {
                ViewBag.ErrorMessage = "此信箱非本系統會員";
            }
            else
            {
                string randomPassword = RandomPasswordGenerator.GeneratePassword(12); //
                _emailService.SendEmailAsync(Email, "住戶更改密碼", $"您的新密碼是：{randomPassword}");
                user.Password = ToMD5(randomPassword);
                ViewBag.SuccessMessage = "系統已重設密碼，請至信箱查看";
                db.SaveChanges();
            }
            return View();
        }
        public IActionResult EditPwd()
        {
            var userProfile = new Resident
            {          
                
                Password = HttpContext.Session.GetString("UserPassword"),
            };
            return View();
        }
        [HttpPost]
        public IActionResult EditPwd(Resident EditPwd)
        {        
            using (var db = new DbHouseContext())
            {
                var user = db.Residents.Find(EditPwd.HouseholdCode);

                user.Password = ToMD5(EditPwd.Password);
                db.Residents.Update(user);
                db.SaveChanges();
            }
            HttpContext.Session.SetString("UserPassword", EditPwd.Password);
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Edit()
        {
            var userProfile = new Resident
            {
                //CommunityBuildingId = HttpContext.Session.GetInt32("CommunityBuildingId"),
                Name = HttpContext.Session.GetString("UserName"),
                Phone = HttpContext.Session.GetString("UserPhone"),
                Email = HttpContext.Session.GetString("UserEmail"),
                //Headshot = HttpContext.Session.GetString("UserHeadshot"),
            };
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Resident EditInfo)
        {
            async Task<string> UploadSingleImage(IFormFile file, string folderPath)
            {
                string photoName = Guid.NewGuid().ToString() + ".jpg";
                string filePath = Path.Combine(folderPath, photoName);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return photoName;
            }
            Resident resident = null;
            string folderPath = _enviro.WebRootPath + "/img/";

            using (var db = new DbHouseContext())
            {
                var user = db.Residents.Find(EditInfo.HouseholdCode);
                //var frontFile = Request.Form.Files[0];
                //resident.Headshot = await UploadSingleImage(frontFile, folderPath);
                if (Request.Form.Files.Count == 0)
                {
                    EditInfo.Headshot = user.Headshot;
                }
                else
                {
                    var frontFile = Request.Form.Files[0];
                    user.Headshot = await UploadSingleImage(frontFile, folderPath);
                }
                if (user != null)
                {
                    user.Name = EditInfo.Name;
                    user.Phone = EditInfo.Phone;
                    user.Email = EditInfo.Email;
                    //user.HouseholdCode = EditInfo.HouseholdCode;
                    //user.CommunityBuildingId = EditInfo.CommunityBuildingId;
                    //user.FloorNumber = EditInfo.FloorNumber;
                    //user.UnitNumber = EditInfo.UnitNumber;
                    user.Password = EditInfo.Password;
                    user.Headshot = user.Headshot;                
               
                    db.Residents.Update(user);
                    await db.SaveChangesAsync();
                }
                HttpContext.Session.SetString("UserName", EditInfo.Name);
                ////HttpContext.Session.SetString("UserPersonId", EditInfo.PersonId);
                HttpContext.Session.SetString("UserPhone", EditInfo.Phone);
                HttpContext.Session.SetString("UserEmail", EditInfo.Email);
                HttpContext.Session.SetString("UserPassword", EditInfo.Password);
                string imagePath = "/img/" + user.Headshot;
                HttpContext.Session.SetString("MemberImg", imagePath);
                return View("Edit");
            }
        }
    }
}