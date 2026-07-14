namespace Unamora.Domain.Entities.Catalog;

public class ServiceCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconName { get; set; }
    public int? ParentCategoryId { get; set; }
    public ServiceCategory? ParentCategory { get; set; }
    public ICollection<ServiceCategory> SubCategories { get; set; } = new List<ServiceCategory>();
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    public ICollection<Trade> Trades { get; set; } = new List<Trade>();
}
