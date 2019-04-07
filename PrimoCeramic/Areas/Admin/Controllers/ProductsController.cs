using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrimoCeramic.Data;
using PrimoCeramic.Models.ViewModel;
using PrimoCeramic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting.Internal;
using System.IO;
using PrimoCeramic.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.FileProviders;
using System.Text;

namespace PrimoCeramic.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly HostingEnvironment _hostingEnvironment;
        [BindProperty]
        public ProductsViewModel ProductsVM { get; set; }

        [BindProperty]
        public ProductsListViewModel ProductsListViewModel { get; set; }
        private int PageSize = 30;
        public ProductsController(ApplicationDbContext db, HostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            
            ProductsVM = new ProductsViewModel()
            {
                ProductTypes = _db.ProductTypes.ToList(),
                SpecialTags = _db.SpecialTags.ToList(),
                ProductApplication = _db.ProductApplication.ToList(),
                ProductCountry = _db.ProductCountry.ToList(),
                ProductSurface = _db.ProductSurface.ToList(),
                ProductImages =new List<ProductImages>(),
                Products = new Products()
                
            };

            ProductsListViewModel = new ProductsListViewModel()
            {

                ProductTypes = ProductsVM.ProductTypes,
                SpecialTags = ProductsVM.SpecialTags,
                ProductApplication = ProductsVM.ProductApplication,
                ProductCountry = ProductsVM.ProductCountry,
                ProductSurface = ProductsVM.ProductSurface,
                Products =new List<Products>()
            };
        }

        public async Task<IActionResult> Index(string SearchName, int productPage = 1,int TagNumber=0,int TypeNumber = 0)
        {
            ProductsListViewModel.Products = await _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).Include(m => m.ProductCountry).Include(m => m.ProductApplication).Include(m => m.ProducSurface).Where(x => x.Deleted == false).ToListAsync();

            
          var SpecialTagsList = ProductsListViewModel.SpecialTags.ToList();
          var ProductTypesList = ProductsListViewModel.ProductTypes.ToList();


            SpecialTagsList.Add(new SpecialTags()
            {

                Id = 0,
                Name = "SpecialTags"


            });
            SpecialTagsList.Reverse();
            ProductsListViewModel.SpecialTags = SpecialTagsList;


            ProductTypesList.Add(new ProductTypes()
            {

                Id = 0,
                Name = "ProductTypes"


            });
            ProductTypesList.Reverse();
            ProductsListViewModel.ProductTypes = ProductTypesList;

            StringBuilder param = new StringBuilder();

            param.Append("/Admin/Products?productPage=:");

            param.Append("&SearchName=");

            if (SearchName != null)
            {
                param.Append(SearchName);
                ProductsListViewModel.Products = ProductsListViewModel.Products.Where(x => x.Name.ToLower().Contains(SearchName)).ToList();
            }

            if (TagNumber != 0)
            {
                ProductsListViewModel.Products = ProductsListViewModel.Products.Where(a => a.SpecialTagsId == TagNumber).ToList();


               
                param.Append("&TagNumber=");
                param.Append(TagNumber);

            }
            if (TypeNumber != 0)
            {
                ProductsListViewModel.Products = ProductsListViewModel.Products.Where(a => a.ProductTypeId == TypeNumber).ToList();



                param.Append("&TypeNumber=");
                param.Append(TypeNumber);

            }
            var count = ProductsListViewModel.Products.Count;

            ProductsListViewModel.Products = ProductsListViewModel.Products
                .Skip((productPage - 1) * PageSize)
                .Take(PageSize).ToList();

            ProductsListViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemPerPage = PageSize,
                TotalItem = count,
                urlParam = param.ToString()
            };


            ProductsListViewModel.SearchName = SearchName;
            ProductsListViewModel.SelectedTag = TagNumber;
            ProductsListViewModel.SelectedType = TypeNumber;

            return View(ProductsListViewModel);
        }

        public IActionResult Create()
        {
            return View(ProductsVM);
        }


        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            if (!ModelState.IsValid)
            {
                return View(ProductsVM);
            }

            _db.Products.Add(ProductsVM.Products);
            await _db.SaveChangesAsync();

            //saveimage

            string webRootPath = _hostingEnvironment.WebRootPath;

            var files = HttpContext.Request.Form.Files;

            var productsFromDb = _db.Products.Find(ProductsVM.Products.Id);
           
            if (files.Count != 0)
            {
                int photonumber = 1;
                foreach (var file in files)
                {
                    var uploads = Path.Combine(webRootPath, SD.ImageFolder, ProductsVM.Products.Id.ToString());
                    System.IO.Directory.CreateDirectory(uploads);
                    var extension = Path.GetExtension(file.FileName);
                    using (var filestream = new FileStream(Path.Combine(uploads, photonumber + extension), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    
                    if (photonumber == 1)
                    {
                        productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + @"\" + 1 + extension;
                        productsFromDb.ImageFolder = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id;
                        _db.ProductImages.Add(new ProductImages
                        {
                            Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + @"\" + photonumber + extension,
                            ProductId = productsFromDb.Id,
                            IsDefault=true,
                            Name=photonumber
                        });
                    }
                    else {
                        _db.ProductImages.Add(new ProductImages
                    {
                        Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + @"\" + photonumber + extension,
                        ProductId= productsFromDb.Id,
                        IsDefault=false,
                        Name = photonumber

                        });
                    }

                    photonumber++;
                }

            }
            else//need edit
            {
                var uploads = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultProductImag);
                var uploads2 = Path.Combine(webRootPath, SD.ImageFolder, ProductsVM.Products.Id.ToString());
                System.IO.Directory.CreateDirectory(uploads2);
                System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + @"\" +1 + ".jpg");

                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + @"\"+1+ ".jpg";

                productsFromDb.ImageFolder = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id;
            }


            await _db.SaveChangesAsync();

            return RedirectToAction("Edit",new { id= productsFromDb.Id });


        }



        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            ProductsVM.Products = await _db.Products.Include(m => m.SpecialTags).Include(m => m.ProductTypes).Include(m => m.ProductCountry).Include(m => m.ProductApplication).Include(m => m.ProducSurface)
                .SingleOrDefaultAsync(m => m.Id == id);
            ProductsVM.ProductImages = await _db.ProductImages.Where(x => x.ProductId == ProductsVM.Products.Id).ToListAsync();
            if (ProductsVM.Products == null)
            {
                return NotFound();
            }

            return View(ProductsVM);

        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditConfirm(int? id)
        {
            if (ModelState.IsValid)
            {
               

                var productFromDb = _db.Products.Find(id);
                var ProductImagesDb = await _db.ProductImages.Where(x => x.ProductId == productFromDb.Id).ToListAsync();
                string webRootPath = _hostingEnvironment.WebRootPath;

                var files = HttpContext.Request.Form.Files;

                
                
               
                

                //if (files.Count != 0)
                //{

                //    var Lastimage = _db.ProductImages.Max(x => x.Name);
                    
                //    int photonumber = Lastimage + 1;
                //    //photonumber++;
                //    foreach (var file in files)
                //    {
                //        var uploads = Path.Combine(webRootPath, SD.ImageFolder, id.ToString());
                //        System.IO.Directory.CreateDirectory(uploads);
                //        var extension = Path.GetExtension(file.FileName);
                //        using (var filestream = new FileStream(Path.Combine(uploads, photonumber + extension), FileMode.Create))
                //        {
                //            file.CopyTo(filestream);
                //        }

                //        if (photonumber == 1)
                //        {
                //            productFromDb.Image = @"\" + SD.ImageFolder + @"\" + id + @"\" + photonumber + extension;
                //            productFromDb.ImageFolder = @"\" + SD.ImageFolder + @"\" + id;
                //            _db.ProductImages.Add(new ProductImages
                //            {
                //                Image = @"\" + SD.ImageFolder + @"\" + id + @"\" + photonumber + extension,
                //                ProductId = productFromDb.Id,
                //                IsDefault = true
                //            });
                //        }
                //        else
                //        {
                //            _db.ProductImages.Add(new ProductImages
                //            {
                //                Image = @"\" + SD.ImageFolder + @"\" + id + @"\" + photonumber + extension,
                //                ProductId = productFromDb.Id,
                //                IsDefault = false

                //            });
                //        }

                //        photonumber++;
                //    }

                //}

                var olddefaultimagedb=await _db.ProductImages.Where(x=>x.ProductId == productFromDb.Id&&x.IsDefault == true).FirstOrDefaultAsync();
                var newdefaultimagedb = await _db.ProductImages.Where(x => x.Id == ProductsVM.SelectedRole).FirstOrDefaultAsync();

                if (olddefaultimagedb != null)
                {
                    olddefaultimagedb.IsDefault = false;
                }

               
                    productFromDb.Image = newdefaultimagedb.Image;
              

                productFromDb.Name = ProductsVM.Products.Name;
                productFromDb.Price = ProductsVM.Products.Price;
                productFromDb.Width = ProductsVM.Products.Width;
                productFromDb.Height = ProductsVM.Products.Height;
                productFromDb.Thickness = ProductsVM.Products.Thickness;
                productFromDb.Weight = ProductsVM.Products.Weight;
                productFromDb.Depth = ProductsVM.Products.Depth;
                productFromDb.Casepackaging = ProductsVM.Products.Casepackaging;
                productFromDb.Note = ProductsVM.Products.Note;
                productFromDb.Available = ProductsVM.Products.Available;
                productFromDb.ProductTypeId = ProductsVM.Products.ProductTypeId;
                productFromDb.SpecialTagsId = ProductsVM.Products.SpecialTagsId;
                productFromDb.ProductCountryId = ProductsVM.Products.ProductCountryId;
                productFromDb.ProductApplicationId = ProductsVM.Products.ProductApplicationId;
                productFromDb.ProductSurfaceId = ProductsVM.Products.ProductSurfaceId;
                productFromDb.ShadeColor = ProductsVM.Products.ShadeColor;
                newdefaultimagedb.IsDefault = true;
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));


            }
            else
            {
                return View(ProductsVM);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Addphotos(int? id)
        {
            var files = HttpContext.Request.Form.Files;
            

            var productFromDb = _db.Products.Find(ProductsVM.Products.Id);
            var ProductImagesDb = await _db.ProductImages.Where(x => x.ProductId == productFromDb.Id).ToListAsync();
            string webRootPath = _hostingEnvironment.WebRootPath;

            


            if (files.Count != 0)
            {
                int photonumber = 0;
                if (ProductImagesDb.Count == 0)
                {
                    photonumber = 1;
                }
                else
                {
                    var Lastimage = ProductImagesDb.Max(x => x.Name);

                    photonumber = Lastimage + 1;
                }
             
                //photonumber++;
                foreach (var file in files)
                {
                    var uploads = Path.Combine(webRootPath, SD.ImageFolder, ProductsVM.Products.Id.ToString());
                    System.IO.Directory.CreateDirectory(uploads);
                    var extension = Path.GetExtension(file.FileName);
                    using (var filestream = new FileStream(Path.Combine(uploads, photonumber + extension), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }

                    if (photonumber == 1)
                    {
                        productFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + @"\" + photonumber + extension;
                        productFromDb.ImageFolder = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id;
                        _db.ProductImages.Add(new ProductImages
                        {
                            Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + @"\" + photonumber + extension,
                            ProductId = productFromDb.Id,
                            IsDefault = true,
                            Name=photonumber
                            
                        });
                    }
                    else
                    {
                        _db.ProductImages.Add(new ProductImages
                        {
                            Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + @"\" + photonumber + extension,
                            ProductId = productFromDb.Id,
                            IsDefault = false,
                            Name=photonumber

                        });
                    }

                    photonumber++;
                }

            }
            await _db.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = ProductsVM.Products.Id });
        }

        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            ProductsVM.Products = await _db.Products.Include(m => m.SpecialTags).Include(m => m.ProductTypes).Include(m => m.ProductCountry).Include(m => m.ProductApplication).Include(m => m.ProducSurface)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (ProductsVM.Products == null)
            {
                return NotFound();
            }

            return View(ProductsVM);

        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductsVM.Products = await _db.Products.Include(m => m.SpecialTags).Include(m => m.ProductTypes).Include(m => m.ProductCountry).Include(m => m.ProductApplication).Include(m => m.ProducSurface)
                 .SingleOrDefaultAsync(m => m.Id == id);

            if (ProductsVM.Products == null)
            {
                return NotFound();
            }

            return View(ProductsVM);

        }

        [HttpPost]
        public async Task<IActionResult> DeletePhoto(string photo, int id )
        {
            var image = await _db.ProductImages.Where(x => x.Image == photo).FirstOrDefaultAsync();

            string imagepath = image.Image;
            string webRoothPath = _hostingEnvironment.WebRootPath;
            
          
            var uploads = Path.Combine(webRoothPath, SD.ImageFolder);
            var extension = Path.GetExtension(imagepath);
            var filename = Path.GetFileNameWithoutExtension(imagepath);
                if (System.IO.File.Exists(Path.Combine(uploads, id.ToString() , filename + extension)))
                {
                    System.IO.File.Delete(Path.Combine(uploads, id.ToString(), filename + extension));
                }

            _db.ProductImages.Remove(image);
            await _db.SaveChangesAsync();
                return RedirectToAction("Edit" ,new {id=id });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRoothPath = _hostingEnvironment.WebRootPath;
            Products products = await _db.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }
            else
            {
                //var uploads = Path.Combine(webRoothPath, SD.ImageFolder,products.Id.ToString());


                //if (System.IO.Directory.Exists(uploads))
                //{
                //    System.IO.Directory.Delete(uploads,true);
                //}


                //_db.Products.Remove(products);


                //await _db.SaveChangesAsync();



                products.Deleted = true;
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }



        }
    }
}