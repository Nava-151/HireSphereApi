using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HireSphereApi.core.entities;
using OpenAI;
public class AIService
{
    private readonly HttpClient _httpClient;
    private readonly string _openAiApiKey;

    public AIService(IConfiguration config, OpenAIClient openAI)
    {
        _httpClient = new HttpClient();
        _openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        Console.WriteLine("api key: " + _openAiApiKey);
    }


    //public async Task<AIResponse> AnalyzeResumeAsync(string resumeText)
    //{
    //    var request = new
    //    {
    //        model = "gpt-4o-mini",
    //        messages = new[]
    //        {
    //            new { role = "system", content = "You are an AI that extracts resume data." },
    //            new { role = "user", content = $"Extract the following information: Experience, Education, Programming Languages, English Level.\n\n{resumeText}" }
    //        },
    //        temperature = 0.5
    //    };

    //    var requestBody = JsonSerializer.Serialize(request);
    //    var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

    //    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openAiApiKey}");
    //    //_httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");

    //    var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
    //    var responseBody = await response.Content.ReadAsStringAsync();

    //    Console.WriteLine($"HTTP Status Code: {response.StatusCode}");
    //    Console.WriteLine($"Response Body: {responseBody}");
    //    if (!response.IsSuccessStatusCode)
    //        throw new Exception("AI request failed.");


    //    //var responseBody = await response.Content.ReadAsStringAsync();
    //    //return JsonSerializer.Deserialize<AIResponse>(responseBody.choices[0].masssege.content)!;
    //    return JsonSerializer.Deserialize<AIResponse>(responseBody)!;

    //}
    //public async Task<AIResponse> AnalyzeResumeAsync(string resumeText)
    //{
    //    var request = new
    //    {
    //        model = "gpt-4o",
    //        messages = new[]
    //        {
    //        new { role = "system", content = "You are an AI that extracts resume data and outputs JSON." },
    //        new { role = "user", content = $"Extract the following information in JSON format: Experience, Education, Programming Languages, English Level.\n\n{resumeText}" }
    //    },
    //        response_format = new { type = "json_object" }, // ✔ תיקון כאן!
    //        temperature = 0.5
    //    };

    //    var requestBody = JsonSerializer.Serialize(request);
    //    var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

    //    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _openAiApiKey);

    //    var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
    //    var responseBody = await response.Content.ReadAsStringAsync();

    //    Console.WriteLine($"Response Body: {responseBody}");

    //    if (!response.IsSuccessStatusCode)
    //        throw new Exception("AI request failed.");

    //    // פענוח JSON ישירות כי פורמט "json_object" מחזיר JSON נקי
    //    var aiResponse = JsonSerializer.Deserialize<AIResponse>(responseBody);

    //    if (aiResponse == null)
    //        throw new Exception("Failed to deserialize AI response.");

    //    return aiResponse;
    //}
    public async Task<AIResponse> AnalyzeResumeAsync(string resumeText)
    {
        var request = new
        {
            model = "gpt-4o-mini",
            messages = new[] {
            new { role = "system", content = "You are an AI that extracts resume data." },
            new { role = "user", content = $"Extract the following information: Experience, Education, Programming Languages, English Level.\n\n{resumeText}" }
        },
            temperature = 0.5
        };

        var requestBody = JsonSerializer.Serialize(request);
        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openAiApiKey}");

        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception("AI request failed.");

        // Parse response to extract JSON content
        using var document = JsonDocument.Parse(responseBody);
        var messageContent = document.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        if (string.IsNullOrEmpty(messageContent))
            throw new Exception("AI response is empty.");

        Console.WriteLine("**************");
        Console.WriteLine( messageContent);
        Console.WriteLine("***************");

        // Clean up content to fit your fields
        var aiResponse = new AIResponse();

        // Extracting experience, education, languages, and English level from the AI's response
        var experienceMatch = Regex.Match(messageContent, @"Experience[\s\S]*?(\[.*\])", RegexOptions.IgnoreCase);
        if (experienceMatch.Success)
        {
            aiResponse.Experience = experienceMatch.Groups[1].Value.Length; // Example: storing length of experience JSON
            Console.WriteLine("-*-*-*-*-*--*-*-*-**-*- experience");
            Console.WriteLine(aiResponse.Experience);
        }

        var educationMatch = Regex.Match(messageContent, @"Education[\s\S]*?(\[.*\])", RegexOptions.IgnoreCase);
        if (educationMatch.Success)
        {
            Console.WriteLine("-*-*-*-*-*--*-*-*-**-*- educatuin");
            Console.WriteLine(aiResponse.Education);

            //aiResponse.Education = educationMatch.Groups[1].Value; // Store education details
        }

        var languagesMatch = Regex.Match(messageContent, @"Programming_Languages[\s\S]*?(\[.*\])", RegexOptions.IgnoreCase);
        if (languagesMatch.Success)
        {
            aiResponse.Languages = languagesMatch.Groups[1].Value; // Store languages as a string
        }

        var englishLevelMatch = Regex.Match(messageContent, @"English_Level[\s\S]*?(\w+)", RegexOptions.IgnoreCase);
        if (englishLevelMatch.Success)
        {
            aiResponse.EnglishLevel = englishLevelMatch.Groups[1].Value; // Store English level
        }

        return aiResponse;
    }


}

