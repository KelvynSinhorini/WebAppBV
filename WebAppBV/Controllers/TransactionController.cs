using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebAppBV.Data;
using WebAppBV.Helpers;
using WebAppBV.Models;
using WebAppBV.ViewModels;

namespace WebAppBV.Controllers
{
    public class TransactionController : Controller
    {
        private readonly BVContext _context;

        public TransactionController(BVContext context)
        {
            _context = context;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            // TODO Tirar a ordenação e colocar filtros na tela

            var transactions = await _context.Transactions.ToListAsync();

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
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(m => m.TransactionId == id);
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
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(transactionViewModel);
        }

        // GET: Transaction/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);
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
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.TransactionId))
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
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(m => m.TransactionId == id);
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
            if (_context.Transactions == null)
            {
                return Problem("Entity set 'BVContext.Transactions'  is null.");
            }
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(Guid id)
        {
          return _context.Transactions.Any(e => e.TransactionId == id);
        }
    }
}
