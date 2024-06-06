using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Construction;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork _unitOfWork, IWebHostEnvironment _webHostEnvironment)
        {
            this._unitOfWork = _unitOfWork;
            this._webHostEnvironment = _webHostEnvironment;
        }
        [HttpGet]
        public IActionResult Index()
        {
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
           
            return View(products);
        }
        [HttpGet]
        public IActionResult Upsert(int? Id)
        {

            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll()
               .Select(u => new SelectListItem
               {
                   Text = u.Name,
                   Value = u.Id.ToString()
               }),
                Product = new Product()
            };
            if(Id == null || Id == 0)
            {
               //create
                return View(productVM);

            }
            else
            {
                //update
                productVM.Product = _unitOfWork.Product.Get(p => p.Id == Id);
                return View(productVM);
            }
        }

        [HttpPost]  
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if(ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        //delete old image
                        //get absolute path
                        string oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl);
                        //delete
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (FileStream filestream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    productVM.Product.ImageUrl =@"images\product\" + fileName;
                }
               
                if(productVM.Product.Id == 0)
                {
                _unitOfWork.Product.Add(productVM.Product);
                TempData["success"] = "Creat new product successfully";
                }
                else {
                    _unitOfWork.Product.Update(productVM.Product); 
                TempData["success"] = "Update product successfully";

                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll()
                    .Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    });
            }
            return View(productVM);
        }

        #region api call endpoint
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();

            return Json(new {data = products});
        }

        [HttpDelete]
        public IActionResult Delete(int? id) 
        {
            Product? productToBeDelte = _unitOfWork.Product.Get(u => u.Id == id);

            if(productToBeDelte == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDelte.ImageUrl);
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Remove(productToBeDelte);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Deleted" });

        }
        #endregion
    }
}


