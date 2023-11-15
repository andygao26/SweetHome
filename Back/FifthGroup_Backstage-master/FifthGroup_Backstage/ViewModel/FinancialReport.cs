using Microsoft.AspNetCore.Mvc.Rendering;

namespace FifthGroup_Backstage.ViewModel
{
    public class FinancialReport
    {

        public int PaymentItemCode { get; set; }
        public string PaymentName { get; set; }
        public string CommunityBuildingName { get; set; }
        public int TotalAmountToReceive { get; set; }//應收款
        public int TotalAmountReceived { get; set; }//實際收到
        public DateTime PaymentItemDate { get; set; }
        public SelectList BuildingList { get; internal set; }
    }
}
