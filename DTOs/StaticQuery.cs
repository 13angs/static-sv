using Microsoft.AspNetCore.Mvc;

namespace static_sv.DTOs
{
    // [BindProperties]
    public class StaticQuery
    {
        [BindProperty]
        public string? query { get; set; }
    }

    public class StaticQueryStore
    {
        public static string All = "all";
    }
}