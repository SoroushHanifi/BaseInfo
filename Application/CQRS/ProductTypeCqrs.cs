using Application.Common;
using Application.OptionPatternModel;
using Application.Refits;
using Domain;
using Domain.Entities.Daroo;
using FluentValidation;
using Infrastructure;
using Infrastructure.Exceptions;
using Infrastructure.Utility;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.CQRS
{
    // ===== PRODUCTTYPE DTOs =====
    public class ProductTypeDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public long? MainTitleID { get; set; }
        public string MainTitleName { get; set; } = string.Empty;
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public bool? IsDeleted { get; set; }
        public int FinalEnt { get; set; }
        public long BaCreatedTime { get; set; }
        public Guid BaGuid { get; set; }

        public DateTime? BaCreatedDateTime => BaCreatedTime > 0
            ? new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(BaCreatedTime)
            : null;
    }

    public class ProductTypeSimpleDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    // ===== PRODUCTTYPE COMMANDS =====

    public record CreateProductTypeCommand(string Name, long? MainTitleID = null) : IRequest<long>;

    public class CreateProductTypeCommandHandler : IRequestHandler<CreateProductTypeCommand, long>
    {
        private readonly DarooDbContext _context;

        public CreateProductTypeCommandHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<long> Handle(CreateProductTypeCommand request, CancellationToken cancellationToken)
        {
            // بررسی وجود MainTitle اگر ارائه شده باشد
            if (request.MainTitleID.HasValue)
            {
                var mainTitleExists = await _context.MainTitles
                    .AnyAsync(mt => mt.Id == request.MainTitleID && mt.IsDeleted != true, cancellationToken);

                if (!mainTitleExists)
                    throw new AppException("عنوان اصلی مورد نظر یافت نشد");
            }

            // بررسی تکراری نبودن نام
            var nameExists = await _context.ProductTypes
                .AnyAsync(pt => pt.Name == request.Name && pt.IsDeleted != true, cancellationToken);

            if (nameExists)
                throw new AppException("نام نوع محصول تکراری است");

            var productType = new ProductType
            {
                Name = request.Name,
                MainTitleID = request.MainTitleID
            };

            productType.PrepareForCreation();

            _context.ProductTypes.Add(productType);
            await _context.SaveChangesAsync(cancellationToken);

            return productType.Id;
        }
    }

    public record UpdateProductTypeCommand(long Id, string Name, long? MainTitleID = null) : IRequest<bool>;

    public class UpdateProductTypeCommandHandler : IRequestHandler<UpdateProductTypeCommand, bool>
    {
        private readonly DarooDbContext _context;

        public UpdateProductTypeCommandHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateProductTypeCommand request, CancellationToken cancellationToken)
        {
            var productType = await _context.ProductTypes
                .FirstOrDefaultAsync(pt => pt.Id == request.Id && pt.IsDeleted != true, cancellationToken);

            if (productType == null)
                return false;

            // بررسی وجود MainTitle اگر ارائه شده باشد
            if (request.MainTitleID.HasValue)
            {
                var mainTitleExists = await _context.MainTitles
                    .AnyAsync(mt => mt.Id == request.MainTitleID && mt.IsDeleted != true, cancellationToken);

                if (!mainTitleExists)
                    throw new AppException("عنوان اصلی مورد نظر یافت نشد");
            }

            // بررسی تکراری نبودن نام (جز خودش)
            var nameExists = await _context.ProductTypes
                .AnyAsync(pt => pt.Name == request.Name &&
                               pt.Id != request.Id &&
                               pt.IsDeleted != true, cancellationToken);

            if (nameExists)
                throw new AppException("نام نوع محصول تکراری است");

            productType.Name = request.Name;
            productType.MainTitleID = request.MainTitleID;
            productType.PrepareForUpdate();

            _context.ProductTypes.Update(productType);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    public record DeleteProductTypeCommand(long Id) : IRequest<bool>;

    public class DeleteProductTypeCommandHandler : IRequestHandler<DeleteProductTypeCommand, bool>
    {
        private readonly DarooDbContext _context;

        public DeleteProductTypeCommandHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteProductTypeCommand request, CancellationToken cancellationToken)
        {
            var productType = await _context.ProductTypes
                .FirstOrDefaultAsync(pt => pt.Id == request.Id && pt.IsDeleted != true, cancellationToken);

            if (productType == null)
                return false;

            productType.SoftDelete();

            _context.ProductTypes.Update(productType);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    // ===== PRODUCTTYPE QUERIES =====

    public record GetAllProductTypesQuery : IRequest<List<ProductTypeDto>>;

    public class GetAllProductTypesQueryHandler : IRequestHandler<GetAllProductTypesQuery, List<ProductTypeDto>>
    {
        private readonly DarooDbContext _context;

        public GetAllProductTypesQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductTypeDto>> Handle(GetAllProductTypesQuery request, CancellationToken cancellationToken)
        {
            return await _context.ProductTypes
                .Include(pt => pt.MainTitle)
                .Where(pt => pt.IsDeleted != true)
                .OrderBy(pt => pt.Name)
                .Select(pt => new ProductTypeDto
                {
                    Id = pt.Id,
                    Name = pt.Name,
                    MainTitleID = pt.MainTitleID,
                    MainTitleName = pt.MainTitle != null ? pt.MainTitle.Name : "",
                    CreateDate = pt.CreateDate,
                    ModifyDate = pt.ModifyDate,
                    IsDeleted = pt.IsDeleted,
                    FinalEnt = pt.FinalEnt,
                    BaCreatedTime = pt.BaCreatedTime,
                    BaGuid = pt.BaGuid
                })
                .ToListAsync(cancellationToken);
        }
    }

    public record GetProductTypeByIdQuery(long Id) : IRequest<ProductTypeDto?>;

    public class GetProductTypeByIdQueryHandler : IRequestHandler<GetProductTypeByIdQuery, ProductTypeDto?>
    {
        private readonly DarooDbContext _context;

        public GetProductTypeByIdQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<ProductTypeDto?> Handle(GetProductTypeByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.ProductTypes
                .Include(pt => pt.MainTitle)
                .Where(pt => pt.Id == request.Id && pt.IsDeleted != true)
                .Select(pt => new ProductTypeDto
                {
                    Id = pt.Id,
                    Name = pt.Name,
                    MainTitleID = pt.MainTitleID,
                    MainTitleName = pt.MainTitle != null ? pt.MainTitle.Name : "",
                    CreateDate = pt.CreateDate,
                    ModifyDate = pt.ModifyDate,
                    IsDeleted = pt.IsDeleted,
                    FinalEnt = pt.FinalEnt,
                    BaCreatedTime = pt.BaCreatedTime,
                    BaGuid = pt.BaGuid
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }

    public record GetProductTypesByMainTitleQuery(long MainTitleId) : IRequest<List<ProductTypeSimpleDto>>;

    public class GetProductTypesByMainTitleQueryHandler : IRequestHandler<GetProductTypesByMainTitleQuery, List<ProductTypeSimpleDto>>
    {
        private readonly DarooDbContext _context;

        public GetProductTypesByMainTitleQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductTypeSimpleDto>> Handle(GetProductTypesByMainTitleQuery request, CancellationToken cancellationToken)
        {
            return await _context.ProductTypes
                .Where(pt => pt.MainTitleID == request.MainTitleId && pt.IsDeleted != true)
                .OrderBy(pt => pt.Name)
                .Select(pt => new ProductTypeSimpleDto
                {
                    Id = pt.Id,
                    Name = pt.Name
                })
                .ToListAsync(cancellationToken);
        }
    }

    public record SearchProductTypesQuery(string SearchTerm, long? MainTitleId = null) : IRequest<List<ProductTypeDto>>;

    public class SearchProductTypesQueryHandler : IRequestHandler<SearchProductTypesQuery, List<ProductTypeDto>>
    {
        private readonly DarooDbContext _context;

        public SearchProductTypesQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductTypeDto>> Handle(SearchProductTypesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.ProductTypes
                .Include(pt => pt.MainTitle)
                .Where(pt => pt.IsDeleted != true);

            // اعمال فیلتر جستجو
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(pt => pt.Name.Contains(request.SearchTerm));
            }

            // اعمال فیلتر عنوان اصلی
            if (request.MainTitleId.HasValue)
            {
                query = query.Where(pt => pt.MainTitleID == request.MainTitleId.Value);
            }

            return await query
                .OrderBy(pt => pt.Name)
                .Select(pt => new ProductTypeDto
                {
                    Id = pt.Id,
                    Name = pt.Name,
                    MainTitleID = pt.MainTitleID,
                    MainTitleName = pt.MainTitle != null ? pt.MainTitle.Name : "",
                    CreateDate = pt.CreateDate,
                    ModifyDate = pt.ModifyDate,
                    IsDeleted = pt.IsDeleted,
                    FinalEnt = pt.FinalEnt,
                    BaCreatedTime = pt.BaCreatedTime,
                    BaGuid = pt.BaGuid
                })
                .ToListAsync(cancellationToken);
        }
    }

    // ===== VALIDATORS =====
    public class CreateProductTypeValidator : AbstractValidator<CreateProductTypeCommand>
    {
        public CreateProductTypeValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("نام نوع محصول الزامی است")
                .MinimumLength(2).WithMessage("نام نوع محصول باید حداقل 2 کاراکتر باشد")
                .MaximumLength(50).WithMessage("نام نوع محصول نباید بیشتر از 50 کاراکتر باشد");

            RuleFor(x => x.MainTitleID)
                .GreaterThan(0).WithMessage("شناسه عنوان اصلی باید عددی مثبت باشد")
                .When(x => x.MainTitleID.HasValue);
        }
    }

    public class UpdateProductTypeValidator : AbstractValidator<UpdateProductTypeCommand>
    {
        public UpdateProductTypeValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("شناسه نوع محصول باید عددی مثبت باشد");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("نام نوع محصول الزامی است")
                .MinimumLength(2).WithMessage("نام نوع محصول باید حداقل 2 کاراکتر باشد")
                .MaximumLength(50).WithMessage("نام نوع محصول نباید بیشتر از 50 کاراکتر باشد");

            RuleFor(x => x.MainTitleID)
                .GreaterThan(0).WithMessage("شناسه عنوان اصلی باید عددی مثبت باشد")
                .When(x => x.MainTitleID.HasValue);
        }
    }
}