using FifthGroup_Backstage.Models;
using System.ComponentModel.DataAnnotations; // 確保引入 System.ComponentModel.DataAnnotations 命名空間，底下的ErrorMessage才可以用

namespace FifthGroup_Backstage.ViewModel
{
    public class CEmails
    {

        public Repair Repair { get; set; }

        public Email Email { get; set; }

        //Email Model

        public int EmailCode { get; set; }

        [Required(ErrorMessage = "戶號不可為空白或零，請填寫住家戶號!")]
        public string HouseholdCode { get; set; } = null!;

        [Required(ErrorMessage = "主題不可為空白或零，請填寫!")]
        public string? Subject { get; set; }

        [Required(ErrorMessage = "寄件人郵箱不可為空白或零，請填寫!")]
        public string FromEmail { get; set; } = null!;

        [Required(ErrorMessage = "收件人郵箱不可為空白或零，請填寫!")]
        public string ToEmail { get; set; } = null!;

       
        public DateTime? Time { get; set; }

        
        public string? Body { get; set; }

        public bool IsRead { get; set; }

        public virtual Resident HouseholdCodeNavigation { get; set; } = null!;




        //Resident Model

        [Required(ErrorMessage = "收件者郵箱不可為空白或零，請填寫!")]
        public string ResidentEmail { get; set; }

        [Required(ErrorMessage = "住戶姓名不可為空白或零，請填寫!")]
        public string ResidentName { get; set; }

        public string ResidentHouseholdCode { get; set; }
    }
}
