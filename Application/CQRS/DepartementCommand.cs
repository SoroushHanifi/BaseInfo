using Domain.Entities.Daroo;
using Infrastructure;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS
{
    public record CreateDepartmentCommand(string Name) : IRequest<Guid>;

    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Guid>
    {
        private readonly DarooDbContext _context;

        public CreateDepartmentCommandHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var department = new Department
            {
                Name = request.Name,
                CreateDate = DateTime.UtcNow
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync(cancellationToken);

            return department.Id;
        }
    }

    // Update Command
    public record UpdateDepartmentCommand(int Id, string Name) : IRequest<bool>;

    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, bool>
    {
        private readonly DarooDbContext _context;

        public UpdateDepartmentCommandHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var department = await _context.Departments.FindAsync(request.Id, cancellationToken);

            if (department == null)
                return false;

            department.Name = request.Name;
            department.ModifyDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    // Delete Command
    public record DeleteDepartmentCommand(int Id) : IRequest<bool>;

    public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, bool>
    {
        private readonly DarooDbContext _context;

        public DeleteDepartmentCommandHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            var department = await _context.Departments.FindAsync(request.Id, cancellationToken);

            if (department == null)
                return false;

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
