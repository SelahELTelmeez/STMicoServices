using IdentityDomain.Models;
using IdentityDomain.Services;
using System.Net;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Text;
namespace IdentityInfrastructure.Services;
public class NotificationService : INotificationService
{
    private readonly HttpClient _SMSClient;
    public NotificationService(IHttpClientFactory factory)
    {
        _SMSClient = factory.CreateClient("SMSClient");
    }
    public async Task<bool> SendEmailAsync(EmailNotificationModel emailModel, CancellationToken cancellationToken)
    {

        MailMessage emailMessage = new MailMessage()
        {
            From = new MailAddress(emailModel.MailFrom, emailModel.DisplayName, Encoding.UTF8),
            Subject = emailModel.MailSubject,
            SubjectEncoding = Encoding.UTF8,
            BodyEncoding = Encoding.UTF8,
            IsBodyHtml = emailModel.IsBodyHtml,
            Priority = MailPriority.High,
            Body = GenerateHtmlEmailBody(emailModel.MailToName, emailModel.MailBody)
        };
        emailMessage.To.Add(emailModel.MailTo);
        SmtpClient client = new SmtpClient()
        {
            Credentials = new NetworkCredential(emailModel.MailFrom, "Smtp@Stpp#2030"),
            Port = 587,
            Host = "smtp.office365.com",
            EnableSsl = true
        };
        try
        {
            await client.SendMailAsync(emailMessage, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }

    }

    public async Task<bool> SendSMSAsync(SMSNotificationModel SMSMessage, CancellationToken cancellationToken)
    {
        HttpResponseMessage httpResponseMessage = await _SMSClient.PostAsJsonAsync("/api/OTPV2", new SMSNotificationModel
        {
            Mobile = SMSMessage.Mobile,
            Code = SMSMessage.Code,
            UserName = "r3kbTRQ1",
            Password = "obHGhCjSef",
            Msignature = "3933754345",
            Token = "75b0e32c-f615-423b-8102-c9c88d7b3700"
        }, cancellationToken);
        return httpResponseMessage.IsSuccessStatusCode;
    }
    private string GenerateHtmlEmailBody(string username, string otp)
        => string.Format("<body style='font: small/1.5 monospace;direction: rtl;'> <table style='max-width:720px' width='100%' cellspacing='0' cellpadding='0' border='0' align='center'> <tbody> <tr> <td valign='top'> <table style='width:90%;margin:auto;height:auto;min-height:230px;border:1px solid rgb(243,243,243);padding:0px;padding-bottom:0px;background-color:#fff' width='100%' cellspacing='0' cellpadding='0' border='0' align='center'> <tbody> <tr> <td style='padding:10px' valign='top' align='center'> <center> <table style='width:100%;margin:auto;height:auto;min-height:230px;border:0px solid rgb(243,243,243); padding-bottom:0px;background-color:#fff' width='100%' cellspacing='0' cellpadding='0' border='0' align='center'> <tbody> <tr> <td style='padding:0px' valign='top' align='center'> <table width='100%' cellspacing='0' cellpadding='0' border='0' align='center'> <tbody> <tr> <td style='padding:0' valign='top' align='center'> <a> <img style='display:block;border-radius:4px;padding: 0 0 10px 0;' class='CToWUd' width='85px' height='85px' border='0' src='https://www.selaheltelmeez.com/ExamTerm1/Ara/Q/files/extfile/logo.png' alt='' class='CToWUd' border='0'> </a> <p style='line-height:24px;padding:0px;padding-bottom:5px;font-size:24px;font-weight:bold;margin:0px;direction: rtl;'> مرحباً {0}</p> <p style='margin:0px;padding:0px;padding-bottom:5px;font-size:18px'> الخطوة الاخيرة لبدء رحلتك التعليمة <br>أدخل الرمز الآتي لكي تتمكن من تفعيل الحساب الخاص بك على منصة سلاح التلميذ</p> </td> </tr> </tbody> </table> <table width='100%' cellspacing='0' cellpadding='0' border='0' align='center'> <tbody> <tr> <td align='center'> <a href='{1}' style='background-color:#4b4c4e;width:200px;font-size:19px;text-decoration:none;border:0 none;border-radius:3px;color:#fff;display:inline-block;line-height:26px;padding:10px 10px;margin-bottom:10px;margin-top:5px;text-align:center' target='_blank'>{1}</a> </td> </tr> </tbody> </table> <div style='height:5px'> </div> <table width='100%' cellspacing='0' cellpadding='0' border='0' align='center'> <tbody> <tr align='center'> <td align='center'> <p align='center' style='margin:0px;padding:5px 0;padding-bottom:5px;font-size:18px'> ملاحظة: إذا لم تكن قد تقدّمت بهذا الطلب، فلا تحتاج إلى فعل أي شيء، ولن تتلقى منا أي رسائل أخرى. هذه رسالة آلية، يرجى عدم الرد على هذه الرسالة. لمزيد من الإستفسارات والملاحظات قم بمراجعة 'مركز المساعدة' عبر منصة سلاح التلميذ. </p> </td> </tr> </tbody> </td> </tr> </tbody> </table> </td> </tr> </tbody> </table> </center> </td> </tr> </tbody>", username, otp);
}
