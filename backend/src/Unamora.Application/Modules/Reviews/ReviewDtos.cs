namespace Unamora.Application.Modules.Reviews.DTOs;

public class CreateReviewDto
{
    public Guid BookingId { get; set; }
    public Guid RevieweeId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public List<ReviewRatingDto> Ratings { get; set; } = new();
    public List<string>? AttachmentUrls { get; set; }
}

public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid ReviewerId { get; set; }
    public string ReviewerName { get; set; }
    public string? ReviewerProfilePicture { get; set; }
    public Guid RevieweeId { get; set; }
    public string RevieweeName { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal OverallRating { get; set; }
    public int Status { get; set; }
    public int SpamStatus { get; set; }
    public string? AISummary { get; set; }
    public bool IsVerifiedBooking { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public List<ReviewRatingDto>? Ratings { get; set; }
    public List<ReviewAttachmentDto>? Attachments { get; set; }
    public List<ReviewReplyDto>? Replies { get; set; }
    public int ReplyCount { get; set; }
}

public class ReviewRatingDto
{
    public int Category { get; set; }
    public int Score { get; set; }
    public string CategoryName { get; set; }
}

public class ReviewAttachmentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string FileUrl { get; set; }
    public string FileType { get; set; }
    public int DisplayOrder { get; set; }
}

public class ReviewReplyDto
{
    public Guid Id { get; set; }
    public Guid ReviewId { get; set; }
    public Guid ResponderId { get; set; }
    public string ResponderName { get; set; }
    public string? ResponderProfilePicture { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateReviewReplyDto
{
    public Guid ReviewId { get; set; }
    public string Content { get; set; }
}

public class ReviewsFilterDto
{
    public Guid? RevieweeId { get; set; }
    public int? MinRating { get; set; }
    public int? MaxRating { get; set; }
    public int? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class ReviewStatisticsDto
{
    public Guid UserId { get; set; }
    public int TotalReviews { get; set; }
    public decimal AverageRating { get; set; }
    public int RatingBreakdown_5Stars { get; set; }
    public int RatingBreakdown_4Stars { get; set; }
    public int RatingBreakdown_3Stars { get; set; }
    public int RatingBreakdown_2Stars { get; set; }
    public int RatingBreakdown_1Star { get; set; }
    public Dictionary<string, decimal> AverageCategoryRatings { get; set; }
}

public class FlagReviewDto
{
    public Guid ReviewId { get; set; }
    public string Reason { get; set; }
    public string? Description { get; set; }
}
