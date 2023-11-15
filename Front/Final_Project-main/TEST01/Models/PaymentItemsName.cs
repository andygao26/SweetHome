using System;
using System.Collections.Generic;

namespace FifthGroup_front.Models;

public partial class PaymentItemsName
{
    public int ItemClassificationCode { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<PaymentItem> PaymentItems { get; set; } = new List<PaymentItem>();
}
