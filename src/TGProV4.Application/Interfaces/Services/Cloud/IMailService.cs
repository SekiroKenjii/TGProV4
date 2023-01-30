namespace TGProV4.Application.Interfaces.Services.Cloud;

public interface IMailService
{
    Task Send(MailRequest request);
}
