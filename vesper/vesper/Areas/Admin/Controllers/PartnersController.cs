using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using vesper.DAL;
using vesper.Models;

namespace vesper.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PartnersController : Controller
    {
        private readonly VesperDbContext db;
        public PartnersController(VesperDbContext _db)
        {
            db = _db;
        }
        public async Task<IActionResult> Index()
        {
            return View(await db.Partners.ToListAsync());
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Partner partner)
        {
            if (!ModelState.IsValid) return View();
            Partner duplicate = await db.Partners.FirstOrDefaultAsync(x => x.Name == partner.Name);
            if (duplicate != null)
            {
                ModelState.AddModelError("Name", "Name unique olmalidir.");
                return View();
            }

            if (!partner.Image.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Image", "Fayl sekil olmalidir.");
                return View();
            }

            if (partner.Image.Length / 1024 > 500)
            {
                ModelState.AddModelError("Image", "Fayl maksimum 500kb ola biler.");
                return View();
            }

            string path = @"C:\Users\Bashir Azizov\Desktop\p411\p411-07-mar-22\vesper\vesper\wwwroot\assets";
            string folder = @"img\clients";
            string fileName = Guid.NewGuid().ToString() + "-" + partner.Image.FileName;
            string finalPath = Path.Combine(path, folder, fileName);
            FileStream fileStream = new FileStream(finalPath, FileMode.Create);
            await partner.Image.CopyToAsync(fileStream);

            partner.Img = fileName;

            await db.Partners.AddAsync(partner);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Partners");
        }
    }
}
