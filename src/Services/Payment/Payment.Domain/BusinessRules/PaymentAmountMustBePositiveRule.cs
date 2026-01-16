using BuildingBlocks.Domain.BusinessRules;

namespace Payment.Domain.BusinessRules;

/// <summary>
/// Business rule: Payment amount must be positive.
/// </summary>
public class PaymentAmountMustBePositiveRule(decimal amount) : IBusinessRule
{
    public bool IsBroken() => amount <= 0;

    public string Message => $"Payment amount must be greater than zero. Provided: {amount}.";
    
    public string Code => "INVALID_PAYMENT_AMOUNT";
}
