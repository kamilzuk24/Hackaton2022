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
    [GoogleScopedAuthorize(GmailService.ScopeConstants.MailGoogleCom)]
    public async Task<IActionResult> Get([FromServices] IGoogleAuthProvider auth)
    {
        var cred = await auth.GetCredentialAsync();
        var emails = await _emailService.GetUnreadEmails(cred);
        return Ok(emails);
    }
    
    [HttpGet("attachment/{messageId}/{fileId}/{name}")]
    [GoogleScopedAuthorize(GmailService.ScopeConstants.MailGoogleCom)]
    public async Task<IActionResult> GetAttachment(
        [FromServices] IGoogleAuthProvider auth,
        [FromRoute] string messageId,
        [FromRoute] string fileId,
        [FromRoute] string name
        )
    {
        var cred = await auth.GetCredentialAsync();
        var attachment = await _emailService.GetAttachment(cred, messageId, fileId);
        
        return new FileContentResult(attachment, "application/pdf")
        {
            FileDownloadName = name
        };

    }
}