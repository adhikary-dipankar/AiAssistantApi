using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using AiAssistantApi.Models;
using Microsoft.Extensions.Configuration;

namespace AiAssistantApi.Services;

public class AiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public AiService(IConfiguration configuration)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://generativelanguage.googleapis.com/")
        };

        _apiKey = configuration["GEMINI_API_KEY"] ?? throw new Exception("GEMINI_API_KEY not found in configuration.");
    }


    // 🔹 1. Generate MCQs
    public async Task<List<McqQuestion>?> GenerateMcqsAsync(string topic)
    {
        var prompt = $"Generate 10 MCQs in a JSON array format on the topic '{topic}'. " +
                     $"Each object should have: Topic, Text, Options (array), CorrectAnswer, Explanation. No markdown, no extra commentary.";
        return await GetAiResult<List<McqQuestion>>(prompt);
    }

    // 🔹 2. Get Recent News
    public async Task<List<NewsArticle>?> GetRecentNewsAsync(string topic)
    {
        var prompt = $"Give 10 recent news articles on '{topic}' in JSON array. " +
                     $"Each item must have: Title, Source, PublishedAt (date), Summary, and Url. No markdown.";
        return await GetAiResult<List<NewsArticle>>(prompt);
    }

    // 🔹 3. Get Roadmap
    public async Task<List<RoadmapStep>?> GetRoadmapAsync(string topic)
    {
        var prompt = $"Generate a learning roadmap for '{topic}' in JSON array. " +
                     $"Each object should include: Order (number), Title, Description, and ResourceLink. No commentary.";
        return await GetAiResult<List<RoadmapStep>>(prompt);
    }

    // 🔹 4. Get Stock High/Low Stats
    public async Task<List<StockStats>?> GetStockStatsAsync(string range)
    {
        var prompt = $"Give a JSON array of stock market summaries(20 items)for '{range}'. " +
                     $"Each object should include: Symbol, Highest, Lowest, and TimeRange. No markdown.";
        return await GetAiResult<List<StockStats>>(prompt);
    }

    // 🔧 Generic AI result processor
    private async Task<T?> GetAiResult<T>(string prompt)
{
    var requestBody = new
    {
        contents = new[]
        {
            new
            {
                parts = new[]
                {
                    new { text = prompt }
                }
            }
        }
    };

    var json = JsonConvert.SerializeObject(requestBody);
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    var response = await _httpClient.PostAsync(
        $"v1/models/gemini-1.5-flash:generateContent?key={_apiKey}",
        content
    );

    var responseString = await response.Content.ReadAsStringAsync();
    Console.WriteLine("🔹 Gemini Raw Response:");
    Console.WriteLine(responseString);

    var result = JsonConvert.DeserializeObject<GeminiResponse>(responseString);
    var reply = result?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

    if (string.IsNullOrWhiteSpace(reply)) return default;

    var cleaned = reply.Trim();

    // 🧹 Remove markdown formatting if present
    if (cleaned.StartsWith("```json"))
        cleaned = cleaned.Substring(7).Trim();
    if (cleaned.EndsWith("```"))
        cleaned = cleaned.Substring(0, cleaned.Length - 3).Trim();

    // ✂️ Truncate any extra text after the end of JSON array or object
    int endBracketIndex = cleaned.LastIndexOf(']');
    if (endBracketIndex != -1)
    {
        cleaned = cleaned.Substring(0, endBracketIndex + 1);
    }
    else
    {
        // Fallback: handle object responses
        int endBraceIndex = cleaned.LastIndexOf('}');
        if (endBraceIndex != -1)
            cleaned = cleaned.Substring(0, endBraceIndex + 1);
    }

    try
    {
        return JsonConvert.DeserializeObject<T>(cleaned);
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ JSON Parse Error: " + ex.Message);
        Console.WriteLine("🔹 Cleaned Content:\n" + cleaned);
        return default;
    }
}

    // 🧠 Gemini API Response Classes
    private class GeminiResponse
    {
        public List<Candidate>? Candidates { get; set; }
    }

    private class Candidate
    {
        public GeminiContent? Content { get; set; }
    }

    private class GeminiContent
    {
        public List<Part>? Parts { get; set; }
    }

    private class Part
    {
        public string? Text { get; set; }
    }
}
