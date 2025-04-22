using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HireSphereApi.core.entities;
using HireSphereApi.Data;
using HireSphereApi.Service.Iservice;
using Newtonsoft.Json;
using OpenAI;
using Sprache;
public class AIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _openAiApiKey;
    private readonly DataContext _context;

    public AIService(IConfiguration config, OpenAIClient openAI, DataContext context)
    {
        _httpClient = new HttpClient();
        _openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        _context = context;
    }
    public async Task<AIResponse> GetAIResponse(int aiId)
    {
        var response = await _context.AIResponses.FindAsync(aiId);
        return response != null ? response : null;
    }
    public async Task<AIResponse> AnalyzeResumeAsync(string resumeText)
    {
        var request = new
        {
            model = "gpt-4o-mini",
            messages = new[] {
            new { role = "system", content = "You are an AI that extracts resume data." },
            new { role = "user", content = $"Extract the following information: Experience calculate from the text and return me a" +
            $" number of years - an intger," +
            $" Education return me one of the following option College , University, or Another ," +
            $" Programming Languages return an array of languages he or she has ever been experienced , " +
            $"English Level- return the english level in one of the words as it sounds from the file Beginner, Intermediate, Advanced, Fluent.\n\n{resumeText}" }
        },
            temperature = 0.5
        };

        var requestBody = System.Text.Json.JsonSerializer.Serialize(request);
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

        Console.WriteLine("************** json");
        Console.WriteLine(messageContent);
        Console.WriteLine("***************");
        AIResponse devForFields = ParseAIResponse(messageContent);

        Console.WriteLine($"Experience: {devForFields.Experience}");
        Console.WriteLine();
        Console.WriteLine($"Education: {devForFields.Education}");
        Console.WriteLine();
        Console.WriteLine($"Languages: {devForFields.Languages}");
        Console.WriteLine();
        Console.WriteLine($"English Level: {devForFields.EnglishLevel}");


        return devForFields;
    }


    static AIResponse ParseAIResponse(string input)
    {
        AIResponse response = new AIResponse();
        //cjeck if there is need of it or return it to the old one
        response.Experience = int.Parse(ExtractField(input, "**Experience**")); 
        ExtractField(input,"**Experience**");
        response.Education = ExtractField(input, "**Education:**");
        response.Languages = ExtractField(input, "**Programming Languages:**");
        response.EnglishLevel = ExtractField(input, "**English Level:**");

        return response;
    }

    static string ExtractField(string input, string fieldName)
    {
        string pattern = Regex.Escape(fieldName) + @"\s*(.*?)\s*(?=\*\*|\Z)";
        Match match = Regex.Match(input, pattern, RegexOptions.Singleline);
        return match.Success ? match.Groups[1].Value.Trim() : null;
    }
    static int ExtractExperienceYears(string input)
    {
        string pattern = @"\((\d{4})-(\d{4})\)|\((\d{4})\)"; // תופס טווח שנים (YYYY-YYYY) או שנה בודדת (YYYY)
        MatchCollection matches = Regex.Matches(input, pattern);
        int totalYears = 0;

        foreach (Match match in matches)
        {
            if (match.Groups[1].Success && match.Groups[2].Success)
            {
                // טווח שנים, מחשבים את ההפרש
                int startYear = int.Parse(match.Groups[1].Value);
                int endYear = int.Parse(match.Groups[2].Value);
                totalYears += (endYear - startYear);
            }
            else if (match.Groups[3].Success)
            {
                // שנה בודדת (ניסיון של שנה אחת)
                totalYears += 1;
            }
        }

        return totalYears;
    }


}




