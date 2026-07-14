using FluentValidation;
using MediatR;
using Unamora.Domain.Enums;

namespace Unamora.Application.Modules.Portfolio.Commands;

public record CreatePortfolioProjectCommand(
    Guid TradespersonProfileId,
    string Title,
    string? Description,
    int? ServiceCategoryId,
    string? ClientName,
    DateTime? CompletedDate) : IRequest<Guid>;

public record UpdatePortfolioProjectCommand(
    Guid ProjectId,
    string Title,
    string? Description,
    int? ServiceCategoryId,
    string? ClientName,
    DateTime? CompletedDate,
    bool IsFeatured) : IRequest<bool>;

public record DeletePortfolioProjectCommand(Guid ProjectId) : IRequest<bool>;

public record UploadPortfolioMediaCommand(
    Guid ProjectId,
    PortfolioMediaType MediaType,
    string FileName,
    string MimeType,
    long FileSizeBytes,
    Stream FileStream,
    string? Caption) : IRequest<Guid>;

public record DeletePortfolioMediaCommand(Guid MediaId) : IRequest<bool>;

public record GetPortfolioProjectsQuery(Guid TradespersonProfileId) : IRequest<IReadOnlyList<PortfolioProjectDto>>;
public record GetPortfolioProjectByIdQuery(Guid ProjectId) : IRequest<PortfolioProjectDto?>;

public class PortfolioProjectDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ServiceCategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? ClientName { get; set; }
    public DateTime? CompletedDate { get; set; }
    public bool IsFeatured { get; set; }
    public IReadOnlyList<PortfolioMediaDto> Media { get; set; } = Array.Empty<PortfolioMediaDto>();
}

public class PortfolioMediaDto
{
    public Guid Id { get; set; }
    public string MediaType { get; set; } = string.Empty;
    public string BlobUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
}

public class CreatePortfolioProjectCommandValidator : AbstractValidator<CreatePortfolioProjectCommand>
{
    public CreatePortfolioProjectCommandValidator()
    {
        RuleFor(x => x.TradespersonProfileId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
    }
}
