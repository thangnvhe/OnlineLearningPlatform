using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using OnlineLearningPlatform.Domain.Abstract;
using OnlineLearningPlatform.Domain.Setting;
using System.Reflection;
using System.Text.Json; // <--- Đã thêm dòng này

namespace OnlineLearningPlatform.Infrastruture.Services
{
    public class EmailService : IEmailService
    {
        private EmailSettings _emailSettings;

        public EmailService(IConfiguration configuration)
        {
            _emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>()
                              ?? throw new ArgumentNullException("EmailSettings not found in configuration");
        }
        public async Task SendEmailAsync(string to, string subject, string body, CancellationToken ct = default)
        {
            await SendEmailInternalAsync(to, subject, body, null, ct);
        }

        public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, IEnumerable<AttachmentDto> attachments, CancellationToken ct = default)
        {
            await SendEmailInternalAsync(to, subject, body, attachments, ct);
        }

        // Hàm dùng chung để tránh lặp code (Private Helper)
        private async Task SendEmailInternalAsync(string to, string subject, string body, IEnumerable<AttachmentDto>? attachments, CancellationToken ct)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = body // Hỗ trợ gửi định dạng HTML cho đẹp
            };

            if (attachments != null)
            {
                foreach (var file in attachments)
                {
                    // Thêm file đính kèm từ mảng byte
                    builder.Attachments.Add(file.FileName, file.Content);
                }
            }

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                // Kết nối tới server SMTP (Gmail, Outlook, etc.)
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, SecureSocketOptions.StartTls, ct);

                // Đăng nhập bằng tài khoản App Password
                await client.AuthenticateAsync(_emailSettings.UserName, _emailSettings.Password, ct);

                await client.SendAsync(message, ct);
            }
            finally
            {
                await client.DisconnectAsync(true, ct);
            }
        }

        public Task SendTemplateEmailAsync(string to, string subject, string templateName, object model, CancellationToken ct = default)
        {
            // Resolve template file path (EmailTemplates folder next to the app base directory)
            var baseDir = AppContext.BaseDirectory ?? Directory.GetCurrentDirectory();
            var templatesDir = Path.Combine(baseDir, "EmailTemplates");
            var fileName = templateName.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase) ? templateName : templateName + ".cshtml";
            var templatePath = Path.Combine(templatesDir, fileName);

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Email template not found: {templatePath}");

            var template = File.ReadAllText(templatePath);
            var body = RenderTemplate(template, model);

            return SendEmailAsync(to, subject, body, ct);
        }

        // ĐÃ CẬP NHẬT: Hỗ trợ cả object in-memory và dữ liệu JSON từ RabbitMQ
        private string RenderTemplate(string template, object model)
        {
            if (model == null) return template;

            // 1. KIỂM TRA BAO QUÁT NHẤT: Bắt mọi thể loại Dictionary (cực kỳ an toàn)
            if (model is System.Collections.IDictionary dict)
            {
                foreach (System.Collections.DictionaryEntry kv in dict)
                {
                    var key = kv.Key?.ToString();
                    if (!string.IsNullOrEmpty(key))
                    {
                        template = template.Replace("{{" + key + "}}", kv.Value?.ToString() ?? string.Empty);
                    }
                }
                return template;
            }

            // 2. Xử lý JsonElement (Trường hợp dữ liệu bị ép kiểu ngầm định khi qua mạng)
            if (model is System.Text.Json.JsonElement jsonElement && jsonElement.ValueKind == System.Text.Json.JsonValueKind.Object)
            {
                foreach (var prop in jsonElement.EnumerateObject())
                {
                    var val = prop.Value.ValueKind == System.Text.Json.JsonValueKind.String ? prop.Value.GetString() : prop.Value.GetRawText();
                    template = template.Replace("{{" + prop.Name + "}}", val ?? string.Empty);
                }
                return template;
            }

            // 3. Fallback: Reflection an toàn (bỏ qua Indexer)
            var props = model.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                             .Where(p => p.GetIndexParameters().Length == 0);

            foreach (var p in props)
            {
                var val = p.GetValue(model)?.ToString() ?? string.Empty;
                template = template.Replace("{{" + p.Name + "}}", val);
            }

            return template;
        }
    }
}