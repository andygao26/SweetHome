using FifthGroup_front.Models;
using X.PagedList;

namespace FifthGroup_front.ViewModels
{
    public class PaymentViewModel
    {
        public IEnumerable<Payment> House_P { get; set; }
        public IEnumerable<Payment> Manage_P { get; set; }
        public IEnumerable<Payment> Else_P { get; set; }

        public IEnumerable<PaymentItem> PaymentItems { get; set; }

    }

    public class PaymentHistory
    {

        public IPagedList<Payment> PagedPayments { get; set; }
     
        public IEnumerable<Payment> Payments { get; set; }
        public IEnumerable<PaymentItem> PaymentItems { get; set; }
        public IEnumerable<PaymentItemsName> PaymentItemsName { get; internal set; }
    }


}
