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
            new { role = "user", content = $"Extract the following information: Experience calculate from the text and return me a" +
            $" number of years - an intger don't give me word only a number of years" +
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
        Console.WriteLine(response.StatusCode+" *** content:*** "+ await response.Content.ReadAsStringAsync());

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


    //static AIResponse ParseAIResponse(string input)
    //{
    //    AIResponse response = new AIResponse();
    //    Console.WriteLine("in function begin");

    //    //cjeck if there is need of it or return it to the old one
    //    response.Experience = int.Parse(ExtractField(input, "Experience"));
    //    Console.WriteLine("exp "+response.Experience);
    //    response.Education = ExtractField(input, "Education");
    //    Console.WriteLine("re "+ response.Education);
    //    response.Languages = ExtractField(input, "Programming Languages");
    //    Console.WriteLine("lang "+response.Languages);
    //    response.EnglishLevel = ExtractField(input, "English Level");
    //    Console.WriteLine("englidh "+response.EnglishLevel);
    //    Console.WriteLine("in function end");
    //    return response;
    //}

    //static string ExtractField(string input, string fieldName)
    //{
    //    input = input.Trim().Trim('\'');

    //    // Adjust the regex pattern for extracting the field
    //    string pattern = Regex.Escape(fieldName) + @"\s*[:\-]?\s*(.*?)(?=\n|\Z)"; // Matching until end of line or string end
    //    Match match = Regex.Match(input, pattern, RegexOptions.Singleline);
    //    return match.Success ? match.Groups[1].Value.Trim() : null;
    //{
    //}
    //static AIResponse ParseAIResponse(string input)
    //{
    //    Console.WriteLine("in function begin");

    //    // אם יש גרשיים בודדים או סימני ``` JSON מיותרים – ננקה אותם
    //    input = input.Trim().Trim('`');

    //    // Deserialize ישיר ל-AIResponse
    //    var response = JsonConvert.DeserializeObject<AIResponse>(input);
    //    Console.WriteLine("exp " + response.Experience);
    //    //    response.Education = ExtractField(input, "Education");
    //        Console.WriteLine("re "+ response.Education);
    //    //    response.Languages = ExtractField(input, "Programming Languages");
    //    Console.WriteLine("lang "+response.Languages);
    //    //    response.EnglishLevel = ExtractField(input, "English Level");
    //        Console.WriteLine("englidh "+response.EnglishLevel);
    //    Console.WriteLine("in function end");
    //    return response;
    //}
    static AIResponse ParseAIResponse(string input)
    {
        Console.WriteLine("in function begin");

        // מסיר את כל ה-JSON המיותר (אם יש תוויים מיותרים לפני/אחר התשובה)
        input = CleanInput(input);

        try
        {
            // אם כל התוויים המיותרים הוסרו, אפשר לנסות לפרסר את JSON
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

            // הוספת הדפסות כדי לבדוק את כל השדות
            Console.WriteLine("Experience: " + response.Experience);
            Console.WriteLine("Education: " + response.Education);
            Console.WriteLine("Languages: " + response.Languages);
            Console.WriteLine("English Level: " + response.EnglishLevel);

            Console.WriteLine("in function end");
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

        // מוצא את המיקום של התו הראשון שהוא '{'
        int jsonStart = input.IndexOf('{');
        if (jsonStart >= 0)
        {
            input = input.Substring(jsonStart); // שומר רק מהנקודה הזאת והלאה
        }

        input = input.Replace("```", ""); // אם יש גבולות קוד מהעתקה
        input = input.Trim('"'); // מסיר גרשיים חיצוניים מיותרים אם קיימים

        return input;
    }





}




