using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Websecurity___Essay_2.Data;
using Websecurity___Essay_2.Models;
using Websecurity___Essay_2.Utilities;

namespace Websecurity___Essay_2.Controllers
{
    public class UserUploadFilesController : Controller
    {
        private readonly Context _context;
        private readonly long fileSizeLimit = 10 * 1048576;
        private readonly string[] permittedExtensions = { ".jpg" };

        public UserUploadFilesController(Context context)
        {
            _context = context;
        }

        // GET: UserUploadFiles
        public async Task<IActionResult> Index()
        {
            return View(await _context.UserUploadFile.ToListAsync());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userUploadFile = await _context.UserUploadFile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userUploadFile == null)
            {
                return NotFound();
            }

            return View(userUploadFile);
        }

        [HttpPost]
        [Route(nameof(UploadFile))]
        public async Task<IActionResult> UploadFile()
        {
            var theWebRequest = HttpContext.Request;

            if (!theWebRequest.HasFormContentType ||
                !MediaTypeHeaderValue.TryParse(theWebRequest.ContentType, out var theMediaTypeHeader) ||
                string.IsNullOrEmpty(theMediaTypeHeader.Boundary.Value))
            {
                return new UnsupportedMediaTypeResult();
            }

            var reader = new MultipartReader(theMediaTypeHeader.Boundary.Value, theWebRequest.Body);
            var section = await reader.ReadNextSectionAsync();

            while (section != null)
            {
                var DoesItHaveContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition,
                    out var theContentDisposition);

                if (DoesItHaveContentDispositionHeader && theContentDisposition.DispositionType.Equals("form-data") &&
                    !string.IsNullOrEmpty(theContentDisposition.FileName.Value))
                {
                    UserUploadFile userUploadFile = new ();
                    userUploadFile.FileName = HttpUtility.HtmlEncode(theContentDisposition.FileName.Value);
                    userUploadFile.TimeStamp = DateTime.UtcNow;

                    userUploadFile.Content =
                            await FileHelper.ProcessStreamedFile(section, theContentDisposition,
                                ModelState, permittedExtensions, fileSizeLimit);
                    if (userUploadFile.Content.Length == 0)
                    {
                        return RedirectToAction("Index", "UserUploadFiles");
                    }
                    userUploadFile.Size = userUploadFile.Content.Length;

                    await _context.UserUploadFile.AddAsync(userUploadFile);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "UserUploadFiles");

                }

                section = await reader.ReadNextSectionAsync();
            }
            return BadRequest("No files data in the request.");
        }

        public async Task<IActionResult> Download(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userUploadFile = await _context.UserUploadFile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userUploadFile == null)
            {
                return NotFound();
            }

            return File(userUploadFile.Content, MediaTypeNames.Application.Octet, userUploadFile.FileName);
        }

        // GET: UserUploadFiles/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userUploadFile = await _context.UserUploadFile.FindAsync(id);
            if (userUploadFile == null)
            {
                return NotFound();
            }
            return View(userUploadFile);
        }

        // POST: UserUploadFiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,FileName,TimeStamp,Size,Content")] UserUploadFile userUploadFile)
        {
            if (id != userUploadFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userUploadFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserUploadFileExists(userUploadFile.Id))
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
            return View(userUploadFile);
        }

        // GET: UserUploadFiles/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userUploadFile = await _context.UserUploadFile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userUploadFile == null)
            {
                return NotFound();
            }

            return View(userUploadFile);
        }

        // POST: UserUploadFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var userUploadFile = await _context.UserUploadFile.FindAsync(id);
            _context.UserUploadFile.Remove(userUploadFile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserUploadFileExists(Guid id)
        {
            return _context.UserUploadFile.Any(e => e.Id == id);
        }
    }
}
