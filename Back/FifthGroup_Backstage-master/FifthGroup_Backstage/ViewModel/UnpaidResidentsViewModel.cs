using X.PagedList;

namespace FifthGroup_Backstage.ViewModel
{
    public class UnpaidResidentsViewModel
    {
       
            public string HouseholdCode { get; set; }
            public string PaymentCategory { get; set; }
            public DateTime PayDay { get; set; }
        public string Amount { get; set; }
        public string ExpiredDay { get; internal set; }



    }
}
