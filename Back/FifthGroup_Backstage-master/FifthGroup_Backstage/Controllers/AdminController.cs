using FifthGroup_Backstage.Models;
using FifthGroup_Backstage.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using static FifthGroup_Backstage.ViewModel.AdminModel;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using FifthGroup_Backstage.Repositories;

namespace FifthGroup_Backstage.Controllers
{
    public class AdminController : Controller
    {
        private readonly DbHouseContext dbHouseContext;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly EmailService emailService;
        //----------------------------*****建構式*****----------------------------//
        public AdminController(DbHouseContext dbHouseContext, IHttpContextAccessor httpContextAccessor,EmailService emailService)
        {
            this.dbHouseContext = dbHouseContext;
            this.httpContextAccessor = httpContextAccessor;
            this.emailService=emailService;
        }
        //----------------------------*****建構式End*****----------------------------//

        //----------------------------*****登入Start*****----------------------------//
        //登入
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        //登入
        public IActionResult DoLogin(DoLoginIn inModel)
        {
            DoLoginOut outModel = new DoLoginOut();

            // 檢查輸入數據
            if (string.IsNullOrEmpty(inModel.UserID) || string.IsNullOrEmpty(inModel.UserPwd))
            {
                outModel.ErrMsg = "請輸入帳號及密碼";
            }
            else
            {
                try
                {
                    // 加密
                    string salt = inModel.UserID.Substring(0, 1).ToLower();
                    SHA256 sha256 = SHA256.Create();
                    byte[] bytes = Encoding.UTF8.GetBytes(salt + inModel.UserPwd);
                    byte[] hash = sha256.ComputeHash(bytes);
                    string CheckPwd = BitConverter.ToString(hash).Replace("-", "").ToLower();

                    // 檢查帳號密碼是否正確
                    var user = dbHouseContext.Admins
                        .Where(m => m.UserId == inModel.UserID && m.UserPwd == CheckPwd)
                        .FirstOrDefault();

                    if (user != null)
                    {
                        // 檢查帳號是否已驗證
                        if ((bool)!user.IsVerified)
                        {
                            outModel.ErrMsg = "帳號尚未驗證，請先驗證您的電子郵件";
                            return Json(outModel);
                        }

                        // 找到匹配的用戶，表示帳號密碼正確
                        // 將登錄帳號紀錄在Session中
                        HttpContext.Session.SetString("UserID", inModel.UserID);

                        // 從DB中獲得UserPhoto并将其存储在Session中
                        string userPhotoUrl = dbHouseContext.Admins
                            .Where(m => m.UserId == inModel.UserID)
                            .Select(m => m.UserPhoto)
                            .FirstOrDefault();

                        HttpContext.Session.SetString("UserPhoto", userPhotoUrl);

                        outModel.ResultMsg = "登入成功：" + inModel.UserID;
                      

                     

                    }
                    else
                    {
                        // 帳號或密碼有誤
                        outModel.ErrMsg = "帳號或密碼有誤";
                    }
                }
                catch (Exception ex)
                {
                    // 異常處理
                    outModel.ErrMsg = ex.Message;
                }
            }

            // 輸出為JSON
            return Json(outModel);
        }
        //----------------------------*****登入End*****----------------------------//




        //----------------------------*****登出*****----------------------------//

        public IActionResult Logout()
        {
            // 清除Session中的UserID和UserPhoto
            HttpContext.Session.Remove("UserID");
            HttpContext.Session.Remove("UserPhoto");

            // 可以根据需要执行其他登出操作，例如重定向到登录页面或者显示一条成功登出的消息。

            return RedirectToAction("Login", "Admin"); // 重定向到登录页面
        }
        //----------------------------*****登出*****----------------------------//



        //----------------------------*****註冊Start*****----------------------------//
        //註冊
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        //註冊
        public ActionResult DoRegister(DoRegisterIn inModel)
        {
            DoRegisterOut outModel = new DoRegisterOut();

            if (string.IsNullOrEmpty(inModel.UserID) || string.IsNullOrEmpty(inModel.UserPwd) || string.IsNullOrEmpty(inModel.UserName) || string.IsNullOrEmpty(inModel.UserEmail) || string.IsNullOrEmpty(inModel.CommunityCode))
            {
                outModel.ErrMsg = "請輸入所有資料";
            }
            else
            {
                try
                {
                    // 檢查帳號是否存在
                    var existingUser = dbHouseContext.Admins.FirstOrDefault(m => m.UserId == inModel.UserID);
                    if (existingUser != null)
                    {
                        outModel.ErrMsg = "此登入帳號已存在";
                    }
                    else
                    {
                        // 檢查社區驗證碼是否存在
                        var community = dbHouseContext.Communities.FirstOrDefault(c => c.VerificationCode == inModel.CommunityCode);
                        if (community == null)
                        {
                            outModel.ErrMsg = "無效的社區驗證碼";
                            return Json(outModel);
                        }

                        // 將密碼使用 SHA256 哈希算法（不可逆）
                        string salt = inModel.UserID.Substring(0, 1).ToLower();
                        string combinedPassword = salt + inModel.UserPwd;
                        using (SHA256 sha256 = SHA256.Create())
                        {
                            byte[] bytes = Encoding.UTF8.GetBytes(combinedPassword);
                            byte[] hash = sha256.ComputeHash(bytes);
                            string hashedPassword = BitConverter.ToString(hash).Replace("-", "").ToLower();
                            // 生成驗證碼
                            string code = Guid.NewGuid().ToString();

                            string UserPhone = "0900000000";
                            string UserPhoto = "https://res.cloudinary.com/dwdov2ta3/image/upload/v1694486698/Admin_Photo_pazizs.jpg";
                            string UserAddress = "台北市松山區";
                          


                            // 註冊數據存入DB
                            Admin newAdmin = new Admin
                            {
                                UserId = inModel.UserID,
                                UserPwd = hashedPassword,
                                UserName = inModel.UserName,
                                UserEmail = inModel.UserEmail,
                                VerificationCode = code,  // 儲存驗證碼
                                CommunityId = community.CommunityId,  // 儲存社區ID
                                UserPhoto = UserPhoto,
                                UserAddress=UserAddress,
                                UserPhone=UserPhone,
                                IsVerified=false,

                            };
                            dbHouseContext.Admins.Add(newAdmin);
                            dbHouseContext.SaveChanges();
                            // 建立驗證連結
                            string verificationLink = Url.Action("VerifyEmail", "Admin", new { code = code }, protocol: Request.Scheme);
                            // 發送包含驗證連結的電子郵件
                            emailService.SendMailByGmail(new List<string> { newAdmin.UserEmail }, "註冊成功", $"恭喜你成功註冊！請點擊以下連結以啟用你的帳號：{verificationLink}");

                            outModel.ResultMsg = "註冊完成,請至信箱點選驗證信以啟用帳號";
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 處理異常
                    outModel.ErrMsg = ex.Message;
                }
            }

            // 輸出為JSON
            return Json(outModel);
        }

        // 驗證電子郵件
        public ActionResult VerifyEmail(string code)
        {
            // 根據驗證碼查找用戶
            var user = dbHouseContext.Admins.FirstOrDefault(u => u.VerificationCode == code);

            if (user != null)
            {
                // 啟用帳號
                user.IsVerified = true;
                dbHouseContext.SaveChanges();

                return Content("帳號已啟用");
            }
            else
            {
                return Content("無效的驗證碼");
            }
        }
        //----------------------------*****註冊End*****----------------------------//


        //----------------------------*****忘記&重設密碼Start*****----------------------------//
        //忘記密碼
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //忘記密碼
        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            // 檢查電子郵件地址是否存在
            var admin = dbHouseContext.Admins.FirstOrDefault(u => u.UserEmail == email);
            if (admin == null)
            {
                return Content("無效的電子郵件地址");
            }

            // 生成重設密碼連結
            string code = Guid.NewGuid().ToString();
            string resetPasswordLink = Url.Action("ResetPassword", "Admin", new { code = code }, protocol: Request.Scheme);

            // 更新用戶的驗證碼
            admin.VerificationCode = code;
            dbHouseContext.SaveChanges();

            // 發送包含重設密碼連結的電子郵件
            EmailService emailService = new EmailService();
            emailService.SendMailByGmail(new List<string> { admin.UserEmail }, "重設密碼", $"請點擊以下連結以重設您的密碼：{resetPasswordLink}");
            return Content("已發送重設密碼的電子郵件");

        }

        //重設密碼
        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        //重設密碼
        public ActionResult ResetPassword(string code, string newPassword)
        {
            // 從資料庫中查找與代碼相對應的使用者
            var user = dbHouseContext.Admins.FirstOrDefault(u => u.VerificationCode == code);
            if (user == null)
            {
                // 如果找不到該使用者，返回一個錯誤頁面
                return View("Register");
            }

            // 將密碼使用 SHA256 哈希算法（不可逆）
            string salt = user.UserId.Substring(0, 1).ToLower();
            string combinedPassword = salt + newPassword;
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(combinedPassword);
                byte[] hash = sha256.ComputeHash(bytes);
                string hashedPassword = BitConverter.ToString(hash).Replace("-", "").ToLower();

                // 更新使用者的密碼
                user.UserPwd = hashedPassword;
                dbHouseContext.SaveChanges();
            }

            // 顯示一個成功頁面
            return View("Login");
        }
        //----------------------------*****忘記&重設密碼End*****----------------------------//


        //----------------------------*****管理者編輯個人檔案Start*****----------------------------//
        //修改管理人員個人檔案
        [HttpGet]
        public ActionResult EditProfile()
        {
            // 从Session中获取UserID
            string userID = HttpContext.Session.GetString("UserID");

            if (!string.IsNullOrEmpty(userID))
            {
                // 根据UserID从数据库中检索Admin记录
                var admin = dbHouseContext.Admins.FirstOrDefault(m => m.UserId == userID);

                if (admin != null)
                {
                    // 创建EditProfileViewModel并填充数据
                    var editProfileViewModel = new EditProfile
                    {
                        UserId = admin.UserId,
                        UserName = admin.UserName,
                        UserPwd = admin.UserPwd,
                        UserEmail = admin.UserEmail,
                        UserPhone = admin.UserPhone,
                        UserAddress = admin.UserAddress,
                        UserPhoto=admin.UserPhoto,
                        
                    };

                    // 将获取到的信息传递给视图
                    return View(editProfileViewModel);
                }
            }

            // 如果没有找到数据或Session中没有UserID，可以执行适当的操作
            return View(new EditProfile()); // 返回一个新的EditProfile模型对象
        }

        //更改個人資料
        [HttpPost]
        public ActionResult UpdateUserProfile([FromBody] EditProfile model)
        {
            try
            {
                // 获取当前用户的UserID
                string userID = HttpContext.Session.GetString("UserID");

                // 根据UserID从数据库中检索Admin记录
                var admin = dbHouseContext.Admins.FirstOrDefault(m => m.UserId == userID);

                if (admin != null)
                {
                    // 更新管理员信息
                    admin.UserEmail = model.UserEmail;// 更新 Email
                    admin.UserPhone = model.UserPhone; // 更新 UserPhone
                    admin.UserAddress=model.UserAddress; // 更新 Address


                    // 保存更改到数据库
                    dbHouseContext.SaveChanges();

                    // 返回成功响应
                    return Json(new { success = true });
                }
                else
                {
                    // 返回失败响应
                    return Json(new { success = false, message = "找不到用戶訊息，請登入帳號。" });
                }
            }
            catch (Exception ex)
            {
                // 返回失败响应，包括错误消息
                return Json(new { success = false, message = "更新個人資料時出錯：" + ex.Message });
            }
        }

        //更改密碼
        [HttpPost]
        public ActionResult ChangePassword(string newPassword)
        {
            // 获取当前用户的用户ID
            string userID = HttpContext.Session.GetString("UserID");

            if (!string.IsNullOrEmpty(userID) && !string.IsNullOrEmpty(newPassword))
            {
                try
                {
                    // 获取用户记录
                    var user = dbHouseContext.Admins.FirstOrDefault(m => m.UserId == userID);

                    if (user != null)
                    {
                        // 更新密码并保存到数据库
                        string salt = userID.Substring(0, 1).ToLower();
                        string combinedPassword = salt + newPassword;
                        using (SHA256 sha256 = SHA256.Create())
                        {
                            byte[] bytes = Encoding.UTF8.GetBytes(combinedPassword);
                            byte[] hash = sha256.ComputeHash(bytes);
                            string hashedPassword = BitConverter.ToString(hash).Replace("-", "").ToLower();

                            user.UserPwd = hashedPassword;
                            dbHouseContext.SaveChanges();

                            // 返回成功消息；回到首頁
                            return RedirectToAction("Login");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 处理异常
                    return Json(new { success = false, message = ex.Message });
                }
            }

            return Json(new { success = false, message = "密碼更改失敗" });
        }

        //換大頭貼
        [HttpPost]
        public async Task<IActionResult> UpdateUserPhoto(string UserPhoto)
        {
            // 將DataURL轉換為byte陣列
            var base64Data = Regex.Match(UserPhoto, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            var binData = Convert.FromBase64String(base64Data);

            // 儲存檔案到伺服器
            var fileName = Guid.NewGuid().ToString() + ".jpg";
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
            System.IO.File.WriteAllBytes(path, binData);

            // 更新資料庫
            string userID = HttpContext.Session.GetString("UserID");
            var admin = await dbHouseContext.Admins.FindAsync(userID);
            if (admin != null)
            {
                admin.UserPhoto = "/images/" + fileName;
                dbHouseContext.Entry(admin).State = EntityState.Modified;
                await dbHouseContext.SaveChangesAsync();
            }

            return RedirectToAction("EditProfile");
        }
        //----------------------------****管理者編輯個人檔案End*****----------------------------//









    }


}
