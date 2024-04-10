using DTOLibrary.Dtos;
using DTOLibrary.Extension;
using DTOLibrary.Interfaces;
using DTOLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using ServicesREST_API.RESTService;

namespace DistribuUI.Controllers
{
    public class ProductController : Controller
    {
        private IProductRepository _productService;

        private ITypeProductRepository _typeProductRepository;

        public ProductController(IProductRepository productService, ITypeProductRepository typeProductRepository)
        {
            _productService = productService;
            _typeProductRepository = typeProductRepository;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.TittleMain = "Todos los productos";

                var typeProduct = await _typeProductRepository.GetTypeProduct();

                ViewBag.TypeProduct = typeProduct.ToList();

                var productResult = await _productService.GetProducts();

                return View(productResult.ToList());
            }
            catch(Exception ex)
            {
                return RedirectToAction("Error", "Home", new { description = ex.Message } );
            }
        }

        public async Task<IActionResult> GetProductsByTypeProdId(int typeProductId)
        {
            var productResult = await _productService.GetProducts();

            if(typeProductId != 0)
            {
                productResult = productResult.Where(a => a.TypeProductId == typeProductId).ToList();
            }

            return PartialView("_Products", productResult);
        }

        public async Task<IActionResult> Product(int productId)
        {
            try
            {
                ViewBag.TittleMain = "Producto";

                ProductViewModel product = new ProductViewModel();

                if (productId != 0)
                {
                    ViewBag.ProductResult = "Editar producto";

                    var productResult = await _productService.GetProductWithRelationsSuppliersByProductId(productId);

                    if (productResult != null)
                    {
                        product = productResult;
                    }
                    else
                    {
                        return Redirect("Index");
                    }
                }
                else
                {
                    ViewBag.ProductResult = "Crear un nuevo producto";
                }

                var typeProducts = await _typeProductRepository.GetTypeProduct();

                ViewBag.TypesProduct = typeProducts;

                return View(product);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Error", "Home", new { description = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProductEditCreate(ProductViewModel product)
        {
            try
            {

                ProductDto productDto = new ProductDto();

                productDto = product.MapProperties(productDto);

                if (product.ProductId == 0)
                {
                    var result = await _productService.InsertProduct(productDto);

                    return RedirectToAction("Product", new { productId = result.ProductId });
                }
                else
                {
                    await _productService.UpdateProduct(productDto);

                    return NoContent();
                }
            }
            catch(Exception ex)
            {
                return RedirectToAction("Error", "Home", new { description = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProduct(int productId) 
        {
            try
            {
                await _productService.DeleteProduct(productId);

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                return RedirectToAction("Error", "Home", new { description = ex.Message });
            }
        }

        public async Task<IActionResult> GetSuppliersByProductView(int productId)
        {
            var productResult = await _productService.GetProductWithRelationsSuppliersByProductId(productId);

            return PartialView("_SuppliersByProduct", productResult);
        }
    }
}
