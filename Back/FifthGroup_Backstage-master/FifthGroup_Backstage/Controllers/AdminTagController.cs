using FifthGroup_Backstage.Models;
using FifthGroup_Backstage.Repositories;
using FifthGroup_Backstage.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace FifthGroup_Backstage.Controllers
{
    public class AdminTagController : Controller
    {
        private readonly ITagRepository tagRepository;

        public AdminTagController(ITagRepository tagRepository)
        {
            this.tagRepository = tagRepository;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddTagRequest addTagRequest)
        {
            //Mapping AddTagRequest to Tag domain model
            var tag = new Tag
            {
                Name = addTagRequest.Name,
                DisplayName = addTagRequest.DisplayName,
            };
            await tagRepository.AddAsync(tag);

            return RedirectToAction("List");
        }
        [HttpGet]
        [ActionName("List")]
        public async Task<IActionResult> List()
        {
            //use dbContext to read the tags
            var tags = await tagRepository.GetAllAsync();
            return View(tags);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var tag = await tagRepository.GetAsync(id);

            if (tag != null)//找到符合條件的標籤，則將標籤的屬性值（Id、Name、DisplayName）填充到一個新的 EditTagRequest 物件中
            {
                var editTagRequest = new EditTagRequest
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    DisplayName = tag.DisplayName,
                };
                return View(editTagRequest);//將填充好的 EditTagRequest 物件傳遞給一個名為 Edit 的視圖（View）中，用於顯示編輯表單。
            }
            return View(null);//如果找不到符合條件的標籤，則返回一個空的視圖（View）。
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditTagRequest editTagRequest)
        {
            var tag = new Tag
            {
                Id = editTagRequest.Id,
                Name = editTagRequest.Name,
                DisplayName = editTagRequest.DisplayName,
            };

            var updatedTag = await tagRepository.UpdateAsync(tag);

            if (updatedTag != null)
            {
                TempData["SuccessMessage"] = "Tag updated successfully!";
                return RedirectToAction("List"); // 將 "List" 替換成你的列表頁面的 Action 名稱
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update tag.";
            }

            return View(editTagRequest); // 若更新失敗，保留在 Edit 頁面
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditTagRequest editTagRequest)
        {

            var deletedTag = await tagRepository.DeleteAsync(editTagRequest.Id);
            if (deletedTag != null)
            {
                //show success notification
                return RedirectToAction("List");
            }

            //show error notification
            return RedirectToAction("Edit", new { id = editTagRequest.Id });


        }
    }
}
