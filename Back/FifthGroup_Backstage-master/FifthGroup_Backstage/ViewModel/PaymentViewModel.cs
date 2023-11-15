using FifthGroup_Backstage.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace FifthGroup_Backstage.ViewModel
{
    public class PaymentListViewModel
    {

        public IEnumerable<PaymentItem> PaymentItem { get; set; }
        public IPagedList<PaymentItem> PagedPaymentItems { get; set; }

        public IEnumerable<CommunityBuilding> CommunityBuilding { get; set; }

        public List<SelectListItem> Buildings { get; set; }
    }



    public class PaymentViewModel
    {

        public PaymentItem PaymentItem { get; set; }
        public IEnumerable<PaymentItemsName> PaymentItemsName { get; set; }

        public IEnumerable<CommunityBuilding> CommunityBuilding { get; set; }
    }

    public class PaymentDetailViewModel
    {

        public IEnumerable<Payment> Payment{ get; set; }
        public PaymentItem PaymentItem { get; set; }
        public CommunityBuilding CommunityBuilding { get; set; }
    }
}
