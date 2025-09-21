using Application.Common;
using Domain.Entities.Daroo;
using FluentValidation;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS
{
    // ===== DEPARTMENT DTOs =====
    public class DepartmentDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? CreateUserID { get; set; }
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
    // ===== DEPARTMENT QUERIES =====

    public record GetAllDepartmentsQuery : IRequest<List<DepartmentDto>>;

    public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, List<DepartmentDto>>
    {
        private readonly DarooDbContext _context;

        public GetAllDepartmentsQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<List<DepartmentDto>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Departments
                .Where(d => d.IsDeleted != true)
                .OrderBy(d => d.Name)
                .Select(d => new DepartmentDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    CreateUserID = d.CreateUserID,
                    CreateDate = d.CreateDate,
                    ModifyDate = d.ModifyDate,
                    IsDeleted = d.IsDeleted,
                    FinalEnt = d.FinalEnt,
                    BaCreatedTime = d.BaCreatedTime,
                    BaGuid = d.BaGuid
                })
                .ToListAsync(cancellationToken);
        }
    }

    public record GetDepartmentByIdQuery(long Id) : IRequest<DepartmentDto?>;

    public class GetDepartmentByIdQueryHandler : IRequestHandler<GetDepartmentByIdQuery, DepartmentDto?>
    {
        private readonly DarooDbContext _context;

        public GetDepartmentByIdQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<DepartmentDto?> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Departments
                .Where(d => d.Id == request.Id && d.IsDeleted != true)
                .Select(d => new DepartmentDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    CreateUserID = d.CreateUserID,
                    CreateDate = d.CreateDate,
                    ModifyDate = d.ModifyDate,
                    IsDeleted = d.IsDeleted,
                    FinalEnt = d.FinalEnt,
                    BaCreatedTime = d.BaCreatedTime,
                    BaGuid = d.BaGuid
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }

    public record GetActiveDepartmentsQuery : IRequest<List<DepartmentDto>>;

    public class GetActiveDepartmentsQueryHandler : IRequestHandler<GetActiveDepartmentsQuery, List<DepartmentDto>>
    {
        private readonly DarooDbContext _context;

        public GetActiveDepartmentsQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<List<DepartmentDto>> Handle(GetActiveDepartmentsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Departments
                .Where(d => d.IsDeleted != true)
                .OrderBy(d => d.Name)
                .Select(d => new DepartmentDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    CreateUserID = d.CreateUserID,
                    CreateDate = d.CreateDate,
                    ModifyDate = d.ModifyDate,
                    IsDeleted = d.IsDeleted,
                    FinalEnt = d.FinalEnt,
                    BaCreatedTime = d.BaCreatedTime,
                    BaGuid = d.BaGuid
                })
                .ToListAsync(cancellationToken);
        }
    }

    public record GetAllDepartmentsPaginationQuery(int PageIndex = 1, int PageSize = 10) : IRequest<PagedData<DepartmentDto>>;

    public class GetAllDepartmentsPaginationQueryHandler : IRequestHandler<GetAllDepartmentsPaginationQuery, PagedData<DepartmentDto>>
    {
        private readonly DarooDbContext _context;

        public GetAllDepartmentsPaginationQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<PagedData<DepartmentDto>> Handle(GetAllDepartmentsPaginationQuery request, CancellationToken cancellationToken)
        {
            int pageIndex = request.PageIndex < 1 ? 1 : request.PageIndex;
            int pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            int totalCount = await _context.Departments
                .Where(d => d.IsDeleted != true)
                .CountAsync(cancellationToken);

            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await _context.Departments
                .Where(d => d.IsDeleted != true)
                .OrderBy(d => d.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DepartmentDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    CreateUserID = d.CreateUserID,
                    CreateDate = d.CreateDate,
                    ModifyDate = d.ModifyDate,
                    IsDeleted = d.IsDeleted,
                    FinalEnt = d.FinalEnt,
                    BaCreatedTime = d.BaCreatedTime,
                    BaGuid = d.BaGuid
                })
                .ToListAsync(cancellationToken);

            return new PagedData<DepartmentDto>
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

    public record SearchDepartmentsQuery(string SearchTerm) : IRequest<List<DepartmentDto>>;

    public class SearchDepartmentsQueryHandler : IRequestHandler<SearchDepartmentsQuery, List<DepartmentDto>>
    {
        private readonly DarooDbContext _context;

        public SearchDepartmentsQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<List<DepartmentDto>> Handle(SearchDepartmentsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Departments
                .Where(d => d.IsDeleted != true);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(d => d.Name.Contains(request.SearchTerm));
            }

            return await query
                .OrderBy(d => d.Name)
                .Select(d => new DepartmentDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    CreateUserID = d.CreateUserID,
                    CreateDate = d.CreateDate,
                    ModifyDate = d.ModifyDate,
                    IsDeleted = d.IsDeleted,
                    FinalEnt = d.FinalEnt,
                    BaCreatedTime = d.BaCreatedTime,
                    BaGuid = d.BaGuid
                })
                .ToListAsync(cancellationToken);
        }
    }

    

    public class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentCommand>
    {
        public UpdateDepartmentValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("شناسه اداره کل باید عددی مثبت باشد");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("نام اداره کل الزامی است")
                .MinimumLength(2).WithMessage("نام اداره کل باید حداقل 2 کاراکتر باشد")
                .MaximumLength(50).WithMessage("نام اداره کل نباید بیشتر از 50 کاراکتر باشد");
        }
    }
}