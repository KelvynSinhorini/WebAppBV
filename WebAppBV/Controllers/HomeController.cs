using HtmlAgilityPack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebAppBV.Models;
using WebAppBV.ViewModels;

namespace WebAppBV.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var importFileViewModel = new ImportFileViewModel();

            return View(importFileViewModel);
        }

        public IActionResult ExportFile(ImportFileViewModel importFileViewModel)
        {
            if (importFileViewModel.FormFile == null || importFileViewModel.FormFile?.Length == 0)
            {
                ModelState.AddModelError("FormFile", "Arquivo inválido.");
                return View("index", importFileViewModel);
            }

            SaveImportedFileInServer(importFileViewModel.FormFile);

            string htmlText = GetHtmlTextFromFromFile(importFileViewModel.FormFile);
            CleanHtmlText(htmlText);

            var transactions = ReadTransactionsInHtmlText(htmlText);

            // Export to download
            string webRootPath = _webHostEnvironment.WebRootPath;

            //string path = Path.Combine(webRootPath, "App_Data/ImportedFiles");
            string templatePath = Path.Combine(webRootPath, "App_Data/Templates");

            if (!(Directory.Exists(templatePath)))
                Directory.CreateDirectory(templatePath);

            templatePath = Path.Combine(templatePath, $"TransactionExportTemplate.xlsx");
            var result = ExportTransactionsDetails(transactions, templatePath);
            // Export to download


            // return RedirectToAction("Index");
            return File(result, "application/vnd.ms-excel", "Transações.xlsx");
        }

        public byte[] ExportTransactionsDetails(List<Transaction> transactions, string templatePath)
        {
            using (var templateStream = new FileStream(templatePath, FileMode.Open))
            {
                XSSFWorkbook xssfWorkbook = new XSSFWorkbook(templateStream);
                using (var exportedStream = new MemoryStream())
                {
                    var transactionSheet = xssfWorkbook.GetSheet("TRANSAÇÕES");

                    //TODO Retirar depois
                    // -----------------------------------------------------------------------------------------------------------------
                    transactions.Add(new Transaction(Guid.NewGuid(), "Ponto Frio", "Campo Bom", new decimal(56.66), new DateTime(2021, 8, 29), 8, 12));
                    transactions.Add(new Transaction(Guid.NewGuid(), "Kabum", "LIMEIRA", new decimal(93.61), new DateTime(2022, 4, 9), 1, 12));
                    // -----------------------------------------------------------------------------------------------------------------

                    int rowCount = 1;
                    int indexFirstRow = 1;

                    foreach (var transaction in transactions?.OrderBy(t => t.Date))
                    {
                        var row = transactionSheet.CreateRow(rowCount);

                        ICellStyle cellStyle = xssfWorkbook.CreateCellStyle();
                        cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                        cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                        cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                        cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;

                        cellStyle.TopBorderColor = IndexedColors.Black.Index;
                        cellStyle.RightBorderColor = IndexedColors.Black.Index;
                        cellStyle.LeftBorderColor = IndexedColors.Black.Index;
                        cellStyle.BottomBorderColor = IndexedColors.Black.Index;

                        var dateCell = row.CreateCell(0);
                        dateCell.CellStyle = cellStyle;
                        dateCell.SetCellValue(transaction.Date.ToString("dd/MM/yy"));

                        var descriptionCell = row.CreateCell(1);

                        string description = transaction.Description;

                        if (transaction.NumberOfParcel != null && transaction.TotalParcel != null)
                        {
                            description += $" {transaction.NumberOfParcel}/{transaction.TotalParcel}";
                        }

                        descriptionCell.CellStyle = cellStyle;
                        descriptionCell.SetCellValue(description);

                        var valueCell = row.CreateCell(2);
                        valueCell.CellStyle = cellStyle;
                        valueCell.SetCellValue(decimal.ToDouble(transaction.Value));

                        if (rowCount == ((transactions.Count - 1) + indexFirstRow))
                        {
                            row = transactionSheet.CreateRow(rowCount);

                            var totalCell = row.CreateCell(2);
                            string formula = $"SUBTOTAL(9, C{indexFirstRow + 1}:C{transactions.Count})";
                            totalCell.SetCellType(CellType.Formula);
                            totalCell.SetCellFormula(formula);
                        }

                        rowCount++;
                    }

                    transactionSheet.AutoSizeColumn(0);
                    transactionSheet.AutoSizeColumn(1);
                    transactionSheet.AutoSizeColumn(2);

                    xssfWorkbook.Write(exportedStream);

                    return exportedStream.ToArray();
                }
            }
        }

        private void CleanHtmlText(string htmlText)
        {
            if (htmlText.Contains("<!---->"))
                htmlText = htmlText.Replace("<!---->", "");
        }

        private string GetHtmlTextFromFromFile(IFormFile formFile)
        {
            using (var reader = new StreamReader(formFile.OpenReadStream()))
            {
                return reader.ReadToEnd();
            }
        }

        private void SaveImportedFileInServer(IFormFile formFile)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;

            string path = Path.Combine(webRootPath, "App_Data/ImportedFiles");

            if (!(Directory.Exists(path)))
                Directory.CreateDirectory(path);

            path = Path.Combine(path, $"{new Random().Next(1000, 9999)}_{formFile.FileName}");

            using (var stream = new FileStream(path, FileMode.Create))
            {
                formFile.CopyToAsync(stream);
            }
        }

        private static List<Transaction> ReadTransactionsInHtmlText(string htmlText)
        {
            var transactions = new List<Transaction>();

            HtmlDocument htmlDocument = new HtmlDocument();

            if (!string.IsNullOrEmpty(htmlText))
            {
                htmlDocument.LoadHtml(htmlText);

                var elementsList = htmlDocument.DocumentNode.SelectNodes("ul").FirstOrDefault();

                if (elementsList != null)
                {
                    // clear list if child node name is Text or Comment
                    var elementsText = elementsList.ChildNodes.Where(x => x.Name != "li").ToList();

                    foreach (var element in elementsText)
                    {
                        elementsList.RemoveChild(element);
                    }

                    var listUl = elementsList.ChildNodes;

                    foreach (var elementUl in listUl)
                    {
                        var transaction = GetTransactionInHtmlList(elementUl);

                        transactions.Add(transaction);
                    }

                    if (transactions.Any(t => t.Description.Contains("PAGAMENTO EFETUADO")))
                    {
                        var transaction = transactions.FirstOrDefault(t => t.Description.Contains("PAGAMENTO EFETUADO"));

                        if (transaction != null)
                            transactions.Remove(transaction);
                    }
                }
            }

            return transactions;
        }

        private static Transaction GetTransactionInHtmlList(HtmlNode elementUl)
        {
            var transaction = new Transaction();

            var mainDiv = elementUl.ChildNodes.FirstOrDefault(c => c.Name != "#comment");

            if (mainDiv != null)
            {
                for (int i = 0; i < mainDiv.ChildNodes.Count; i++)
                {
                    if (i == 0)
                    {
                        var firstDiv = mainDiv.ChildNodes[i];

                        for (int j = 0; j < firstDiv.ChildNodes.Count; j++)
                        {
                            if (j == 0)
                            {
                                var divDescription = firstDiv.ChildNodes[j];

                                var descriptionAndParcel = divDescription.InnerText;
                                var description = "";
                                int numberOfParcel = 0;
                                int totalParcel = 0;

                                if (!string.IsNullOrEmpty(descriptionAndParcel))
                                {
                                    if (descriptionAndParcel.Contains('('))
                                    {
                                        var indexOfparentheses = descriptionAndParcel.IndexOf('(');

                                        description = descriptionAndParcel.Substring(0, indexOfparentheses - 1);

                                        var numbers = descriptionAndParcel.Substring(indexOfparentheses + 1, (descriptionAndParcel.Length - (indexOfparentheses + 1)) - 1);

                                        int.TryParse(numbers.Split('/')?[0], out numberOfParcel);

                                        int.TryParse(numbers.Split('/')?[1], out totalParcel);

                                        transaction.NumberOfParcel = numberOfParcel;
                                        transaction.TotalParcel = totalParcel;
                                    }
                                    else
                                    {
                                        description = descriptionAndParcel;

                                        transaction.OnlyThisMonth = true;
                                    }
                                }

                                transaction.Description = description;
                            }
                            else
                            if (j == 3)
                            {
                                var divDate = firstDiv.ChildNodes[j];

                                var dateAndLocal = divDate.InnerText;
                                string date = "";
                                string local = "";

                                if (dateAndLocal?.Contains('-') == true)
                                {
                                    date = dateAndLocal.Split('-')?[0]?.Trim() ?? "";
                                    local = dateAndLocal.Split('-')?[1]?.Trim() ?? "";
                                }

                                transaction.Date = DateTime.Parse(date);
                                transaction.Local = local;
                            }
                        }
                    }
                    else
                    if (i == 1)
                    {
                        var secondDiv = mainDiv.ChildNodes[i];

                        var value = secondDiv.InnerText;

                        if (value?.Contains("R$") == true)
                        {
                            value = value.Replace("R$", "");
                        }

                        if (!string.IsNullOrEmpty(value))
                            transaction.Value = decimal.Parse(value);
                    }
                }
            }

            return transaction;
        }

        public byte[] ExportTransactionsDetails2(List<Transaction> transactions, string templatePath)
        {
            using (var templateStream = new FileStream(templatePath, FileMode.Open))
            {
                XSSFWorkbook xssfWorkbook = new XSSFWorkbook(templateStream);
                using (var exportedStream = new MemoryStream())
                {
                    xssfWorkbook.Write(exportedStream);

                    return exportedStream.ToArray();
                }
            }
        }


        //método para enviar os arquivos usando a interface IFormFile
        public async Task<IActionResult> EnviarArquivo(List<IFormFile> arquivos)
        {
            long tamanhoArquivos = arquivos.Sum(f => f.Length);
            // caminho completo do arquivo na localização temporária
            var caminhoArquivo = Path.GetTempFileName();

            // processa os arquivo enviados
            //percorre a lista de arquivos selecionados
            foreach (var arquivo in arquivos)
            {
                //verifica se existem arquivos 
                if (arquivo == null || arquivo.Length == 0)
                {
                    //retorna a viewdata com erro
                    ViewData["Erro"] = "Error: Arquivo(s) não selecionado(s)";
                    return View(ViewData);
                }
                // < define a pasta onde vamos salvar os arquivos >
                string pasta = "Arquivos_Usuario";
                // Define um nome para o arquivo enviado incluindo o sufixo obtido de milesegundos
                string nomeArquivo = "Usuario_arquivo_" + DateTime.Now.Millisecond.ToString();
                //verifica qual o tipo de arquivo : jpg, gif, png, pdf ou tmp
                if (arquivo.FileName.Contains(".jpg"))
                    nomeArquivo += ".jpg";
                else if (arquivo.FileName.Contains(".gif"))
                    nomeArquivo += ".gif";
                else if (arquivo.FileName.Contains(".png"))
                    nomeArquivo += ".png";
                else if (arquivo.FileName.Contains(".pdf"))
                    nomeArquivo += ".pdf";
                else
                    nomeArquivo += ".tmp";
                //< obtém o caminho físico da pasta wwwroot >
                string caminho_WebRoot = _webHostEnvironment.WebRootPath;
                // monta o caminho onde vamos salvar o arquivo : 
                // ~\wwwroot\Arquivos\Arquivos_Usuario\Recebidos
                string caminhoDestinoArquivo = caminho_WebRoot + "\\Arquivos\\" + pasta + "\\";
                // incluir a pasta Recebidos e o nome do arquivo enviado : 
                // ~\wwwroot\Arquivos\Arquivos_Usuario\Recebidos\
                string caminhoDestinoArquivoOriginal = caminhoDestinoArquivo + "\\Recebidos\\" + nomeArquivo;
                //copia o arquivo para o local de destino original
                using (var stream = new FileStream(caminhoDestinoArquivoOriginal, FileMode.Create))
                {
                    await arquivo.CopyToAsync(stream);
                }
            }
            //monta a ViewData que será exibida na view como resultado do envio 
            ViewData["Resultado"] = $"{arquivos.Count} arquivos foram enviados ao servidor, " +
             $"com tamanho total de : {tamanhoArquivos} bytes";
            //retorna a viewdata
            return View(ViewData);
        }
    }
}

