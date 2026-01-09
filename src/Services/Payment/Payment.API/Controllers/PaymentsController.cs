using MediatR;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.Commands;
using Payment.Application.DTOs;
using Payment.Application.Queries;

namespace Payment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController(IMediator mediator, ILogger<PaymentsController> logger) : ControllerBase
{
    [HttpGet("{paymentId:guid}")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid paymentId, CancellationToken cancellationToken)
    {
        var query = new GetPaymentByIdQuery { PaymentId = paymentId };
        var result = await mediator.Send(query, cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
    
    [HttpGet("order/{orderId:guid}")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByOrderId(Guid orderId, CancellationToken cancellationToken)
    {
        var query = new GetPaymentByOrderIdQuery { OrderId = orderId };
        var result = await mediator.Send(query, cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
    
    [HttpGet("customer/{customerId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCustomer(Guid customerId, CancellationToken cancellationToken)
    {
        var query = new GetPaymentsByCustomerQuery { CustomerId = customerId };
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
    
    [HttpGet("pending")]
    [ProducesResponseType(typeof(IReadOnlyList<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPending(CancellationToken cancellationToken)
    {
        var query = new GetPendingPaymentsQuery();
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePaymentDto request, CancellationToken cancellationToken)
    {
        var command = new CreatePaymentCommand
        {
            OrderId = request.OrderId,
            CustomerId = request.CustomerId,
            Amount = request.Amount,
            Currency = request.Currency,
            Method = request.Method,
            CardDetails = request.CardDetails
        };

        try
        {
            var paymentId = await mediator.Send(command, cancellationToken);
            logger.LogInformation("Payment {PaymentId} created", paymentId);
            return CreatedAtAction(nameof(GetById), new { paymentId }, paymentId);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPost("{paymentId:guid}/process")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Process(Guid paymentId, CancellationToken cancellationToken)
    {
        var command = new ProcessPaymentCommand { PaymentId = paymentId };

        try
        {
            var result = await mediator.Send(command, cancellationToken);

            if (!result)
                return NotFound();

            logger.LogInformation("Payment {PaymentId} processing started", paymentId);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPost("{paymentId:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Complete(
        Guid paymentId,
        [FromBody] CompletePaymentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CompletePaymentCommand
        {
            PaymentId = paymentId,
            TransactionId = request.TransactionId
        };

        try
        {
            var result = await mediator.Send(command, cancellationToken);

            if (!result)
                return NotFound();

            logger.LogInformation("Payment {PaymentId} completed", paymentId);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPost("{paymentId:guid}/fail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Fail(
        Guid paymentId,
        [FromBody] FailPaymentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new FailPaymentCommand
        {
            PaymentId = paymentId,
            Reason = request.Reason
        };

        try
        {
            var result = await mediator.Send(command, cancellationToken);

            if (!result)
                return NotFound();

            logger.LogWarning("Payment {PaymentId} failed. Reason: {Reason}", paymentId, request.Reason);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPost("{paymentId:guid}/refund")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Refund(
        Guid paymentId,
        [FromBody] RefundPaymentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RefundPaymentCommand
        {
            PaymentId = paymentId,
            Reason = request.Reason
        };

        try
        {
            var result = await mediator.Send(command, cancellationToken);

            if (!result)
                return NotFound();

            logger.LogInformation("Payment {PaymentId} refunded. Reason: {Reason}", paymentId, request.Reason);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

public record CompletePaymentRequest(string TransactionId);
public record FailPaymentRequest(string Reason);
public record RefundPaymentRequest(string Reason);
