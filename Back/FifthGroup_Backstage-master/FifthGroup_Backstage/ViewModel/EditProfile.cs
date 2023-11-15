using System.ComponentModel.DataAnnotations;

namespace FifthGroup_Backstage.ViewModel
{
    public class EditProfile
    {
    
            public string UserId { get; set; }
            public string UserName { get; set; }
            public string UserPwd { get; set; }
            public string UserEmail { get; set; }
        public string UserPhone { get; set; } // Add UserPhone property
        public string? UserAddress { get; set; }

        public string? UserPhoto { get; set; }



    }










}
