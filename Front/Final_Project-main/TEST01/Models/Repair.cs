using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // 確保引入 System.ComponentModel.DataAnnotations 命名空間，底下的ErrorMessage才可以用
using System.ComponentModel.DataAnnotations.Schema;

namespace FifthGroup_front.Models;

public partial class Repair
{
    public int RepairCode { get; set; }


    [Required(ErrorMessage = "戶號不可為空白，請填寫住家戶號!")]
    public string HouseholdCode { get; set; }

    [Required(ErrorMessage = "姓名不可為空白，請填寫!")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "電話不可為空白，請填寫(參考格式:0972468159)")]
    public string Phone { get; set; } = null!;

    [Required(ErrorMessage = "請選擇報修項目，此攔不能空白!")]
    public string Type { get; set; } = null!;

    [Required(ErrorMessage = "請選擇時間，此攔不能空白!")]
    public DateTime? Time { get; set; }

    public string? ProcessingStatus { get; set; }

    public string? ManufacturerCode { get; set; }

    public string? Detail { get; set; }

    public string? Pic { get; set; }

    public string? ProcessingStatusDetail { get; set; }

    public virtual Resident HouseholdCodeNavigation { get; set; } = null!;


    //for照片上傳用
    [NotMapped]
    public IFormFile photo { get; set; }
}