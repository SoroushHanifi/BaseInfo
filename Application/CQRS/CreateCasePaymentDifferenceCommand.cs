using Application.CQRS;
using Application.Refits;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace Application.CQRS;

public record CreateCasePaymentDifferenceCommand(
    long MainTitleId,
    DateTime StartTime,
    DateTime EndTime,
    long NewAmount) : IRequest<IActionResult>;

public record CreateCasePaymentDifferenceResult(
    int TotalPaymentsProcessed,
    int SuccessfulCalls,
    List<string> FailedPaymentIds);


public class CreateCasePaymentDifferenceCommandHandler :
    IRequestHandler<CreateCasePaymentDifferenceCommand, IActionResult>
{
    private readonly IMediator _mediator;
    private readonly IPaymentOrganization _paymentOrganization;
    private readonly ILogger<CreateCasePaymentDifferenceCommandHandler> _logger;

    public CreateCasePaymentDifferenceCommandHandler(
        IMediator mediator,
        IPaymentOrganization paymentOrganization,
        ILogger<CreateCasePaymentDifferenceCommandHandler> logger)
    {
        _mediator = mediator;
        _paymentOrganization = paymentOrganization;
        _logger = logger;
    }

    public async Task<IActionResult> Handle(
        CreateCasePaymentDifferenceCommand request,
        CancellationToken cancellationToken)
    {
        // ۱. توکن سیستم رو می‌گیریم (همون که توکن SSO که تو مثال داشتی)
        var tokenResult = await _mediator.Send(new GetUserSystemTokenQuery(), cancellationToken);
        // فرض می‌کنیم GetUserSystemTokenQuery یه string برمی‌گردونه
        string cookieHeader = $"token={tokenResult}"; // بسته به فرمت دقیقش ممکنه فرق کنه

        // ۲. لیست پرداخت‌هایی که تفاوت قیمت دارن رو می‌گیریم
        var payments = await _mediator.Send(new GetPaymentWithTimeQuery(
            MainTitleId: request.MainTitleId,
            StartTime: request.StartTime,
            EndTime: request.EndTime,
            NewAmount: request.NewAmount), cancellationToken);

        if (!payments.Any())
        {
            return new OkObjectResult(new CreateCasePaymentDifferenceResult(0, 0, new()));
        }

        var results = new List<string>();
        int successCount = 0;

        // ۳. برای هر پرداخت، درخواست به سازمان پرداخت می‌فرستیم
        foreach (var payment in payments)
        {
            try
            {
                // نکته مهم: بعضی APIها تو مسیر یا کوئری استرینگ Id پرداخت رو می‌خوان
                // چون تو Refit فعلی فقط هدر داره، دو راه داریم:

                // راه ۱ (ترجیحی): Refit رو آپدیت کنیم که Id رو هم بگیره
                // راه ۲: اگر API فقط با توکن کار می‌کنه و خودش می‌دونه کدوم پرداخت، همون رو بزن

                var response = await _paymentOrganization.CreateCaseDifference(cookieHeader,"Payment", 20, 1,payment.NationalCode);

                // اگر API موفق بود (200-299)
                if (response.Data.Status == Bizagi.Application.Models.ActionResultStatus.Complete)
                {
                    successCount++;
                    _logger.LogInformation("Payment {PaymentId} processed successfully.", payment.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while processing Payment {PaymentId}", payment.Id);
                results.Add(payment.Id.ToString());
            }

            await Task.Delay(100, cancellationToken);
        }

        var result = new CreateCasePaymentDifferenceResult(
            TotalPaymentsProcessed: payments.Count,
            SuccessfulCalls: successCount,
            FailedPaymentIds: results);

        return new OkObjectResult(result);
    }
}
