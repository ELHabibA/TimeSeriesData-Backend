using Timeseriesdata.Models;

public interface IProductsService
{
    Task<List<Product>> GetProductSalesData(DateTime? startTime, DateTime? endTime, string? productName = null);

}
