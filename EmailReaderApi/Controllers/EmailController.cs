using System.Text;
using EmailReaderApi.Helpers;
using EmailReaderApi.Models;
using EmailReaderApi.Services;
using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Auth.OAuth2;

using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;

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
        try
        {
            var emails = await _emailService.GetUnreadEmails();
            return Ok(emails);
        }
        catch (Exception ex)
        {
            return Ok(ex.Message + " " + ex.StackTrace);
        }
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