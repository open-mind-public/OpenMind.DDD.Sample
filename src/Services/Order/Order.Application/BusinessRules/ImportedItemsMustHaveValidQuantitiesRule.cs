using BuildingBlocks.Domain.BusinessRules;

namespace Order.Application.BusinessRules;

/// <summary>
/// Business rule: All items in the imported order must have valid quantities.
/// </summary>
public class ImportedItemsMustHaveValidQuantitiesRule(IEnumerable<int> quantities) : IBusinessRule
{
    public bool IsBroken() => quantities.Any(qty => qty <= 0);

    public string Message => "All items must have a quantity greater than zero.";
    
    public string Code => "INVALID_ITEM_QUANTITY";
}
