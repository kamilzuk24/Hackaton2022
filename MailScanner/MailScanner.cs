using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MailScanner
{
    public static class MailScannerClass
    {
        public static async Task<ScanningResult> ScanAttachment(
            string pathToFile = @"D:\Hackaton2022\MailScanner\Dokument_201987012.pdf",
            string companyName = "PGE")
        {

            #region Validation
            //validation
            if (string.IsNullOrEmpty(pathToFile))
            {
                return new ScanningResult()
                {
                    IsSuccess = false,
                    ErrorData = new ErrorData() { Message = "Empty file path" },
                    Bills = new List<BillData>()
                };
            }

            if (!File.Exists(pathToFile))
            {
                return new ScanningResult()
                {
                    IsSuccess = false,
                    ErrorData = new ErrorData() { Message = "FILE DOES NOT EXIST!" },
                    Bills = new List<BillData>()
                };
            }
            #endregion 

            string endpoint = "https://westeurope.api.cognitive.microsoft.com/";
            string key = "b5d3f356e6c942c3aa473462f4e483d9";
            AzureKeyCredential credential = new AzureKeyCredential(key);
            DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(endpoint), credential);

            AnalyzeDocumentOperation operation;
            AnalyzeResult result;
            try
            {
                using (FileStream fsSource = File.OpenRead(pathToFile))
                {
                    operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-invoice", fsSource);
                    result = operation.Value;
                }

                switch (companyName)
                {
                    case "PGE":
                        return processPGEInvoice(result);

                    default:
                        return processDefaultInvoice(result);
                }


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static ScanningResult processPGEInvoice(AnalyzeResult analyzeResult)
        {
            var result = new ScanningResult() { 
                Bills = new List<BillData>() 
            };

            var analyzeResultArray = analyzeResult.KeyValuePairs.ToArray();
            var temp = analyzeResultArray.Select((q, i) => (q.Key.Content ,i));
            var amountsIndexesArray = temp.Where(q => q.Content == "kwota:").Select(q=>q.i);


            var arrayIndexesArrayLength = amountsIndexesArray.Count();

            for (int currentIndex = 0; currentIndex < arrayIndexesArrayLength; currentIndex++)
            {
                var billData = new BillData();
                bool isLastElement = currentIndex == arrayIndexesArrayLength - 1;

                var billElements = analyzeResultArray.Skip(amountsIndexesArray.ElementAtOrDefault(currentIndex))
                    .Take(
                        (!isLastElement ? amountsIndexesArray.ElementAtOrDefault(currentIndex + 1) : analyzeResultArray.Length)
                        - amountsIndexesArray.ElementAtOrDefault(currentIndex));

                billData.CashAmount = billElements.FirstOrDefault(q => q.Key.Content == "kwota:")?.Value?.Content ?? String.Empty;
                billData.Currency = billElements.FirstOrDefault(q => q.Key.Content == "waluta")?.Value?.Content ?? "PLN";
                billData.BillAccountNumber = billElements.FirstOrDefault(q => q.Key.Content.Contains("nr rachunku"))?.Value?.Content ?? String.Empty;
                billData.PaymentName = billElements.FirstOrDefault(q => q.Key.Content.Contains("tytułem"))?.Value?.Content ?? String.Empty;

                result.Bills.Add(billData);
            }

            result.IsSuccess = true;
            return result;
        }

        private static ScanningResult processDefaultInvoice(AnalyzeResult analyzeResult)
        {
            var result = new ScanningResult();
            var billData = new BillData();

            foreach (DocumentKeyValuePair kvp in analyzeResult.KeyValuePairs)
            {
                billData.CashAmount = analyzeResult.KeyValuePairs.FirstOrDefault(q => q.Key.Content.Contains( "kwota"))?.Value?.Content ?? String.Empty;
                billData.Currency = analyzeResult.KeyValuePairs.FirstOrDefault(q => q.Key.Content.Contains("waluta"))?.Value?.Content ?? "PLN";
                billData.BillAccountNumber = analyzeResult.KeyValuePairs.FirstOrDefault(q => q.Key.Content.Contains("nr rachunku"))?.Value?.Content ?? String.Empty;
                billData.PaymentName = analyzeResult.KeyValuePairs.FirstOrDefault(q => q.Key.Content.Contains("tytułem"))?.Value?.Content ?? String.Empty;
            }
            return result;
        }
    }
}





//foreach (DocumentPage page in result.Pages)
//{
//    Console.WriteLine($"Document Page {page.PageNumber} has {page.Lines.Count} line(s), {page.Words.Count} word(s),");
//    Console.WriteLine($"and {page.SelectionMarks.Count} selection mark(s).");

//    for (int i = 0; i < page.Lines.Count; i++)
//    {
//        DocumentLine line = page.Lines[i];
//        Console.WriteLine($"  Line {i} has content: '{line.Content}'.");

//        Console.WriteLine($"    Its bounding box is:");
//        Console.WriteLine($"      Upper left => X: {line.BoundingPolygon[0].X}, Y= {line.BoundingPolygon[0].Y}");
//        Console.WriteLine($"      Upper right => X: {line.BoundingPolygon[1].X}, Y= {line.BoundingPolygon[1].Y}");
//        Console.WriteLine($"      Lower right => X: {line.BoundingPolygon[2].X}, Y= {line.BoundingPolygon[2].Y}");
//        Console.WriteLine($"      Lower left => X: {line.BoundingPolygon[3].X}, Y= {line.BoundingPolygon[3].Y}");
//    }

//    for (int i = 0; i < page.SelectionMarks.Count; i++)
//    {
//        DocumentSelectionMark selectionMark = page.SelectionMarks[i];

//        Console.WriteLine($"  Selection Mark {i} is {selectionMark.State}.");
//        Console.WriteLine($"    Its bounding box is:");
//        Console.WriteLine($"      Upper left => X: {selectionMark.BoundingPolygon[0].X}, Y= {selectionMark.BoundingPolygon[0].Y}");
//        Console.WriteLine($"      Upper right => X: {selectionMark.BoundingPolygon[1].X}, Y= {selectionMark.BoundingPolygon[1].Y}");
//        Console.WriteLine($"      Lower right => X: {selectionMark.BoundingPolygon[2].X}, Y= {selectionMark.BoundingPolygon[2].Y}");
//        Console.WriteLine($"      Lower left => X: {selectionMark.BoundingPolygon[3].X}, Y= {selectionMark.BoundingPolygon[3].Y}");
//    }
//}

//foreach (DocumentStyle style in result.Styles)
//{
//    // Check the style and style confidence to see if text is handwritten.
//    // Note that value '0.8' is used as an example.

//    bool isHandwritten = style.IsHandwritten.HasValue && style.IsHandwritten == true;

//    if (isHandwritten && style.Confidence > 0.8)
//    {
//        Console.WriteLine($"Handwritten content found:");

//        foreach (DocumentSpan span in style.Spans)
//        {
//            Console.WriteLine($"  Content: {result.Content.Substring(span.Index, span.Length)}");
//        }
//    }
//}

//Console.WriteLine("The following tables were extracted:");

//for (int i = 0; i < result.Tables.Count; i++)
//{
//    DocumentTable table = result.Tables[i];
//    Console.WriteLine($"  Table {i} has {table.RowCount} rows and {table.ColumnCount} columns.");

//    foreach (DocumentTableCell cell in table.Cells)
//    {
//        Console.WriteLine($"    Cell ({cell.RowIndex}, {cell.ColumnIndex}) has kind '{cell.Kind}' and content: '{cell.Content}'.");
//    }
//}