using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Websecurity___Essay_2.Data;
using Websecurity___Essay_2.Models;

namespace Websecurity___Essay_2.Controllers
{
    public class UserInputsController : Controller
    {
        private readonly Context _context;
        public List<string> allowedTags { get; set; }

        public List<string> allowedSpecialChar { get; set; }

        public UserInputsController(Context context)
        {
            _context = context;
            allowedTags = new List<string>() { 
                "<b>",
                "</b>",
                "<i>",
                "</i>" 
            };

            allowedSpecialChar = new List<string>() { 
                "&#96;", 
                "&#39;", 
                "&#233;", 
                "&#252" 
            };
        }

        // GET: UserInputs
        public async Task<IActionResult> Index()
        {
            return View(await _context.UserInput.ToListAsync());
        }

        // GET: UserInputs/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userInput = await _context.UserInput
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userInput == null)
            {
                return NotFound();
            }

            return View(userInput);
        }

        // GET: UserInputs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserInputs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Content,Author")] UserInput userInput)
        {
            if (ModelState.IsValid)
            {
                userInput.Id = Guid.NewGuid();
                userInput.TimeStamp = DateTime.UtcNow;
                string encodedInput = HttpUtility.HtmlEncode(userInput.Content);
                foreach (var tag in allowedTags)
                {
                    string encodedTag = HttpUtility.HtmlEncode(tag);
                    encodedInput = encodedInput.Replace(encodedTag, tag);
                }
                userInput.Content = encodedInput;

                string encodedAuthor = HttpUtility.HtmlEncode(userInput.Author);
                foreach (var specialchar in allowedSpecialChar)
                {
                    string encodedChar = HttpUtility.HtmlEncode(specialchar);
                    encodedAuthor = encodedAuthor.Replace(encodedChar, specialchar);
                }
                userInput.Author = encodedAuthor;
                _context.Add(userInput);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userInput);
        }

        // GET: UserInputs/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userInput = await _context.UserInput.FindAsync(id);
            if (userInput == null)
            {
                return NotFound();
            }
            return View(userInput);
        }

        // POST: UserInputs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Content,Author,TimeStamp")] UserInput userInput)
        {
            if (id != userInput.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userInput);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserInputExists(userInput.Id))
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
            return View(userInput);
        }

        // GET: UserInputs/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userInput = await _context.UserInput
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userInput == null)
            {
                return NotFound();
            }

            return View(userInput);
        }

        // POST: UserInputs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var userInput = await _context.UserInput.FindAsync(id);
            _context.UserInput.Remove(userInput);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserInputExists(Guid id)
        {
            return _context.UserInput.Any(e => e.Id == id);
        }
    }
}
