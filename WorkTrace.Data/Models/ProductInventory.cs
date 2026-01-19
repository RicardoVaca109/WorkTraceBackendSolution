using WorkTrace.Data.Common.Generics;

namespace WorkTrace.Data.Models;

public class ProductInventory : BaseModel
{
    public string Code { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int QuantityBalance { get; set; }
    public decimal AverageCostBalance { get; set; }
    public decimal TotalCostBalance { get; set; }
}