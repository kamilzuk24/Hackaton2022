namespace Models;

public class EmailAttachmentViewModel
{
    public string Name { get; set; }
    public string Extension { get; set; }
    public string MimeType { get; set; }
    public byte[] Data { get; set; }
}