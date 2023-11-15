using Microsoft.AspNetCore.Mvc.Rendering;

namespace FifthGroup_Backstage.ViewModel
{
    public class EditBlogPostRequest
    {
        public int Id { get; set; }
        public string Heading { get; set; }//標題
        public string PageTitle { get; set; }//頁面標題
        public string Content { get; set; }//內容
        public string ShortDescription { get; set; }//簡短介紹
        public string FeacturedImageUrl { get; set; }//圖片網址
        public string UrlHandle { get; set; }
        public DateTime PublishedDate { get; set; }//公告建立時間
        public string Author { get; set; }//作者(發布人)
        public bool Visible { get; set; }//公告顯示

        //Display Tags
        public IEnumerable<SelectListItem> Tags { get; set; }
        //Collect Tags
        public string[] SelectedTags { get; set; } = Array.Empty<string>();
    }
}
