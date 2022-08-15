using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebAppBV.Helpers;
using WebAppBV.Services;
using WebAppBV.ViewModels;

namespace WebAppBV.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            // TODO Tirar a ordenação e colocar filtros na tela
            var transactions = await _transactionService.GetAll();

            var transactionsViewModel = transactions.Select(t =>
            {
                var transactionViewModel = Converters.ConvertTransactionToViewModel(t);
                return transactionViewModel;
            });

            return View(transactionsViewModel.OrderBy(t => t.Owner).ThenBy(t => t.Value));
        }

        // GET: Transaction/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _transactionService.Get(id);
            if (transaction == null)
            {
                return NotFound();
            }

            var transactionViewModel = Converters.ConvertTransactionToViewModel(transaction);
            return View(transactionViewModel);
        }

        // GET: Transaction/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Transaction/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransactionViewModel transactionViewModel)
        {
            if (ModelState.IsValid)
            {
                var transaction = Converters.ConvertTransactionViewModelToModel(transactionViewModel);
                transaction.SetNewTransactionId();
                await _transactionService.Create(transaction);
                return RedirectToAction(nameof(Index));
            }
            return View(transactionViewModel);
        }

        // GET: Transaction/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _transactionService.Get(id);
            if (transaction == null)
            {
                return NotFound();
            }
            var transactionViewModel = Converters.ConvertTransactionToViewModel(transaction);
            return View(transactionViewModel);
        }

        // POST: Transaction/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, TransactionViewModel transactionViewModel)
        {
            if (id != transactionViewModel.TransactionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var transaction = Converters.ConvertTransactionViewModelToModel(transactionViewModel);

                try
                {
                    await _transactionService.Edit(transaction);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    transaction = await _transactionService.Get(transactionViewModel.TransactionId);
                    if (transaction == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(transactionViewModel);
        }

        // GET: Transaction/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _transactionService.Get(id);
            if (transaction == null)
            {
                return NotFound();
            }

            var transactionViewModel = Converters.ConvertTransactionToViewModel(transaction);
            return View(transactionViewModel);
        }

        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var transaction = await _transactionService.Get(id);
            if (transaction != null)
            {
                await _transactionService.Delete(transaction);
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}
