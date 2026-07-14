using Unamora.Domain.Common;
using Unamora.Domain.Entities.Catalog;
using Unamora.Domain.Enums;

namespace Unamora.Domain.Entities.Profiles;

public class TradespersonSkill : BaseEntity
{
    public Guid TradespersonProfileId { get; set; }
    public TradespersonProfile TradespersonProfile { get; set; } = null!;
    public int SkillId { get; set; }
    public Skill Skill { get; set; } = null!;
    public ProficiencyLevel ProficiencyLevel { get; set; } = ProficiencyLevel.Intermediate;
    public int EndorsementCount { get; set; }
}
