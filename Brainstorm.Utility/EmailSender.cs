using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Brainstorm.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // 1. Khai báo thông tin tài khoản Gmail của bạn
            var mail = "phamanhtienphat@gmail.com"; // Thay bằng Gmail của bạn
            var pw = "sfwp czny ubto mcjp";    // Thay bằng mật khẩu 16 chữ cái bạn vừa tạo

            // 2. Cấu hình máy chủ SMTP của Google
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true, // Bật SSL để bảo mật thông tin
                Credentials = new NetworkCredential(mail, pw)
            };

            // 3. Tiến hành gửi email
            return client.SendMailAsync(
                new MailMessage(from: mail,
                                to: email,
                                subject,
                                htmlMessage)
                {
                    IsBodyHtml = true // Đảm bảo email có thể hiển thị định dạng HTML (có chứa link, in đậm...)
                }
            );
        }
    }
}
