using HireSphereApi.core.entities;
using HireSphereApi.entities;

namespace HireSphereApi.core.Services
{
    public class AIService
    {
        private readonly HttpClient _httpClient;

        public AIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //    public async Task<ProjectAnalysisResult> ParseProjectDescription(string description)
        //    {
        //        // שולח את המחרוזת ל-AI (כאן אתה יכול להשתמש בשירות חיצוני כמו GPT-3 או מודל אחר)
        //        var response = await _httpClient.PostAsJsonAsync("https://api.ai-service.com/parse", new { text = description });

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            return null;
        //        }

        //        var aiResponse = await response.Content.ReadAsStringAsync();

        //        // מחלץ את הערכים המתאימים מתוך התשובה
        //        //return new ProjectAnalysisResult
        //        //{
        //        //    ProjectTitle = aiResponse.Title,
        //        //    ProjectDescription = aiResponse.Description,
        //        //    RequiredExperience = aiResponse.Experience,
        //        //    WorkPlace = aiResponse.WorkPlace,
        //        //    ProgrammingLanguages = aiResponse.Languages,
        //        //    RemoteWorkAvailable = aiResponse.RemoteWork ? "כן" : "לא",
        //        //    EnglishLevel = aiResponse.EnglishLevel
        //        //};
        //    }
        //}
    }

}
