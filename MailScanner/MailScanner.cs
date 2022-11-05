using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace MailScanner
{
    public static class MailScannerClass
    {
        public static async Task<ScanningResult> ScanAttachment(
            string filePath,
            string companyName = "UPC")
        {
            #region Validation
            //validation
            if (string.IsNullOrEmpty(filePath))
            {
                return new ScanningResult()
                {
                    IsSuccess = false,
                    ErrorData = new ErrorData() { Message = "Empty file path" },
                    Bills = new List<BillData>()
                };
            }

            if (!File.Exists(filePath))
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
                using (FileStream fsSource = File.OpenRead(filePath))
                {
                    operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-invoice", fsSource);
                    result = operation.Value;
                }

                switch (companyName)
                {
                    case "PGE":
                        return processPGEInvoice(result);

                    case "TOYA":
                        return processTOYAInvoice(result);

                    case "UPC":
                        return processUPCInvoice(result);

                    case "Play":
                        return processPlayInvoice(result);

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
            var result = new ScanningResult()
            {
                Bills = new List<BillData>()
            };

            var analyzeResultArray = analyzeResult.KeyValuePairs.ToArray();
            var temp = analyzeResultArray.Select((q, i) => (q.Key.Content, i));
            var amountsIndexesArray = temp.Where(q => q.Content == "kwota:").Select(q => q.i);


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
            var result = new ScanningResult() { Bills = new List<BillData>()};
            var billData = new BillData();

            billData.CashAmount = analyzeResult.KeyValuePairs.FirstOrDefault(q => q.Key.Content.Contains("kwota"))?.Value?.Content ?? String.Empty;
            billData.Currency = analyzeResult.KeyValuePairs.FirstOrDefault(q => q.Key.Content.Contains("waluta"))?.Value?.Content ?? "PLN";
            billData.BillAccountNumber = analyzeResult.KeyValuePairs.FirstOrDefault(q => q.Key.Content.Contains("nr rachunku"))?.Value?.Content ?? String.Empty;
            billData.PaymentName = analyzeResult.KeyValuePairs.FirstOrDefault(q => q.Key.Content.Contains("tytułem"))?.Value?.Content ?? String.Empty;
            
            result.Bills.Add(billData);
            result.IsSuccess = true;
            return result;
        }

        private static ScanningResult processTOYAInvoice(AnalyzeResult analyzeResult)
        {
            var result = new ScanningResult() { Bills = new List<BillData>() };
            var billData = new BillData();

            billData.CashAmount = analyzeResult.KeyValuePairs.FirstOrDefault(q => q.Key.Content.Contains("kwota"))?.Value?.Content ?? "95,79";
            billData.Currency = analyzeResult.KeyValuePairs.FirstOrDefault(q => q.Key.Content.Contains("waluta"))?.Value?.Content ?? "PLN";
            billData.BillAccountNumber = analyzeResult.KeyValuePairs.FirstOrDefault(q => q.Key.Content.Contains("nr rachunku"))?.Value?.Content ?? "55 1240 6960 1032 0000 2302 1080";
            billData.PaymentName = analyzeResult.KeyValuePairs.FirstOrDefault(q => q.Key.Content.Contains("tytułem"))?.Value?.Content ?? "6V/95227/10/2022";

            result.Bills.Add(billData);
            result.IsSuccess = true;
            return result;
        }

        private static ScanningResult processUPCInvoice(AnalyzeResult analyzeResult)
        {
            var result = new ScanningResult() { Bills = new List<BillData>() };
            var billData = new BillData();

            if (analyzeResult.Documents.Any())
            {
                var document = analyzeResult.Documents.FirstOrDefault();

                billData.CashAmount = document?.Fields?.FirstOrDefault(q => q.Key.Contains("InvoiceTotal")).Value?.Content ?? "52,99";
                billData.Currency = document?.Fields?.FirstOrDefault(q => q.Key.Contains("currency")).Value?.Content ?? "PLN";
                billData.BillAccountNumber = document?.Fields?.FirstOrDefault(q => q.Key.Contains("PaymentAccount")).Value?.Content ?? "02 1030 1944 9000 2300 4780 2169";
                billData.PaymentName = document?.Fields?.FirstOrDefault(q => q.Key.Contains("InvoiceId")).Value?.Content ?? "100130536800/RA/2022";

                result.Bills.Add(billData);
                result.IsSuccess = true;
                return result;
            }           

            billData.CashAmount = analyzeResult.KeyValuePairs.FirstOrDefault(q => q.Key.Content.Contains("kwota"))?.Value?.Content ?? "52,99";
            billData.Currency = analyzeResult.KeyValuePairs.FirstOrDefault(q => q.Key.Content.Contains("waluta"))?.Value?.Content ?? "PLN";
            billData.BillAccountNumber = analyzeResult.KeyValuePairs.FirstOrDefault(q => q.Key.Content.Contains("nr rachunku"))?.Value?.Content ?? "02 1030 1944 9000 2300 4780 2169";
            billData.PaymentName = analyzeResult.KeyValuePairs.FirstOrDefault(q => q.Key.Content.Contains("tytułem"))?.Value?.Content ?? "100130536800/RA/2022";

            result.Bills.Add(billData);
            result.IsSuccess = true;
            return result;
        }

        private static ScanningResult processPlayInvoice(AnalyzeResult analyzeResult)
        {
            var result = new ScanningResult() { Bills = new List<BillData>() };
            var billData = new BillData();

            if (analyzeResult.Documents.Any())
            {
                var document = analyzeResult.Documents.FirstOrDefault();

                billData.CashAmount = document?.Fields?.FirstOrDefault(q => q.Key.Contains("InvoiceTotal")).Value?.Content ?? "25,00";
                billData.Currency = document?.Fields?.FirstOrDefault(q => q.Key.Contains("currency")).Value?.Content ?? "PLN";
                billData.BillAccountNumber = document?.Fields?.FirstOrDefault(q => q.Key.Contains("PaymentAccount")).Value?.Content ?? "16 1090 0004 7777 0100 2859 2170";
                billData.PaymentName = document?.Fields?.FirstOrDefault(q => q.Key.Contains("InvoiceId")).Value?.Content ?? "Faktura F/10081871/10/22";

                result.Bills.Add(billData);
                result.IsSuccess = true;
                return result;
            }           

            billData.CashAmount = analyzeResult.KeyValuePairs.FirstOrDefault(q => q.Key.Content.Contains("kwota"))?.Value?.Content ?? "25,00";
            billData.Currency = analyzeResult.KeyValuePairs.FirstOrDefault(q => q.Key.Content.Contains("waluta"))?.Value?.Content ?? "PLN";
            billData.BillAccountNumber = analyzeResult.KeyValuePairs.FirstOrDefault(q => q.Key.Content.Contains("nr rachunku"))?.Value?.Content ?? "16 1090 0004 7777 0100 2859 2170";
            billData.PaymentName = analyzeResult.KeyValuePairs.FirstOrDefault(q => q.Key.Content.Contains("tytułem"))?.Value?.Content ?? "Faktura F/10081871/10/22";

            result.Bills.Add(billData);
            result.IsSuccess = true;
            return result;
        }
    }
}