using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelExpenses.Web.Data;
using TravelExpenses.Web.Data.Entities;
using TravelExpenses.Web.Models.Country;

namespace TravelExpenses.Web.Controllers
{
    public class CountryController : ApplicationBaseController
    {
        private readonly DataContext _context;

        public CountryController(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Countries.Include(o => o.Cities).ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CountryEntity countryEntity)
        {
            if (ModelState.IsValid)
            {
                countryEntity.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(countryEntity.Name.ToLower());
                countryEntity.ShortCode = countryEntity.ShortCode.ToUpper();
                _context.Add(countryEntity);

                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Already exists a Country with the same Name.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                    }
                }
            }

            return View(countryEntity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CountryEntity countryEntity)
        {
            if (id != countryEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                countryEntity.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(countryEntity.Name.ToLower());
                countryEntity.ShortCode = countryEntity.ShortCode.ToUpper();
                _context.Update(countryEntity);

                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Already exists a Country with the same name.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                    }
                }
            }

            return View(countryEntity);
        }

        public async Task<IActionResult> Cities(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cities = await _context.Countries
                .Include(o => o.Cities)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cities == null)
            {
                return NotFound();
            }

            return View(cities);
        }

        public async Task<IActionResult> AddCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries.FindAsync(id.Value);
            if (country == null)
            {
                return NotFound();
            }

            var model = new CityViewModel
            {
                CountryId = country.Id
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddCity(CityViewModel model)
        {
            if (ModelState.IsValid)
            {
                var city = new CityEntity
                {
                    Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(model.Name.ToLower()),
                    Country = await _context.Countries.FindAsync(model.CountryId),
                };

                _context.Cities.Add(city);
                await _context.SaveChangesAsync();
                return RedirectToAction("Cities", "Country", new { id = model.CountryId } );
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CountryEntity countryEntity = await _context.Countries.FindAsync(id);
            if (countryEntity == null)
            {
                return NotFound();
            }

            _context.Countries.Remove(countryEntity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
