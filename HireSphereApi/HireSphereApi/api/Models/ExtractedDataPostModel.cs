﻿namespace HireSphereApi.api.Models
{
    public class ExtractedDataPostModel
    {
        public string Links { get; set; }
        public int CandidateId { get; set; }

        public string? Technologies { get; set; }
        public decimal Experience { get; set; }
        public string Education { get; set; }
        public string PreviousWorkplaces { get; set; }
        public string ProgrammingLanguages { get; set; }
    }
}
