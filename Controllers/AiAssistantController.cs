using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AiAssistantApi.Models;
using AiAssistantApi.Services; 
namespace AiAssistantApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiAssistantController : ControllerBase
{
    private readonly AiService _aiService;

    public AiAssistantController(AiService aiService)
    {
        _aiService = aiService;
    }

    /// <summary>
    /// Generate 10 MCQ questions on a topic with answers and explanations.
    /// </summary>
    [HttpGet("generate-mcq")]
    public async Task<ActionResult<List<McqQuestion>>> GenerateMcq([FromQuery] string topic)
    {
        if (string.IsNullOrWhiteSpace(topic))
            return BadRequest("Topic is required.");

        var questions = await _aiService.GenerateMcqsAsync(topic);
        if (questions == null)
            return StatusCode(500, "AI failed to generate MCQs.");

        return Ok(questions);
    }

    /// <summary>
    /// Get structured recent news for a topic.
    /// </summary>
    [HttpGet("recent-news")]
    public async Task<ActionResult<List<NewsArticle>>> GetRecentNews([FromQuery] string topic)
    {
        if (string.IsNullOrWhiteSpace(topic))
            return BadRequest("Topic is required.");

        var news = await _aiService.GetRecentNewsAsync(topic);
        if (news == null)
            return StatusCode(500, "AI failed to generate news articles.");

        return Ok(news);
    }

    /// <summary>
    /// Generate a roadmap for learning a topic with resources and examples.
    /// </summary>
    [HttpGet("roadmap")]
    public async Task<ActionResult<List<RoadmapStep>>> GetRoadmap([FromQuery] string topic)
    {
        if (string.IsNullOrWhiteSpace(topic))
            return BadRequest("Topic is required.");

        var roadmap = await _aiService.GetRoadmapAsync(topic);
        if (roadmap == null)
            return StatusCode(500, "AI failed to generate roadmap.");

        return Ok(roadmap);
    }

    /// <summary>
    /// Get stock high/low trade records by time range (Today, Week, Month, Year).
    /// </summary>
    [HttpGet("stock-high-low")]
    public async Task<ActionResult<List<StockStats>>> GetStockStats([FromQuery] string range = "Today")
    {
        var stats = await _aiService.GetStockStatsAsync(range);
        if (stats == null)
            return StatusCode(500, "AI failed to generate stock data.");

        return Ok(stats);
    }
}
