namespace HireSphereApi.core.entities
{

    public class ProjectAnalysisResult
    {
        public string ProjectTitle { get; set; } = "";
        public string ProjectDescription { get; set; } = "";
        public int? RequiredExperience { get; set; }
        public string WorkPlace { get; set; } = "";
        public string ProgrammingLanguages { get; set; } = "";
        public string RemoteWorkAvailable { get; set; } = "";
        public string EnglishLevel { get; set; } = "";
    }

}
