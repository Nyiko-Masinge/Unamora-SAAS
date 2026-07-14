using MediatR;
using Unamora.Application.Common.Exceptions;
using Unamora.Application.Common.Interfaces;
using Unamora.Application.Modules.Portfolio.Commands;
using Unamora.Domain.Entities.Portfolio;

namespace Unamora.Application.Modules.Portfolio.Handlers;

public class CreatePortfolioProjectCommandHandler(
    IPortfolioRepository portfolioRepo, ITradespersonProfileRepository profileRepo, ICurrentUserService currentUser, IUnitOfWork uow)
    : IRequestHandler<CreatePortfolioProjectCommand, Guid>
{
    public async Task<Guid> Handle(CreatePortfolioProjectCommand request, CancellationToken cancellationToken)
    {
        var profile = await profileRepo.GetByIdAsync(request.TradespersonProfileId, cancellationToken)
            ?? throw new NotFoundException("Profile not found");
        if (profile.UserId != currentUser.UserId) throw new ForbiddenException("Access denied");

        var project = new PortfolioProject
        {
            TradespersonProfileId = profile.Id,
            Title = request.Title,
            Description = request.Description,
            ServiceCategoryId = request.ServiceCategoryId,
            ClientName = request.ClientName,
            CompletedDate = request.CompletedDate
        };
        await portfolioRepo.AddAsync(project, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        return project.Id;
    }
}

public class UploadPortfolioMediaCommandHandler(
    IPortfolioRepository portfolioRepo, IFileStorageService fileStorage, IUnitOfWork uow)
    : IRequestHandler<UploadPortfolioMediaCommand, Guid>
{
    public async Task<Guid> Handle(UploadPortfolioMediaCommand request, CancellationToken cancellationToken)
    {
        var project = await portfolioRepo.GetByIdAsync(request.ProjectId, cancellationToken)
            ?? throw new NotFoundException("Portfolio project not found");

        var container = request.MediaType == Domain.Enums.PortfolioMediaType.Video ? "portfolio-videos" : "portfolio-images";
        var blobUrl = await fileStorage.UploadAsync(request.FileStream, request.FileName, request.MimeType, container, cancellationToken);

        var media = new PortfolioMedia
        {
            PortfolioProjectId = project.Id,
            MediaType = request.MediaType,
            BlobUrl = blobUrl,
            FileName = request.FileName,
            MimeType = request.MimeType,
            FileSizeBytes = request.FileSizeBytes,
            Caption = request.Caption,
            DisplayOrder = project.Media.Count
        };
        project.Media.Add(media);
        portfolioRepo.Update(project);
        await uow.SaveChangesAsync(cancellationToken);
        return media.Id;
    }
}

public class GetPortfolioProjectsQueryHandler(IPortfolioRepository portfolioRepo) : IRequestHandler<GetPortfolioProjectsQuery, IReadOnlyList<PortfolioProjectDto>>
{
    public async Task<IReadOnlyList<PortfolioProjectDto>> Handle(GetPortfolioProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = await portfolioRepo.GetByProfileIdAsync(request.TradespersonProfileId, cancellationToken);
        return projects.Select(PortfolioMapper.MapProject).ToList();
    }
}

public class GetPortfolioProjectByIdQueryHandler(IPortfolioRepository portfolioRepo) : IRequestHandler<GetPortfolioProjectByIdQuery, PortfolioProjectDto?>
{
    public async Task<PortfolioProjectDto?> Handle(GetPortfolioProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await portfolioRepo.GetByIdAsync(request.ProjectId, cancellationToken);
        return project is null ? null : PortfolioMapper.MapProject(project);
    }
}

public class UpdatePortfolioProjectCommandHandler(IPortfolioRepository portfolioRepo, IUnitOfWork uow) : IRequestHandler<UpdatePortfolioProjectCommand, bool>
{
    public async Task<bool> Handle(UpdatePortfolioProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await portfolioRepo.GetByIdAsync(request.ProjectId, cancellationToken) ?? throw new NotFoundException("Project not found");
        project.Title = request.Title;
        project.Description = request.Description;
        project.ServiceCategoryId = request.ServiceCategoryId;
        project.ClientName = request.ClientName;
        project.CompletedDate = request.CompletedDate;
        project.IsFeatured = request.IsFeatured;
        portfolioRepo.Update(project);
        await uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public class DeletePortfolioProjectCommandHandler(IPortfolioRepository portfolioRepo, IUnitOfWork uow) : IRequestHandler<DeletePortfolioProjectCommand, bool>
{
    public async Task<bool> Handle(DeletePortfolioProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await portfolioRepo.GetByIdAsync(request.ProjectId, cancellationToken) ?? throw new NotFoundException("Project not found");
        portfolioRepo.Delete(project);
        await uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public class DeletePortfolioMediaCommandHandler(IPortfolioRepository portfolioRepo, IFileStorageService fileStorage, IUnitOfWork uow) : IRequestHandler<DeletePortfolioMediaCommand, bool>
{
    public async Task<bool> Handle(DeletePortfolioMediaCommand request, CancellationToken cancellationToken)
    {
        var media = await portfolioRepo.GetMediaByIdAsync(request.MediaId, cancellationToken);
        if (media is null) throw new NotFoundException("Media not found");
        await fileStorage.DeleteAsync(media.BlobUrl, cancellationToken);
        media.IsDeleted = true;
        await uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public static class PortfolioMapper
{
    public static PortfolioProjectDto MapProject(PortfolioProject p) => new()
    {
        Id = p.Id, Title = p.Title, Description = p.Description, ServiceCategoryId = p.ServiceCategoryId,
        CategoryName = p.ServiceCategory?.Name, ClientName = p.ClientName, CompletedDate = p.CompletedDate, IsFeatured = p.IsFeatured,
        Media = p.Media.Where(m => !m.IsDeleted).OrderBy(m => m.DisplayOrder).Select(m => new PortfolioMediaDto
        {
            Id = m.Id, MediaType = m.MediaType.ToString(), BlobUrl = m.BlobUrl, ThumbnailUrl = m.ThumbnailUrl,
            Caption = m.Caption, DisplayOrder = m.DisplayOrder
        }).ToList()
    };
}
