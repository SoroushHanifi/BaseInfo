using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.CQRS;

public record GetPaymentWithTimeQuery(long MainTitleId,DateTime StartTime,DateTime EndTime,long NewAmount) : IRequest<List<GetPaymentWithTimeQueryResult>>;
public class GetPaymentWithTimeQueryResult
{
    public long Id { get; set; }
    public string NationalCode { get; set; }
    public long DifferenceAmount { get; set; }

}

public class GetPaymentWithTimeQueryHandler : IRequestHandler<GetPaymentWithTimeQuery, List<GetPaymentWithTimeQueryResult>>
{
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetPaymentWithTimeQueryHandler> _logger;

        public GetPaymentWithTimeQueryHandler(ApplicationDbContext context, ILogger<GetPaymentWithTimeQueryHandler> logger) 
        {
            _context = context; 
            _logger = logger;
        }
        public async Task<List<GetPaymentWithTimeQueryResult>> Handle(GetPaymentWithTimeQuery request, CancellationToken cancellationToken)
        {
            return await _context.Payments.Where(q => q.PaymentDate > request.StartTime && q.PaymentDate <= request.EndTime).Select(q => new GetPaymentWithTimeQueryResult
                {
                    Id = q.Id,
                    DifferenceAmount = request.NewAmount - q.PaymentAmount,
                    NationalCode = q.NationalCode,
                }).AsNoTracking().ToListAsync();
        }
}



 


