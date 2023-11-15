using FifthGroup_Backstage.ViewModel;

namespace FifthGroup_Backstage.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(Mailrequest mailrequest);
    }
}
