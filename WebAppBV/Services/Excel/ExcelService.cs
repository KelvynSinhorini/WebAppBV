using IronXL;
using IronXL.Styles;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebAppBV.Models;

namespace WebAppBV.Services
{
    public class ExcelService : IExcelService
    {
        public async Task<Stream> GenerateExcel(Stream stream, string fileName)
        {
            var content = new StreamContent(stream);
            var json = await content.ReadAsStringAsync();

            var fatura = JsonConvert.DeserializeObject<Fatura>(json);

            var teste = WorkBook.Create();
            var workBook = WorkBook.Create(ExcelFileFormat.XLSX);

            var sheet = workBook.CreateWorkSheet("Transações");
            SetHeader(sheet);

            var movimentacoesNacionais = fatura.detalhesFaturaCartoes.SelectMany(d => d.movimentacoesNacionais).ToList();

            var pagamentoEfetuado = movimentacoesNacionais.FirstOrDefault(m => m.nomeEstabelecimento.Trim() == "PAGAMENTO EFETUADO");
            movimentacoesNacionais.Remove(pagamentoEfetuado);

            var transactions = new List<Transaction>();

            var index = 2;
            foreach (var movimentacao in movimentacoesNacionais.OrderBy(m => m.valorAbsolutoMovimentacaoReal))
            {
                var transaction = new Transaction(movimentacao.dataMovimentacao, movimentacao.nomeEstabelecimento, movimentacao.valorAbsolutoMovimentacaoReal, movimentacao.numeroParcelaMovimentacao, movimentacao.quantidadeTotalParcelas, movimentacao.sinal);

                // SetTransactionPerson(ref transaction, transactionsOfLastMonth);
                SetCellsValue(sheet, index, transaction);

                transactions.Add(transaction);

                index++;
            }

            // CreateSheetTransactionByPerson(workBook, transactions);

            var borderType = IronXL.Styles.BorderType.Thin;
            var borderColor = "#000000";

            SetTotalValueAndStyle(sheet, index, borderType, borderColor);

            SetStyleInAllCells(sheet, index, borderType, borderColor);

            var dateCollumn = sheet[$"A1:A{index + 1}"];
            dateCollumn.Style.HorizontalAlignment = IronXL.Styles.HorizontalAlignment.Left;

            var fileNameToSave = $"fatura_{fileName.Replace(".txt", "")}_{Guid.NewGuid()}";
            var memoryStream = new MemoryStream(workBook.ToByteArray());
            return memoryStream;
            // workBook.SaveAs($@"{PathToSave}\{fileNameToSave}.xlsx");
        }

        private void SetHeader(WorkSheet sheet)
        {
            sheet["A1"].Value = "Data";
            sheet["A1"].Style.HorizontalAlignment = IronXL.Styles.HorizontalAlignment.Center;

            sheet["B1"].Value = "Nome";

            sheet["C1"].Value = "Valor";
            sheet["C1"].Style.HorizontalAlignment = IronXL.Styles.HorizontalAlignment.Center;

            sheet["D1"].Value = "Pessoa";
        }

        private void SetCellsValue(WorkSheet sheet, int index, Transaction transaction)
        {
            // Data Item
            sheet[$"A{index}"].DateTimeValue = transaction.Date;
            sheet[$"A{index}"].FormatString = "dd/MM/yyyy";

            // Descrição item
            sheet[$"B{index}"].Value = transaction.NameTotalAmountAndNumberParcel;

            // Valor
            var sheetValue = sheet[$"C{index}"];
            sheetValue.Value = transaction.Value;
            sheetValue.FormatString = IronXL.Formatting.BuiltinFormats.Currency2;

            // Pessoa
            sheet[$"D{index}"].Value = transaction.Person ?? "";
        }

        private void SetTotalValueAndStyle(WorkSheet sheet, int index, BorderType borderType, string borderColor)
        {
            var totalValue = sheet[$"C{index}"];
            totalValue.Formula = $"=SUM(C2:C{index - 1})";
            totalValue.FormatString = IronXL.Formatting.BuiltinFormats.Currency2;

            totalValue.Style.BottomBorder.Type = borderType;
            totalValue.Style.BottomBorder.SetColor(borderColor);

            totalValue.Style.LeftBorder.Type = borderType;
            totalValue.Style.LeftBorder.SetColor(borderColor);

            totalValue.Style.TopBorder.Type = borderType;
            totalValue.Style.TopBorder.SetColor(borderColor);

            totalValue.Style.RightBorder.Type = borderType;
            totalValue.Style.RightBorder.SetColor(borderColor);
        }

        private void SetStyleInAllCells(WorkSheet sheet, int index, BorderType borderType, string borderColor)
        {
            sheet.AutoSizeColumn(0);
            sheet.AutoSizeColumn(1);
            sheet.AutoSizeColumn(2);

            var allCells = sheet[$"A1:C{index - 1}"];

            allCells.Style.BottomBorder.Type = borderType;
            allCells.Style.BottomBorder.SetColor(borderColor);

            allCells.Style.LeftBorder.Type = borderType;
            allCells.Style.LeftBorder.SetColor(borderColor);

            allCells.Style.TopBorder.Type = borderType;
            allCells.Style.TopBorder.SetColor(borderColor);

            allCells.Style.RightBorder.Type = borderType;
            allCells.Style.RightBorder.SetColor(borderColor);
        }

        public IEnumerable<Transaction> ReadLastMonthExcel()
        {
            //var directoryInfo = new DirectoryInfo(PathToSave);

            //var files = directoryInfo.GetFiles("*.*");

            //var file = files.OrderByDescending(f => f.CreationTime).FirstOrDefault();

            //var workbook = WorkBook.Load(file.FullName);
            //var sheet = workbook.WorkSheets.First(x => x.Name == "Transações");

            //for (int i = 1; i < sheet.Rows.Length; i++)
            //{
            //    var row = sheet.Rows[i];

            //    var date = row.Columns[0].DateTimeValue;
            //    var name = row.Columns[1].StringValue;
            //    var value = row.Columns[2].DecimalValue;
            //    var person = row.Columns[3].StringValue;

            //    yield return new Transaction(date, name, value, person);
            //}

            return null;
        }
    }
}
