using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyApp.DataAccesslayer;
using MyApp.DataAccessLayer.Infrastructure.IRepository;
using MyApp.Models;
using MyApp.Models.ViewModel;

namespace MyAppWeb.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            ProductVM ProductVM = new ProductVM();
            ProductVM.Products = _unitOfWork.Product.GetAll();
            foreach (var item in ProductVM.Products)
            {
                item.Category = _unitOfWork.Category.GetT(x => x.Id == item.Id);
            }

            return View(ProductVM);
        }

        [HttpGet]
        public IActionResult CreateUpdate(int? id)
        {
            ProductVM ProductVM = new ProductVM()
            {
                Product = new(),
                Categories = _unitOfWork.Category.GetAll().Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            if (id == null || id == 0)
            {
                return View(ProductVM);
            }
            else
            {
                ProductVM.Product = _unitOfWork.Product.GetT(x => x.Id == id);
                if (ProductVM.Product == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(ProductVM);
                }
            }
        }

        [HttpPost]
        public IActionResult CreateUpdate(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string fileName = string.Empty;
                if (file != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "ProductImage");
                    fileName = Guid.NewGuid().ToString() + "-" + file.FileName;
                    string filePath = Path.Combine(uploadDir, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\ProductImage\" + fileName;
                }
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var Product = _unitOfWork.Product.GetT(x => x.Id == id);
            if (Product == null)
            {
                return NotFound();
            }
            return View(Product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(product);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View("Index");
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var Product = _unitOfWork.Product.GetT(x => x.Id == id);
            if (Product == null)
            {
                return NotFound();
            }
            _unitOfWork.Product.Delete(Product);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

    }
}
