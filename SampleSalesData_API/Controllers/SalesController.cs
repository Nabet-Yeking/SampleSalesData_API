using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SampleSalesData_API.Controllers
{
    [Route("api/v1/sales-data")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<ActionResult> UploadSalesData([FromForm] IFormFile file)
        {
            try
            {
                if (file != null && file.ContentType == LiteralConstants.TEXT_CSV && file.Length != 0)
                {
                    Guid fileID = Guid.NewGuid();
                    var filePath = Path.Combine("Data", "TempSalesData", $"{fileID}.csv");
                    using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                    await file.CopyToAsync(stream);
                    stream.Close();

                    if (processSalesData(fileID, filePath))
                    {
                        return Ok(new { downloadURL = $"{RouteData}/download/{fileID}" });
                    }

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                return BadRequest(LiteralConstants.FILE_NOT_UPLOADED);
            }
            catch (Exception)
            {
                return BadRequest(LiteralConstants.FILE_NOT_UPLOADED);
            }
        }

        private bool processSalesData(Guid fileID, string filePath)
        {
            try
            {
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                StreamReader streamReader = new StreamReader(stream);
                Dictionary<string, string> salesData = new Dictionary<string, string>();
                string? line, department, numberOfsales;

                while ((line = streamReader.ReadLine()) != null)
                {
                    var row = line.Split(',');
                    department = row[0];
                    if (salesData.TryGetValue(department, out numberOfsales))
                    {
                        var No_sales = salesData[department];
                        salesData[department] = (int.Parse(No_sales) + int.Parse(row[2])).ToString();
                    }
                    else
                    {
                        salesData.Add(department, row[2]);
                    }
                }

                streamReader.Close();
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                return storeExtractedSalesData(fileID, salesData.ToArray());
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool storeExtractedSalesData(Guid fileID, KeyValuePair<string, string>[] extractedSalesData) 
        {
            try
            {
                var filePath = Path.Combine("Data", "SalesData", $"{fileID}.csv");
                if (filePath == null) return false;

                var fstream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                StreamWriter streamWriter = new StreamWriter(fstream);

                for (int i = 0; i < extractedSalesData.Length; i++)
                {
                    streamWriter.WriteLine($"{extractedSalesData[i].Key},{extractedSalesData[i].Value}");
                }
                streamWriter.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [HttpGet("download/{Id}")]
        public async Task<ActionResult> DownloadSalesData(string Id)
        {
            try
            {
                var fileName = $"{Id}.csv";
                var filePath = Path.Combine("Data", "SalesData", fileName);
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return File(fileBytes, LiteralConstants.TEXT_CSV, fileName);
            } catch (Exception) {
                return BadRequest(LiteralConstants.FILE_NOT_FOUND);
            }
        }
    }
}
