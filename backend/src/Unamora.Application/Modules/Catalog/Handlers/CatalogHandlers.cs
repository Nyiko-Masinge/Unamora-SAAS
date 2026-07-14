using MediatR;
using Unamora.Application.Common.Exceptions;
using Unamora.Application.Common.Interfaces;
using Unamora.Application.Modules.Catalog.Commands;
using Unamora.Domain.Entities.Catalog;

namespace Unamora.Application.Modules.Catalog.Handlers;

public class CreateCategoryCommandHandler(IServiceCategoryRepository repo, IUnitOfWork uow) : IRequestHandler<CreateCategoryCommand, int>
{
    public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = new ServiceCategory
        {
            Name = request.Name,
            Slug = request.Slug,
            Description = request.Description,
            IconName = request.IconName,
            ParentCategoryId = request.ParentCategoryId,
            DisplayOrder = request.DisplayOrder
        };
        await repo.AddAsync(entity, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}

public class GetCategoriesQueryHandler(IServiceCategoryRepository repo) : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    public async Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var items = await repo.GetAllActiveAsync(cancellationToken);
        return items.Where(c => !request.ActiveOnly || c.IsActive).Select(c => new CategoryDto
        {
            Id = c.Id, Name = c.Name, Slug = c.Slug, Description = c.Description,
            IconName = c.IconName, ParentCategoryId = c.ParentCategoryId, IsActive = c.IsActive, DisplayOrder = c.DisplayOrder
        }).ToList();
    }
}

public class GetCategoryByIdQueryHandler(IServiceCategoryRepository repo) : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
{
    public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var c = await repo.GetByIdAsync(request.Id, cancellationToken);
        if (c is null) return null;
        return new CategoryDto { Id = c.Id, Name = c.Name, Slug = c.Slug, Description = c.Description, IconName = c.IconName, ParentCategoryId = c.ParentCategoryId, IsActive = c.IsActive, DisplayOrder = c.DisplayOrder };
    }
}

public class UpdateCategoryCommandHandler(IServiceCategoryRepository repo, IUnitOfWork uow) : IRequestHandler<UpdateCategoryCommand, bool>
{
    public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await repo.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException("Category not found");
        entity.Name = request.Name; entity.Slug = request.Slug; entity.Description = request.Description;
        entity.IconName = request.IconName; entity.ParentCategoryId = request.ParentCategoryId;
        entity.DisplayOrder = request.DisplayOrder; entity.IsActive = request.IsActive;
        repo.Update(entity);
        await uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public class DeleteCategoryCommandHandler(IServiceCategoryRepository repo, IUnitOfWork uow) : IRequestHandler<DeleteCategoryCommand, bool>
{
    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await repo.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException("Category not found");
        repo.Delete(entity);
        await uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public class CreateSkillCommandHandler(ISkillRepository repo, IUnitOfWork uow) : IRequestHandler<CreateSkillCommand, int>
{
    public async Task<int> Handle(CreateSkillCommand request, CancellationToken cancellationToken)
    {
        var entity = new Skill { ServiceCategoryId = request.ServiceCategoryId, Name = request.Name, Description = request.Description };
        await repo.AddAsync(entity, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}

public class GetSkillsQueryHandler(ISkillRepository repo) : IRequestHandler<GetSkillsQuery, IReadOnlyList<SkillDto>>
{
    public async Task<IReadOnlyList<SkillDto>> Handle(GetSkillsQuery request, CancellationToken cancellationToken)
    {
        var items = request.CategoryId.HasValue
            ? await repo.GetByCategoryIdAsync(request.CategoryId.Value, cancellationToken)
            : await repo.GetAllActiveAsync(cancellationToken);
        return items.Select(s => new SkillDto { Id = s.Id, ServiceCategoryId = s.ServiceCategoryId, Name = s.Name, Description = s.Description, IsActive = s.IsActive }).ToList();
    }
}

public class CreateTradeCommandHandler(ITradeRepository repo, IUnitOfWork uow) : IRequestHandler<CreateTradeCommand, int>
{
    public async Task<int> Handle(CreateTradeCommand request, CancellationToken cancellationToken)
    {
        var entity = new Trade { ServiceCategoryId = request.ServiceCategoryId, Name = request.Name, Slug = request.Slug, Description = request.Description, DisplayOrder = request.DisplayOrder };
        await repo.AddAsync(entity, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}

public class GetTradesQueryHandler(ITradeRepository repo) : IRequestHandler<GetTradesQuery, IReadOnlyList<TradeDto>>
{
    public async Task<IReadOnlyList<TradeDto>> Handle(GetTradesQuery request, CancellationToken cancellationToken)
    {
        var items = await repo.GetAllActiveAsync(cancellationToken);
        return items.Where(t => !request.ActiveOnly || t.IsActive).Select(t => new TradeDto { Id = t.Id, ServiceCategoryId = t.ServiceCategoryId, Name = t.Name, Slug = t.Slug, Description = t.Description, IsActive = t.IsActive, DisplayOrder = t.DisplayOrder }).ToList();
    }
}
