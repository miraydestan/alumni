using Microsoft.AspNetCore.Mvc;

namespace Alumni.Controllers;

[ApiController]
[Route("/")]
public class AlumniController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var html = """
            <!DOCTYPE html>
            <html lang="tr">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Alumni Tracking System</title>
                <style>
                    body {
                        font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
                        display: flex;
                        justify-content: center;
                        align-items: center;
                        min-height: 100vh;
                        margin: 0;
                        background-color: #f8fafc;
                        color: #1e293b;
                    }
                    .card {
                        background: #ffffff;
                        padding: 2.5rem;
                        border-radius: 12px;
                        box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -2px rgba(0, 0, 0, 0.1);
                        max-width: 520px;
                        text-align: center;
                    }
                    h1 {
                        color: #0f172a;
                        margin-bottom: 0.75rem;
                        font-size: 1.75rem;
                    }
                    p {
                        color: #475569;
                        line-height: 1.6;
                        margin-bottom: 1.25rem;
                    }
                    .badge {
                        display: inline-block;
                        background: #fef3c7;
                        color: #92400e;
                        font-size: 0.875rem;
                        font-weight: 500;
                        padding: 0.4rem 0.85rem;
                        border-radius: 9999px;
                        border: 1px solid #fde68a;
                    }
                </style>
            </head>
            <body>
                <div class="card">
                    <h1>Alumni Tracking System</h1>
                    <p>Mezun bilgi ve takip platformuna hoş geldiniz. Bu sistem üniversite mezunlarının bilgilerini yönetmek ve takip etmek amacıyla geliştirilmektedir.</p>
                    <span class="badge">⚠️ Bu sayfa geçici bir ana giriş sayfasıdır (Temporary Main Page).</span>
                </div>
            </body>
            </html>
            """;
        return Content(html, "text/html; charset=utf-8");
    }

    [HttpGet("hello")]
    public IActionResult GetHello()
    {
        return Ok("Hello World!");
    }

    [HttpGet("hello/miray")]
    public IActionResult GetHelloMiray()
    {
        return Ok("Hello Miray");
    }

    [HttpGet("sum/{number1}/{number2}")]
    public IActionResult Sum(int number1, int number2)
    {
        return Ok(number1 + number2);
    }

    [HttpGet("about")]
    public IActionResult About()
    {
        var html = """
            <!DOCTYPE html>
            <html lang="tr">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>About - Alumni Tracking System</title>
                <style>
                    body {
                        font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
                        display: flex;
                        justify-content: center;
                        align-items: center;
                        min-height: 100vh;
                        margin: 0;
                        background-color: #f8fafc;
                        color: #1e293b;
                    }
                    .card {
                        background: #ffffff;
                        padding: 2.5rem;
                        border-radius: 12px;
                        box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -2px rgba(0, 0, 0, 0.1);
                        max-width: 520px;
                        text-align: center;
                    }
                    h1 {
                        color: #0f172a;
                        margin-bottom: 0.75rem;
                        font-size: 1.75rem;
                    }
                    p {
                        color: #475569;
                        line-height: 1.6;
                        margin-bottom: 1.25rem;
                    }
                    .badge {
                        display: inline-block;
                        background: #e0f2fe;
                        color: #0369a1;
                        font-size: 0.875rem;
                        font-weight: 500;
                        padding: 0.4rem 0.85rem;
                        border-radius: 9999px;
                        border: 1px solid #bae6fd;
                    }
                </style>
            </head>
            <body>
                <div class="card">
                    <h1>About Alumni Tracking System</h1>
                    <p>Bu sistem, üniversite mezunlarının kariyer yollarını, iletişim bilgilerini ve akademik geçmişlerini takip etmek ve mezunlar arasındaki ağı güçlendirmek amacıyla tasarlanmıştır.</p>
                    <span class="badge">Temporary About Page</span>
                </div>
            </body>
            </html>
            """;
        return Content(html, "text/html; charset=utf-8");
    }
}
