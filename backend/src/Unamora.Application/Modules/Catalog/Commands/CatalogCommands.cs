using FluentValidation;
using MediatR;
using Unamora.Application.Common.Models;

namespace Unamora.Application.Modules.Catalog.Commands;

public record CreateCategoryCommand(string Name, string Slug, string? Description, string? IconName, int? ParentCategoryId, int DisplayOrder) : IRequest<int>;
public record UpdateCategoryCommand(int Id, string Name, string Slug, string? Description, string? IconName, int? ParentCategoryId, int DisplayOrder, bool IsActive) : IRequest<bool>;
public record DeleteCategoryCommand(int Id) : IRequest<bool>;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(100);
    }
}

public record CreateSkillCommand(int ServiceCategoryId, string Name, string? Description) : IRequest<int>;
public record UpdateSkillCommand(int Id, int ServiceCategoryId, string Name, string? Description, bool IsActive) : IRequest<bool>;
public record DeleteSkillCommand(int Id) : IRequest<bool>;

public record CreateTradeCommand(int ServiceCategoryId, string Name, string Slug, string? Description, int DisplayOrder) : IRequest<int>;
public record UpdateTradeCommand(int Id, int ServiceCategoryId, string Name, string Slug, string? Description, int DisplayOrder, bool IsActive) : IRequest<bool>;
public record DeleteTradeCommand(int Id) : IRequest<bool>;

public record GetCategoriesQuery(bool ActiveOnly = true) : IRequest<IReadOnlyList<CategoryDto>>;
public record GetCategoryByIdQuery(int Id) : IRequest<CategoryDto?>;
public record GetSkillsQuery(int? CategoryId) : IRequest<IReadOnlyList<SkillDto>>;
public record GetTradesQuery(bool ActiveOnly = true) : IRequest<IReadOnlyList<TradeDto>>;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconName { get; set; }
    public int? ParentCategoryId { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
}

public class SkillDto
{
    public int Id { get; set; }
    public int ServiceCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

public class TradeDto
{
    public int Id { get; set; }
    public int ServiceCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
}
