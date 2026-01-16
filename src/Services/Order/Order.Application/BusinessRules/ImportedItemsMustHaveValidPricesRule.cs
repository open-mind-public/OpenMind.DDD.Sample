using BuildingBlocks.Domain.BusinessRules;

namespace Order.Application.BusinessRules;

/// <summary>
/// Business rule: All items in the imported order must have valid prices.
/// </summary>
public class ImportedItemsMustHaveValidPricesRule(IEnumerable<decimal> unitPrices) : IBusinessRule
{
    public bool IsBroken() => unitPrices.Any(price => price <= 0);

    public string Message => "All items must have a positive unit price.";
    
    public string Code => "INVALID_ITEM_PRICE";
}
