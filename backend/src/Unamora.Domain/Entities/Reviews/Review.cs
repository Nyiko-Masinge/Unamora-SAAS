using Unamora.Domain.Common;
using Unamora.Domain.Enums;

namespace Unamora.Domain.Entities.Reviews;

public class Review : BaseEntity
{
    public Guid BookingId { get; set; }
    public Guid ReviewerId { get; set; }
    public Guid RevieweeId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal OverallRating { get; set; }
    public ReviewStatus Status { get; set; } = ReviewStatus.Pending;
    public SpamDetectionStatus SpamStatus { get; set; } = SpamDetectionStatus.Approved;
    public string? AISummary { get; set; }
    public bool IsVerifiedBooking { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public ICollection<ReviewRating> Ratings { get; set; } = new List<ReviewRating>();
    public ICollection<ReviewAttachment> Attachments { get; set; } = new List<ReviewAttachment>();
    public ICollection<ReviewReply> Replies { get; set; } = new List<ReviewReply>();
}

public class ReviewRating : BaseEntity
{
    public Guid ReviewId { get; set; }
    public RatingCategory Category { get; set; }
    public int Score { get; set; } // 1-5
    
    public Review Review { get; set; }
}

public class ReviewAttachment : BaseEntity
{
    public Guid ReviewId { get; set; }
    public string FileName { get; set; }
    public string FileUrl { get; set; }
    public string FileType { get; set; }
    public int DisplayOrder { get; set; }
    
    public Review Review { get; set; }
}

public class ReviewReply : BaseEntity
{
    public Guid ReviewId { get; set; }
    public Guid ResponderId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public Review Review { get; set; }
}
