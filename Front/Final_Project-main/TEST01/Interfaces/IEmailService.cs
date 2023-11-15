using FifthGroup_front.ViewModels;

namespace FifthGroup_front.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(Mailrequest mailrequest);
    }
}
