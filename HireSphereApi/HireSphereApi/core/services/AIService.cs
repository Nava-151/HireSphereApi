using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HireSphereApi.core.entities;

public class AIService
{
    private readonly HttpClient _httpClient;
    private readonly string _openAiApiKey;

    public AIService(IConfiguration config)
    {
        _httpClient = new HttpClient();
        _openAiApiKey = config["OPENAI_API_KEY"];
    }


    public async Task<AIResponse> AnalyzeResumeAsync(string resumeText)
    {
        var request = new
        {
            model = "gpt-4",
            messages = new[]
            {
                new { role = "system", content = "You are an AI that extracts resume data." },
                new { role = "user", content = $"Extract the following information: Experience, Education, Programming Languages, English Level.\n\n{resumeText}" }
            },
            temperature = 0.5
        };

        var requestBody = JsonSerializer.Serialize(request);
        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openAiApiKey}");
        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("AI request failed.");
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<AIResponse>(responseBody)!;
    }
}

