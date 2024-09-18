namespace demo_getproduct.API.Models
{
    public class ProductInfo
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public List<string> Images { get; set; }

        public ProductInfo()
        {
            Images = new List<string>();
        }

        public ProductInfo(string name, string price, List<string> images)
        {
            Name = name;
            Price = price;
            Images = images ?? new List<string>();
        }
    }
}
