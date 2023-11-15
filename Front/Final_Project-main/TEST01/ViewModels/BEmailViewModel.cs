namespace FifthGroup_front.ViewModels
{
    public class BEmailViewModel
    {
        public const string Name = "SmtpSettings";
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
