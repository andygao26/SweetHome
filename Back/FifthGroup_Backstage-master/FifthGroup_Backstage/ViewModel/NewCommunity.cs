namespace FifthGroup_Backstage.ViewModel
{
    public class NewCommunity
    {

        public int CommunityId { get; set; }


        public string CommunityName { get; set; }

        public int TotalUnits { get; set; }


        public string VerificationCode { get; set; }

        public string Address { get; set; }

        public string city { get; set; } // 用來匹配 twzipcode 生成的 city 的值
        public string town { get; set; } // 用來匹配 twzipcode 生成的 town 的值
    }
}
