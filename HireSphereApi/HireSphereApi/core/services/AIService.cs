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
using Newtonsoft.Json.Linq;
using OpenAI;
using Sprache;
using JsonException = System.Text.Json.JsonException;

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
        return response != null? response : null;
    }
    public async Task<AIResponse> AnalyzeResumeAsync(string resumeText)
    {
        var request = new
        {
            model = "gpt-4o-mini",
            messages = new[] {
            new { role = "system", content = "You are an AI that extracts resume data." },
            new { role = "user", content = $"Extract the following information: Experience calculate from the text and return me a" +
            $" number of years - an intger don't give me word only a number of years" +
            $" Education return me one of the following option College , University who learned in univarsty or acadmay, or Another it it  not one of the other options ," +
            $" Programming Languages return an array of languages he or she has ever been experienced , " +
            $"English Level- return the english level in one of the words as it sounds from the file:  Beginner, Intermediate, Advanced, Fluent.\n\n{resumeText}" }
        },
            temperature = 0.5
        };

        var requestBody = System.Text.Json.JsonSerializer.Serialize(request);
        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openAiApiKey}");

        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
        var responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(response.StatusCode+" *** content:*** "+ await response.Content.ReadAsStringAsync());

        if (!response.IsSuccessStatusCode)
            throw new Exception("AI request failed.");

        using var document = JsonDocument.Parse(responseBody);
        var messageContent = document.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        if (string.IsNullOrEmpty(messageContent))
            throw new Exception("AI response is empty.");

        AIResponse devForFields = ParseAIResponse(messageContent);

        return devForFields;
    }

    static AIResponse ParseAIResponse(string input)
    {
        input = CleanInput(input);

        try
        {
            var jObj = JObject.Parse(input);

            var response = new AIResponse
            {
                Experience = jObj["Experience"]?.ToObject<int>(),
                Education = jObj["Education"]?.ToString(),
                Languages = jObj["Programming Languages"] != null
                    ? string.Join(", ", jObj["Programming Languages"].ToObject<List<string>>())
                    : null,
                EnglishLevel = jObj["English Level"]?.ToString()
            };
            return response;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error parsing JSON: {ex.Message}");
            throw;
        }
    }


    static string CleanInput(string input)
    {
        input = input.Trim();

        int jsonStart = input.IndexOf('{');
        if (jsonStart >= 0)
        {
            input = input.Substring(jsonStart); 
        }

        input = input.Replace("```", ""); 
        input = input.Trim('"'); 

        return input;
    }





}




