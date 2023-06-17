using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WebAppBV.Models;

namespace WebAppBV.Services
{
    public interface IExcelService
    {
        Task<Stream> GenerateExcel(Stream stream, string fileName);
        IEnumerable<Transaction> ReadLastMonthExcel();
    }
}