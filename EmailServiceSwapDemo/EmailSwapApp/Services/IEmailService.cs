namespace EmailSwapApp.Services;

public interface IEmailService
{
    void SendEmail(string to, string subject, string body);
}

public class SmtpEmailService : IEmailService
{
    public void SendEmail(string to, string subject, string body)
    {
        Console.WriteLine($"[SMTP] Sending email to {to}: {subject}");
    }
}

public class SendGridEmailService : IEmailService
{
    public void SendEmail(string to, string subject, string body)
    {
        Console.WriteLine($"[SendGrid] Sending email to {to}: {subject}");
    }
}
