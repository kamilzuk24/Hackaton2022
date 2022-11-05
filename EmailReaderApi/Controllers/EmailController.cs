using EmailReaderApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmailReaderApi.Controllers;

[ApiController]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [Authorize]
    [HttpGet("signin-oidc")]
    public IActionResult Signin()
    {
        return Ok();
    }

    [HttpGet("emails")]
    public async Task<IActionResult> Get()
    {
        var emails = await _emailService.GetUnreadEmails();
        return Ok(emails);
    }

    [HttpGet("attachment/{messageId}/{fileId}/{name}")]
    public async Task<IActionResult> GetAttachment(
        [FromRoute] string messageId,
        [FromRoute] string fileId,
        [FromRoute] string name
        )
    {
        var attachment = await _emailService.GetAttachment(messageId, fileId, name);

        return new FileContentResult(attachment.Data, attachment.MimeType)
        {
            FileDownloadName = attachment.Name
        };
    }
}