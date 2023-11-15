using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FifthGroup_Backstage.Models;
using X.PagedList; // Add this namespace for pagination.
using FifthGroup_Backstage.ViewModel;

namespace FifthGroup_Backstage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepairsAPIController : ControllerBase
    {
        private readonly DbHouseContext _db;
        private readonly IWebHostEnvironment _enviro;

        public RepairsAPIController(DbHouseContext db, IWebHostEnvironment enviro)
        {
            _db = db;
            _enviro = enviro;
        }

        [HttpGet("RepairInquire")]
        public IActionResult RepairInquire([FromQuery] CKeywordViewModel vm, string txtKeywordItems, string txtKeywordState, int? Page)
        {
            IQueryable<Repair> datas = _db.Repairs; // Starting with all repairs

            if (!string.IsNullOrEmpty(vm.txtKeyword))
            {
                if (int.TryParse(vm.txtKeyword, out int keywordAsInt))
                {
                    datas = datas.Where(t => t.RepairCode == keywordAsInt);
                }
                else
                {
                    datas = datas.Where(t =>
                        t.HouseholdCode.Contains(vm.txtKeyword) ||
                        t.Name.Contains(vm.txtKeyword) ||
                        t.Phone.Contains(vm.txtKeyword)
                    );
                }
            }

            if (!string.IsNullOrEmpty(txtKeywordItems))
            {
                datas = datas.Where(t => t.Type == txtKeywordItems);
            }

            if (!string.IsNullOrEmpty(txtKeywordState))
            {
                datas = datas.Where(t => t.ProcessingStatus == txtKeywordState);
            }

            if (DateTime.TryParse(vm.txtKeywordDate, out DateTime keywordAsDateTime))
            {
                datas = datas.Where(t => t.Time.HasValue && t.Time.Value.Date == keywordAsDateTime.Date);
            }

            int pageNumber = Page.HasValue ? Page.Value : 1;
            var varResultList = datas.OrderBy(x => x.RepairCode).ToPagedList(pageNumber, 10);

            return Ok(varResultList);
        }

        [HttpGet("CheckTheDetails/{id}")]
        public IActionResult CheckTheDetails(int id)
        {
            Repair repair = _db.Repairs.FirstOrDefault(t => t.RepairCode == id);
            if (repair == null)
                return NotFound();

            return Ok(repair);
        }

        [HttpPost("Create")]
        public IActionResult Create([FromBody] Repair p)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(p.HouseholdCode) || p.HouseholdCode == "0" ||
                string.IsNullOrWhiteSpace(p.Name) || p.Name == "0" ||
                string.IsNullOrWhiteSpace(p.Phone) || p.Phone == "0" ||
                string.IsNullOrWhiteSpace(p.Type) || p.Type == "0" ||
                !p.Time.HasValue)
            {
                if (string.IsNullOrWhiteSpace(p.HouseholdCode) || p.HouseholdCode == "0")
                {
                    ModelState.AddModelError("HouseholdCode", "戶號不可為空白或零，請填寫住家戶號!");
                }

                if (string.IsNullOrWhiteSpace(p.Name) || p.Name == "0")
                {
                    ModelState.AddModelError("Name", "姓名不可為空白或零，請填寫!");
                }

                if (string.IsNullOrWhiteSpace(p.Phone) || p.Phone == "0")
                {
                    ModelState.AddModelError("Phone", "電話不可為空白或零，請填寫(參考格式:0972468159)");
                }

                if (string.IsNullOrWhiteSpace(p.Type) || p.Type == "0")
                {
                    ModelState.AddModelError("Type", "請選擇報修項目，此欄不能空白!");
                }

                if (!p.Time.HasValue)
                {
                    ModelState.AddModelError("Time", "請選擇時間，此欄不能空白!");
                }

                return BadRequest(ModelState);
            }

            if (p.photo != null && _enviro != null)
            {
                string photoName = Guid.NewGuid().ToString() + ".jpg";
                string path = Path.Combine(_enviro.WebRootPath, "img", photoName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    p.photo.CopyTo(stream);
                }
                p.Pic = photoName;
            }

            _db.Repairs.Add(p);
            _db.SaveChanges();
            return Ok(p);
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            Repair repair = _db.Repairs.FirstOrDefault(t => t.RepairCode == id);
            if (repair != null)
            {
                _db.Repairs.Remove(repair);
                _db.SaveChanges();
            }
            return NoContent();
        }

        [HttpPut("Edit/{id}")]
        public IActionResult Edit(int id, [FromBody] Repair repairIn)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Repair repairDb = _db.Repairs.FirstOrDefault(t => t.RepairCode == id);
            if (repairDb == null)
                return NotFound();

            if (repairIn.photo != null)
            {
                string photoName = Guid.NewGuid().ToString() + ".jpg";
                string path = Path.Combine(_enviro.WebRootPath, "img", photoName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    repairIn.photo.CopyTo(stream);
                }
                repairDb.Pic = photoName;
            }

            repairDb.HouseholdCode = repairIn.HouseholdCode;
            repairDb.Name = repairIn.Name;
            repairDb.Phone = repairIn.Phone;
            repairDb.Type = repairIn.Type;
            repairDb.Time = repairIn.Time;
            repairDb.ProcessingStatus = repairIn.ProcessingStatus;
            repairDb.Detail = repairIn.Detail;

            _db.SaveChanges();
            return Ok(repairDb);
        }
    }
}
