using System.Text;
using Google.Apis.Gmail.v1.Data;

namespace EmailReaderApi.Helpers;

public class Decoders
{
    public static string GetNestedBodyParts(IList<MessagePart> part, string curr)
    {
        string str = curr;
        if (part == null)
        {
            return str;
        }
        else
        {
            foreach (var parts in part)
            {
                if (parts.Parts == null)
                {
                    if (parts.Body == null || parts.Body.Data == null) continue;
                    var ts = DecodeURLEncodedBase64EncodedString(parts.Body.Data);
                    str += ts;
                }
                else
                {
                    return GetNestedBodyParts(parts.Parts, str);
                }
            }

            return str;
        }
    }


    public static string DecodeURLEncodedBase64EncodedString(string sInput)
    {
        string sBase46codedBody = sInput.Replace("-", "+").Replace("_", "/").Replace("=", String.Empty);  //get rid of URL encoding, and pull any current padding off.
        string sPaddedBase46codedBody = sBase46codedBody.PadRight(sBase46codedBody.Length + (4 - sBase46codedBody.Length % 4) % 4, '=');  //re-pad the string so it is correct length.
        byte[] data = Convert.FromBase64String(sPaddedBase46codedBody);
        return Encoding.UTF8.GetString(data);
    }
}