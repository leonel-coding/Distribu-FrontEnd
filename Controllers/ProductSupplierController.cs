using DTOLibrary.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DistribuUI.Controllers
{
    public class ProductSupplierController : Controller
    {
        private ISupplierRepository _supplierRepository;

        private IProductSupplierRepository _productSupplierRepository;

        public ProductSupplierController(ISupplierRepository supplierRepository, IProductSupplierRepository productSupplierRepository)
        {
            _supplierRepository = supplierRepository;
            _productSupplierRepository = productSupplierRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SupplierToProduct(int productId, int productSupplierId)
        {
            try
            {
                ViewBag.TittleSupplierToProduct = "Agregar un proveedor al producto";

                ProductSupplierDto productSupplierDto = new ProductSupplierDto();

                productSupplierDto.ProductId = productId;

                var suppliers = await _supplierRepository.GetSuppliers();

                if (productSupplierId != 0)
                {
                    ViewBag.TittleSupplierToProduct = "Editar detalles de producto/proveedor";

                    var productSupplierResult = await _productSupplierRepository.GetProductSupplierById(productSupplierId);

                    if (productSupplierResult != null)
                    {
                        productSupplierDto = productSupplierResult;

                        ViewBag.Suppliers = suppliers.Where(a => a.SupplierId == productSupplierDto.SupplierId).ToList();
                    }
                }
                else
                {
                    ViewBag.Suppliers = suppliers;
                }

                return View(productSupplierDto);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Error", "Home", new { description = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddSupplier(ProductSupplierDto productSupplier)
        {

            try
            {
                if(productSupplier.ProductSupplierId == 0) 
                {
                    await _productSupplierRepository.InsertProductSupplier(productSupplier);
                }
                else
                {
                    await _productSupplierRepository.UpdateProductSupplier(productSupplier);
                }

                return RedirectToAction("Product", "Product", new { productId = productSupplier.ProductId });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { description = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProductSupplier(int productSupplierId)
        {
            try
            {
                await _productSupplierRepository.DeleteProductSupplier(productSupplierId);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { description = ex.Message });
            }

            return NoContent();
        }
    }
}
