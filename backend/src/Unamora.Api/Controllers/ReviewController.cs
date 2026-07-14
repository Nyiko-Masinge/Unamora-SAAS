using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unamora.Application.Modules.Reviews.DTOs;
using Unamora.Application.Modules.Reviews.Services;

namespace Unamora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly ICurrentUserService _currentUserService;

    public ReviewController(IReviewService reviewService, ICurrentUserService currentUserService)
    {
        _reviewService = reviewService;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    public async Task<ActionResult<ReviewDto>> CreateReview([FromBody] CreateReviewDto dto)
    {
        var userId = _currentUserService.UserId;
        var review = await _reviewService.CreateReviewAsync(dto, userId);
        return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ReviewDto>> GetReview(Guid id)
    {
        var review = await _reviewService.GetReviewAsync(id);
        return Ok(review);
    }

    [HttpGet("user/{userId}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ReviewDto>>> GetUserReviews(Guid userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var filter = new ReviewsFilterDto { PageNumber = pageNumber, PageSize = pageSize };
        var reviews = await _reviewService.GetReviewsForUserAsync(userId, filter);
        return Ok(reviews);
    }

    [HttpGet("user/{userId}/statistics")]
    [AllowAnonymous]
    public async Task<ActionResult<ReviewStatisticsDto>> GetUserStatistics(Guid userId)
    {
        var statistics = await _reviewService.GetUserReviewStatisticsAsync(userId);
        return Ok(statistics);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ReviewDto>> UpdateReview(Guid id, [FromBody] CreateReviewDto dto)
    {
        var userId = _currentUserService.UserId;
        var review = await _reviewService.UpdateReviewAsync(id, dto, userId);
        return Ok(review);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(Guid id)
    {
        var userId = _currentUserService.UserId;
        await _reviewService.DeleteReviewAsync(id, userId);
        return NoContent();
    }

    [HttpPut("{id}/publish")]
    public async Task<IActionResult> PublishReview(Guid id)
    {
        await _reviewService.PublishReviewAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/flag")]
    public async Task<IActionResult> FlagReview(Guid id, [FromBody] FlagReviewDto dto)
    {
        var userId = _currentUserService.UserId;
        await _reviewService.FlagReviewAsync(dto, userId);
        return NoContent();
    }

    [HttpPost("{id}/replies")]
    public async Task<ActionResult<ReviewReplyDto>> CreateReply(Guid id, [FromBody] CreateReviewReplyDto dto)
    {
        var userId = _currentUserService.UserId;
        dto.ReviewId = id;
        var reply = await _reviewService.CreateReplyAsync(dto, userId);
        return CreatedAtAction(nameof(GetReview), new { id = id }, reply);
    }

    [HttpDelete("replies/{replyId}")]
    public async Task<IActionResult> DeleteReply(Guid replyId)
    {
        var userId = _currentUserService.UserId;
        await _reviewService.DeleteReplyAsync(replyId, userId);
        return NoContent();
    }

    [HttpPost("{id}/check-spam")]
    public async Task<IActionResult> CheckSpam(Guid id)
    {
        await _reviewService.CheckForSpamAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/generate-summary")]
    public async Task<ActionResult<string>> GenerateSummary(Guid id)
    {
        var summary = await _reviewService.GenerateAISummaryAsync(id);
        return Ok(new { summary });
    }

    [HttpGet("user/{userId}/helpful")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ReviewDto>>> GetHelpfulReviews(Guid userId, [FromQuery] int count = 5)
    {
        var reviews = await _reviewService.GetMostHelpfulReviewsAsync(userId, count);
        return Ok(reviews);
    }

    [HttpGet("recent")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ReviewDto>>> GetRecentReviews([FromQuery] int count = 10)
    {
        var reviews = await _reviewService.GetRecentReviewsAsync(count);
        return Ok(reviews);
    }

    [HttpPost("filter")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ReviewDto>>> FilterReviews([FromBody] ReviewsFilterDto filter)
    {
        var reviews = await _reviewService.GetReviewsForUserAsync(Guid.Empty, filter);
        return Ok(reviews);
    }
}
