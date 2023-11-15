using System;
using System.Collections.Generic;

namespace FifthGroup_Backstage.Models;

public partial class PaymentItem
{
    public int PaymentItemCode { get; set; }

    public int CommunityBuildingId { get; set; }

    public int ItemClassificationCode { get; set; }

    public string PaymentName { get; set; } = null!;

    public DateTime Date { get; set; }

    public int Amount { get; set; }

    public bool Ispushed { get; set; }

    public string? Remark { get; set; }

    public virtual CommunityBuilding CommunityBuilding { get; set; } = null!;

    public virtual PaymentItemsName ItemClassificationCodeNavigation { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
