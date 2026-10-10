using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Vizus.Application.Common.Interfaces;
using Vizus.Domain.Entities;

namespace Vizus.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendAppointmentConfirmationRequestAsync(string toEmail, string doctorName, DateTime startTime, string confirmationToken, CancellationToken ct)
    {
        var baseUrl = _configuration["Frontend:BaseUrl"];
        var confirmUrl = $"{baseUrl}/appointments/confirm?token={confirmationToken}";
        var logoUrl = $"{baseUrl}/assets/logo_bg.jpg"; // Пътят до логото ти в публикувания сайт

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Медицински център Визус", _configuration["Email:Username"]));
        message.To.Add(new MailboxAddress(toEmail, toEmail));
        message.Subject = "Потвърди часа си — Визус";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"
                <div style='background-color: #F8FAFC; padding: 40px 20px; font-family: ""Helvetica Neue"", Helvetica, Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; background: #ffffff; border-radius: 16px; overflow: hidden; box-shadow: 0 4px 24px rgba(0,0,0,0.05); border: 1px solid #e2e8f0;'>
                        
                        <!-- Header с Лого -->
                        <div style='padding: 30px; text-align: center; border-bottom: 1px solid #f1f5f9;'>
                            <img src='{logoUrl}' alt='Vizus' style='height: 45px; width: auto;' />
                        </div>

                        <!-- Съдържание -->
                        <div style='padding: 40px 40px; text-align: center;'>
                            <h2 style='color: #0F172A; margin-top: 0; font-size: 24px;'>Остана само да потвърдиш часа</h2>
                            <p style='color: #475569; font-size: 16px; line-height: 1.6;'>Заявил си преглед при <strong style='color: #0F172A;'>{doctorName}</strong>.</p>
                            
                            <div style='background: #F8FAFC; border: 1px solid #e2e8f0; border-radius: 12px; padding: 20px; margin: 30px 0; text-align: center;'>
                                <p style='margin: 0; color: #475569; font-size: 16px;'><strong>Дата:</strong> <span style='color: #235B9C;'>{startTime:dd.MM.yyyy}</span></p>
                                <p style='margin: 10px 0 0; color: #475569; font-size: 16px;'><strong>Час:</strong> <span style='color: #235B9C;'>{startTime:HH:mm}</span></p>
                            </div>

                            <a href='{confirmUrl}'
                               style='display: inline-block; background: #235B9C; color: #ffffff; text-decoration: none; padding: 14px 32px; border-radius: 999px; font-size: 16px; font-weight: bold;'>
                                Потвърди часа
                            </a>
                            
                            <p style='color: #94a3b8; font-size: 13px; margin-top: 30px;'>
                                Ако не си заявявал този час, просто игнорирай имейла.
                            </p>
                        </div>

                        <!-- Footer -->
                        <div style='background-color: #0F172A; padding: 24px; text-align: center; color: #94a3b8; font-size: 13px; line-height: 1.5;'>
                            &copy; {DateTime.Now.Year} Медицински център Визус.<br/>
                            ул. Примерна 12, София | +359 88 123 4567
                        </div>

                    </div>
                </div>"
        };

        message.Body = bodyBuilder.ToMessageBody();

        await SendEmailAsync(message, ct);
    }

    public async Task SendOrderConfirmationAsync(Order order, CancellationToken ct)
    {
        var baseUrl = _configuration["Frontend:BaseUrl"];
        var logoUrl = $"{baseUrl}/assets/logo_bg.jpg";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Оптика Визус", _configuration["Email:Username"]));
        message.To.Add(new MailboxAddress(order.RecipientName, order.RecipientEmail));
        message.Subject = $"Потвърждение за поръчка №{order.Id} — Визус";

        var itemsHtml = string.Join("", order.OrderItems.Select(i => $@"
        <tr>
            <td style='padding: 12px 0; color: #475569; border-bottom: 1px solid #f1f5f9;'>{i.ProductName} <span style='color: #94a3b8;'>× {i.Quantity}</span></td>
            <td style='padding: 12px 0; text-align: right; color: #0F172A; font-weight: bold; border-bottom: 1px solid #f1f5f9;'>{(i.UnitPrice * i.Quantity):F2} €</td>
        </tr>"));

        var deliveryText = order.DeliveryType switch
        {
            "Pickup" => "Вземане от клиниката",
            "EcontOffice" => $"Еконт офис: {order.EcontOfficeName}",
            "EcontAddress" => $"Еконт до адрес: {order.DeliveryAddress}",
            _ => order.DeliveryType
        };

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"
            <div style='background-color: #F8FAFC; padding: 40px 20px; font-family: ""Helvetica Neue"", Helvetica, Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; background: #ffffff; border-radius: 16px; overflow: hidden; box-shadow: 0 4px 24px rgba(0,0,0,0.05); border: 1px solid #e2e8f0;'>
                    
                    <!-- Header -->
                    <div style='padding: 30px; text-align: center; border-bottom: 1px solid #f1f5f9;'>
                        <img src='{logoUrl}' alt='Vizus' style='height: 45px; width: auto;' />
                    </div>

                    <!-- Съдържание -->
                    <div style='padding: 40px 40px;'>
                        <h2 style='color: #0F172A; margin-top: 0; font-size: 22px; text-align: center;'>Благодарим за поръчката!</h2>
                        <p style='color: #475569; font-size: 15px; line-height: 1.6; text-align: center;'>Здравей, <strong style='color: #0F172A;'>{order.RecipientName}</strong>! Плащането ти е получено успешно.</p>

                        <div style='background: #F8FAFC; border: 1px solid #e2e8f0; border-radius: 12px; padding: 24px; margin: 30px 0;'>
                            <p style='margin: 0 0 16px; font-weight: bold; color: #0F172A; font-size: 18px;'>Поръчка №{order.Id}</p>
                            <table style='width: 100%; border-collapse: collapse; font-size: 15px;'>
                                {itemsHtml}
                                <tr>
                                    <td style='padding: 16px 0 0; color: #0F172A;'><strong>Общо</strong></td>
                                    <td style='padding: 16px 0 0; text-align: right; color: #235B9C; font-size: 18px;'><strong>{order.TotalAmount:F2} €</strong></td>
                                </tr>
                            </table>
                        </div>

                        <div style='background: #ffffff; border: 1px solid #e2e8f0; border-radius: 12px; padding: 20px; font-size: 14px; color: #475569;'>
                            <p style='margin: 0 0 8px;'><strong style='color: #0F172A;'>Доставка:</strong> {deliveryText}</p>
                            <p style='margin: 0;'><strong style='color: #0F172A;'>Телефон за връзка:</strong> {order.RecipientPhone}</p>
                        </div>

                        <p style='color: #94a3b8; font-size: 14px; margin-top: 30px; text-align: center;'>
                            Ще се свържем с теб при готовност на поръчката.
                        </p>
                    </div>

                    <!-- Footer -->
                    <div style='background-color: #0F172A; padding: 24px; text-align: center; color: #94a3b8; font-size: 13px; line-height: 1.5;'>
                        &copy; {DateTime.Now.Year} Оптика Визус.<br/>
                        ул. Примерна 12, София | +359 88 123 4567
                    </div>

                </div>
            </div>"
        };

        message.Body = bodyBuilder.ToMessageBody();

        await SendEmailAsync(message, ct);
    }
    public async Task SendAppointmentCancelledAsync(string toEmail, string doctorName, DateTime startTime, CancellationToken ct)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Vizus", _configuration["Email:Username"]));
        message.To.Add(new MailboxAddress(toEmail, toEmail));
        message.Subject = "Часът ти е отказан — Vizus";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"
            <div style='font-family: sans-serif; max-width: 480px; margin: 0 auto;'>
                <h2 style='color: #0F2A2E;'>Часът ти е отказан</h2>
                <p>Часът ти при <strong>{doctorName}</strong> беше отказан.</p>
                <div style='background: #F6F4EE; border-radius: 12px; padding: 16px; margin: 20px 0;'>
                    <p style='margin: 0;'><strong>Дата:</strong> {startTime:dd.MM.yyyy}</p>
                    <p style='margin: 8px 0 0;'><strong>Час:</strong> {startTime:HH:mm}</p>
                </div>
                <p>Можеш да запишеш нов час по всяко време от сайта ни.</p>
            </div>"
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_configuration["Email:SmtpHost"], int.Parse(_configuration["Email:SmtpPort"]!), SecureSocketOptions.StartTls, ct);
        await client.AuthenticateAsync(_configuration["Email:Username"], _configuration["Email:Password"], ct);
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);
    }
    // Изнесохме повтарящата се логика за пращане в отделен private метод
    private async Task SendEmailAsync(MimeMessage message, CancellationToken ct)
    {
        using var client = new SmtpClient();
        await client.ConnectAsync(_configuration["Email:SmtpHost"], int.Parse(_configuration["Email:SmtpPort"]!), SecureSocketOptions.StartTls, ct);
        await client.AuthenticateAsync(_configuration["Email:Username"], _configuration["Email:Password"], ct);
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);
    }

}