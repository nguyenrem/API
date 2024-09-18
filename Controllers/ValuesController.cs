using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using demo_getproduct.API.Models;
using HtmlAgilityPack.CssSelectors.NetCore; // Sử dụng thư viện CSS selectors 

namespace demo_getproduct.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // Method phụ để thực hiện tải và phân tích HTML 
        private async Task<HtmlDocument> LoadHtmlDocument(string url)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml+json");

            HttpResponseMessage response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(content);
                return htmlDoc;
            }
            else
            {
                return null; // Trả về null nếu không lấy được HTML 
            }
        }

        // Method phụ để tạo đối tượng ProductInfo 
        private ProductInfo ParseProductInfo(HtmlDocument htmlDoc)
        {
            // Lấy tên sản phẩm từ class "p-name" 
            var nameNode = htmlDoc.QuerySelector(".p-name");
            string productName = nameNode?.InnerText?.Trim() ?? "Unknown Product";

            // Lấy giá sản phẩm từ class "p-price .show-him" 
            var priceNode = htmlDoc.QuerySelector(".p-price .show-him");
            string productPrice = priceNode?.InnerText?.Trim() ?? "Unknown Price";

            // Lấy danh sách hình ảnh từ thẻ <img> bên trong div có class "p-img" 
            var imageNodes = htmlDoc.QuerySelectorAll(".p-img img");
            List<string> imageUrls = new List<string>();

            foreach (var img in imageNodes)
            {
                string imageUrl = img.GetAttributeValue("src", "");
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    imageUrls.Add(imageUrl);
                }
            }

            // Tạo đối tượng ProductInfo và trả về 
            return new ProductInfo(productName, productPrice, imageUrls);
        }

        // API để lấy đối tượng ProductInfo đầy đủ 
        [HttpGet("GetProductInfo")]
        public async Task<IActionResult> GetProductInfo([FromQuery] string url)
        {
            var htmlDoc = await LoadHtmlDocument(url);
            if (htmlDoc == null)
                return StatusCode(500, "Lỗi khi tải trang.");

            // Tạo đối tượng ProductInfo 
            var productInfo = ParseProductInfo(htmlDoc);

            return Ok(productInfo);
        }

        // API để lấy tên sản phẩm 
        [HttpGet("GetProductName")]
        public async Task<IActionResult> GetProductName([FromQuery] string url)
        {
            var htmlDoc = await LoadHtmlDocument(url);
            if (htmlDoc == null)
                return StatusCode(500, "Lỗi khi tải trang.");

            // Lấy tên sản phẩm 
            var productInfo = ParseProductInfo(htmlDoc);
            return Ok(productInfo.Name);
        }

        // API để lấy giá sản phẩm 
        [HttpGet("GetProductPrice")]
        public async Task<IActionResult> GetProductPrice([FromQuery] string url)
        {
            var htmlDoc = await LoadHtmlDocument(url);
            if (htmlDoc == null)
                return StatusCode(500, "Lỗi khi tải trang.");

            // Lấy giá sản phẩm 
            var productInfo = ParseProductInfo(htmlDoc);
            return Ok(productInfo.Price);
        }

        // API để lấy danh sách các hình ảnh sản phẩm 
        [HttpGet("GetProductImages")]
        public async Task<IActionResult> GetProductImages([FromQuery] string url)
        {
            var htmlDoc = await LoadHtmlDocument(url);
            if (htmlDoc == null)
                return StatusCode(500, "Lỗi khi tải trang.");

            // Lấy danh sách hình ảnh 
            var productInfo = ParseProductInfo(htmlDoc);
            return Ok(productInfo.Images);
        }
    }
}
