using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelExpenses.Web.Data;
using TravelExpenses.Web.Data.Entities;

namespace TravelExpenses.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ExpenseTypeController : ApplicationBaseController
    {
        private readonly DataContext _context;

        public ExpenseTypeController(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.ExpenseTypes.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ExpenseTypeEntity expenseEntity = await _context.ExpenseTypes.FindAsync(id);
            if (expenseEntity == null)
            {
                return NotFound();
            }

            return View(expenseEntity);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpenseTypeEntity expenseEntity)
        {
            if (ModelState.IsValid)
            {
                expenseEntity.Name = expenseEntity.Name.ToUpper();
                _context.Add(expenseEntity);

                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Already exists a expense with the same name.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                    }
                }
            }

            return View(expenseEntity);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ExpenseTypeEntity expenseEntity = await _context.ExpenseTypes.FindAsync(id);
            if (expenseEntity == null)
            {
                return NotFound();
            }

            return View(expenseEntity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExpenseTypeEntity expenseEntity)
        {
            if (id != expenseEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                expenseEntity.Name = expenseEntity.Name.ToUpper();
                _context.Update(expenseEntity);

                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Already exists a taxi with the same plaque.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                    }
                }
            }

            return View(expenseEntity);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ExpenseTypeEntity expenseEntity = await _context.ExpenseTypes.FindAsync(id);
            if (expenseEntity == null)
            {
                return NotFound();
            }

            _context.ExpenseTypes.Remove(expenseEntity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
