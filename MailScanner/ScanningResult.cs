using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailScanner;

public class ScanningResult
{
    public List<BillData> Bills { get; set; }
    public decimal Total { get; set; }

    public bool IsSuccess { get; set; }

    public ErrorData ErrorData { get; set; }
}

public class ErrorData
{
    public string Message { get; set; }
}

