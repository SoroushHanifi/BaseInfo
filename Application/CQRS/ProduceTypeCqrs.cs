using Application.Common;
using Domain;
using Domain.Entities.Daroo;
using FluentValidation;
using Infrastructure;
using Infrastructure.Exceptions;
using Infrastructure.Utility;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS
{
    // ===== CREATE COMMAND =====
    public record CreateProduceTypeCommand(
        string Title,
        int? Code = null,
        bool IsActive = true
    ) : IRequest<long>;

    public class CreateProduceTypeCommandHandler : IRequestHandler<CreateProduceTypeCommand, long>
    {
        private readonly ApplicationDbContext _context;

        public CreateProduceTypeCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<long> Handle(CreateProduceTypeCommand request, CancellationToken cancellationToken)
        {
            // بررسی تکراری نبودن عنوان
            var titleExists = await _context.ProduceTypes
                .AnyAsync(pt => pt.Title == request.Title && pt.IsDeleted != true, cancellationToken);

            if (titleExists)
                throw new AppException("عنوان نوع تولید تکراری است");

            // بررسی تکراری نبودن کد در صورت وجود
            if (request.Code.HasValue)
            {
                var codeExists = await _context.ProduceTypes
                    .AnyAsync(pt => pt.Code == request.Code && pt.IsDeleted != true, cancellationToken);

                if (codeExists)
                    throw new AppException("کد نوع تولید تکراری است");
            }

            var produceType = new ProduceType
            {
                Id = _context.GetLastId<ProduceType>() + 1,
                Title = request.Title,
                Code = request.Code,
                IsActive = request.IsActive
            };

            produceType.PrepareForCreation();

            _context.ProduceTypes.Add(produceType);
            await _context.SaveChangesAsync(cancellationToken);

            return produceType.Id;
        }
    }

    // ===== UPDATE COMMAND =====
    public record UpdateProduceTypeCommand(
        long Id,
        string Title,
        int? Code = null,
        bool IsActive = true
    ) : IRequest<bool>;

    public class UpdateProduceTypeCommandHandler : IRequestHandler<UpdateProduceTypeCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public UpdateProduceTypeCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateProduceTypeCommand request, CancellationToken cancellationToken)
        {
            var produceType = await _context.ProduceTypes
                .FirstOrDefaultAsync(pt => pt.Id == request.Id && pt.IsDeleted != true, cancellationToken);

            if (produceType == null)
                return false;

            // بررسی تکراری نبودن عنوان (جز خودش)
            var titleExists = await _context.ProduceTypes
                .AnyAsync(pt => pt.Title == request.Title &&
                               pt.Id != request.Id &&
                               pt.IsDeleted != true, cancellationToken);

            if (titleExists)
                throw new AppException("عنوان نوع تولید تکراری است");

            // بررسی تکراری نبودن کد در صورت وجود (جز خودش)
            if (request.Code.HasValue)
            {
                var codeExists = await _context.ProduceTypes
                    .AnyAsync(pt => pt.Code == request.Code &&
                                   pt.Id != request.Id &&
                                   pt.IsDeleted != true, cancellationToken);

                if (codeExists)
                    throw new AppException("کد نوع تولید تکراری است");
            }

            produceType.Title = request.Title;
            produceType.Code = request.Code;
            produceType.IsActive = request.IsActive;
            produceType.PrepareForUpdate();

            _context.ProduceTypes.Update(produceType);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    // ===== DELETE COMMAND =====
    public record DeleteProduceTypeCommand(long Id) : IRequest<bool>;

    public class DeleteProduceTypeCommandHandler : IRequestHandler<DeleteProduceTypeCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public DeleteProduceTypeCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteProduceTypeCommand request, CancellationToken cancellationToken)
        {
            var produceType = await _context.ProduceTypes
                .FirstOrDefaultAsync(pt => pt.Id == request.Id && pt.IsDeleted != true, cancellationToken);

            if (produceType == null)
                return false;

            produceType.SoftDelete();

            _context.ProduceTypes.Update(produceType);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    // ===== VALIDATORS =====
    public class CreateProduceTypeValidator : AbstractValidator<CreateProduceTypeCommand>
    {
        public CreateProduceTypeValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("عنوان نوع تولید الزامی است")
                .MinimumLength(2).WithMessage("عنوان نوع تولید باید حداقل 2 کاراکتر باشد")
                .MaximumLength(50).WithMessage("عنوان نوع تولید نباید بیشتر از 50 کاراکتر باشد");

            RuleFor(x => x.Code)
                .GreaterThan(0).WithMessage("کد باید عددی مثبت باشد")
                .When(x => x.Code.HasValue);
        }
    }

    // ===== DTOs =====
    public class ProduceTypeDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int? Code { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool? IsDeleted { get; set; }
        public int FinalEnt { get; set; }
        public long BaCreatedTime { get; set; }
        public Guid BaGuid { get; set; }

        public DateTime? BaCreatedDateTime => BaCreatedTime > 0
            ? new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(BaCreatedTime)
            : null;
    }

    public class ProduceTypeSimpleDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int? Code { get; set; }
        public bool? IsActive { get; set; }
    }

    // ===== GET ALL QUERY =====
    public record GetAllProduceTypesQuery : IRequest<List<ProduceTypeDto>>;

    public class GetAllProduceTypesQueryHandler : IRequestHandler<GetAllProduceTypesQuery, List<ProduceTypeDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetAllProduceTypesQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProduceTypeDto>> Handle(GetAllProduceTypesQuery request, CancellationToken cancellationToken)
        {
            return await _context.ProduceTypes
                .Where(pt => pt.IsDeleted != true)
                .OrderBy(pt => pt.Code)
                .ThenBy(pt => pt.Title)
                .Select(pt => new ProduceTypeDto
                {
                    Id = pt.Id,
                    Title = pt.Title ?? "",
                    Code = pt.Code,
                    IsActive = pt.IsActive,
                    CreateDate = pt.CreateDate,
                    IsDeleted = pt.IsDeleted,
                    FinalEnt = pt.FinalEnt,
                    BaCreatedTime = pt.BaCreatedTime,
                    BaGuid = pt.BaGuid
                })
                .ToListAsync(cancellationToken);
        }
    }

    // ===== GET BY ID QUERY =====
    public record GetProduceTypeByIdQuery(long Id) : IRequest<ProduceTypeDto?>;

    public class GetProduceTypeByIdQueryHandler : IRequestHandler<GetProduceTypeByIdQuery, ProduceTypeDto?>
    {
        private readonly ApplicationDbContext _context;

        public GetProduceTypeByIdQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProduceTypeDto?> Handle(GetProduceTypeByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.ProduceTypes
                .Where(pt => pt.Id == request.Id && pt.IsDeleted != true)
                .Select(pt => new ProduceTypeDto
                {
                    Id = pt.Id,
                    Title = pt.Title ?? "",
                    Code = pt.Code,
                    IsActive = pt.IsActive,
                    CreateDate = pt.CreateDate,
                    IsDeleted = pt.IsDeleted,
                    FinalEnt = pt.FinalEnt,
                    BaCreatedTime = pt.BaCreatedTime,
                    BaGuid = pt.BaGuid
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }

    // ===== GET ACTIVE QUERY =====
    public record GetActiveProduceTypesQuery : IRequest<List<ProduceTypeSimpleDto>>;

    public class GetActiveProduceTypesQueryHandler : IRequestHandler<GetActiveProduceTypesQuery, List<ProduceTypeSimpleDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetActiveProduceTypesQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProduceTypeSimpleDto>> Handle(GetActiveProduceTypesQuery request, CancellationToken cancellationToken)
        {
            return await _context.ProduceTypes
                .Where(pt => pt.IsDeleted != true && pt.IsActive == true)
                .OrderBy(pt => pt.Code)
                .ThenBy(pt => pt.Title)
                .Select(pt => new ProduceTypeSimpleDto
                {
                    Id = pt.Id,
                    Title = pt.Title ?? "",
                    Code = pt.Code,
                    IsActive = pt.IsActive
                })
                .ToListAsync(cancellationToken);
        }
    }

    // ===== PAGINATION QUERY =====
    public record GetAllProduceTypesPaginationQuery(int PageIndex = 1, int PageSize = 10) : IRequest<PagedData<ProduceTypeDto>>;

    public class GetAllProduceTypesPaginationQueryHandler : IRequestHandler<GetAllProduceTypesPaginationQuery, PagedData<ProduceTypeDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetAllProduceTypesPaginationQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedData<ProduceTypeDto>> Handle(GetAllProduceTypesPaginationQuery request, CancellationToken cancellationToken)
        {
            int pageIndex = request.PageIndex < 1 ? 1 : request.PageIndex;
            int pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            int totalCount = await _context.ProduceTypes
                .Where(pt => pt.IsDeleted != true)
                .CountAsync(cancellationToken);

            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await _context.ProduceTypes
                .Where(pt => pt.IsDeleted != true)
                .OrderBy(pt => pt.Code)
                .ThenBy(pt => pt.Title)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(pt => new ProduceTypeDto
                {
                    Id = pt.Id,
                    Title = pt.Title ?? "",
                    Code = pt.Code,
                    IsActive = pt.IsActive,
                    CreateDate = pt.CreateDate,
                    IsDeleted = pt.IsDeleted,
                    FinalEnt = pt.FinalEnt,
                    BaCreatedTime = pt.BaCreatedTime,
                    BaGuid = pt.BaGuid
                })
                .ToListAsync(cancellationToken);

            return new PagedData<ProduceTypeDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                IndexFrom = (pageIndex - 1) * pageSize,
                Items = items,
                HasPreviousPage = pageIndex > 1,
                HasNextPage = pageIndex < totalPages
            };
        }
    }

    // ===== SEARCH QUERY =====
    public record SearchProduceTypesQuery(string? SearchTerm = null, bool? IsActive = null) : IRequest<List<ProduceTypeDto>>;

    public class SearchProduceTypesQueryHandler : IRequestHandler<SearchProduceTypesQuery, List<ProduceTypeDto>>
    {
        private readonly ApplicationDbContext _context;

        public SearchProduceTypesQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProduceTypeDto>> Handle(SearchProduceTypesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.ProduceTypes
                .Where(pt => pt.IsDeleted != true);

            // فیلتر جستجوی متنی
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim();
                query = query.Where(pt => pt.Title != null && pt.Title.Contains(searchTerm));
            }

            // فیلتر وضعیت فعال/غیرفعال
            if (request.IsActive.HasValue)
            {
                query = query.Where(pt => pt.IsActive == request.IsActive.Value);
            }

            return await query
                .OrderBy(pt => pt.Code)
                .ThenBy(pt => pt.Title)
                .Select(pt => new ProduceTypeDto
                {
                    Id = pt.Id,
                    Title = pt.Title ?? "",
                    Code = pt.Code,
                    IsActive = pt.IsActive,
                    CreateDate = pt.CreateDate,
                    IsDeleted = pt.IsDeleted,
                    FinalEnt = pt.FinalEnt,
                    BaCreatedTime = pt.BaCreatedTime,
                    BaGuid = pt.BaGuid
                })
                .ToListAsync(cancellationToken);
        }
    }



    public class UpdateProduceTypeValidator : AbstractValidator<UpdateProduceTypeCommand>
    {
        public UpdateProduceTypeValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("شناسه نوع تولید باید عددی مثبت باشد");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("عنوان نوع تولید الزامی است")
                .MinimumLength(2).WithMessage("عنوان نوع تولید باید حداقل 2 کاراکتر باشد")
                .MaximumLength(50).WithMessage("عنوان نوع تولید نباید بیشتر از 50 کاراکتر باشد");

            RuleFor(x => x.Code)
                .GreaterThan(0).WithMessage("کد باید عددی مثبت باشد")
                .When(x => x.Code.HasValue);
        }
    }
}