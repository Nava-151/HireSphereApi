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
        return response != null ? response : null;
    }
    public async Task<AIResponse> AnalyzeResumeAsync(string resumeText)
    {
        var request = new
        {
            model = "gpt-4o-mini",
            messages = new[] {
                new { role = "system", content = "You are an AI that extracts resume data." },
                new { role = "user", content = $"You are an expert resume analyzer. Your task is to extract structured information from resume text " +
                $"and return it in a very specific object format.\n\n" +
                $"Please analyze the following resume and return the extracted data in the following object structure:\n\n" +
                "public int? Experience { get; set; }\n" +
                "public string? Education { get; set; }\n" +
                "public string? Languages { get; set; }\n" +
                "public string? EnglishLevel { get; set; }\n\n" +

                "Instructions:\n\n" +

                "1. Experience – Total number of *professional* years of experience as an integer. " +
                "Round up. Do **not** include academic years such as university or college projects.\n\n" +

                "2. Education – One of the following values only:\n" +
                "- \"University\"\n" +
                "- \"College\"\n" +
                "- \"Other\"\n" +
                "If the resume mentions a university – use \"University\".\n" +
                "If it mentions a college – use \"College\".\n" +
                "If neither is present, return \"Other\".\n\n" +

                "3. Languages – List all programming languages mentioned in the resume " +
                "as a single comma-separated string. " +
                "Do not include frameworks or tools, only programming languages.\n\n" +

                "4. EnglishLevel – One of the following options:\n" +
                "- \"Beginner\"\n" +
                "- \"Intermediate\"\n" +
                "- \"Advanced\"\n" +
                "- \"Fluent\"\n" +
                "Base your assessment on how the resume is written (language, grammar) " +
                "and any mention of English proficiency.\n\n" +

                $"Use this resume:\n\n,{resumeText}\n\n" +
                "\n\nReturn the result as a JSON object with the same property names and format as shown above." }

            },
            temperature = 0.2
        };

        var requestBody = System.Text.Json.JsonSerializer.Serialize(request);
        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openAiApiKey}");

        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
        var responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(response.StatusCode + " *** content:*** " + await response.Content.ReadAsStringAsync());

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




