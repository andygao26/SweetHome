
using System.Drawing;

namespace FifthGroup_Backstage.ViewModel
{
    public class AdminModel
    {


        /// 註冊參數
        public class DoRegisterIn
        {
            public string UserID { get; set; }
            public string UserPwd { get; set; }
            public string UserName { get; set; }
            public string UserEmail { get; set; }

            public String CommunityCode { get; set; }
        }


        /// 註冊回傳
        public class DoRegisterOut
        {
            public string ErrMsg { get; set; }
            public string ResultMsg { get; set; }
        }


        /// 登入參數

        public class DoLoginIn
        {
            public string UserID { get; set; }
            public string UserPwd { get; set; }
        }


        /// 登入回傳

        public class DoLoginOut
        {
            public string ErrMsg { get; set; }
            public string ResultMsg { get; set; }
        }

        /// 取得個人資料回傳
        public class GetUserProfileOut
        {
            public string ErrMsg { get; set; }
            public string UserID { get; set; }
            public string UserName { get; set; }
            public string UserEmail { get; set; }
        }

    }
}
