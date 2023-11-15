using System;
using System.Collections.Generic;

namespace FifthGroup_Backstage.Models;

public partial class Payment
{
    public int PaymentCode { get; set; }

    public string HouseholdCode { get; set; } = null!;

    public DateTime? PayDay { get; set; }

    public int PaymentItemCode { get; set; }

    public int Amount { get; set; }

    public bool Paid { get; set; }

    public string? MerchantTradeNo { get; set; }

    public virtual Resident HouseholdCodeNavigation { get; set; } = null!;

    public virtual EcpayOrder? MerchantTradeNoNavigation { get; set; }

    public virtual PaymentItem PaymentItemCodeNavigation { get; set; } = null!;
}
