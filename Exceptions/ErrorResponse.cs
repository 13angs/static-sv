using Newtonsoft.Json;

namespace static_sv.Exceptions
{
    public class ErrorResponse
    {
        public ErrorResponse()
        {
            Errors = new List<Error>();
        }
        [JsonProperty("status_code")]
        public virtual int StatusCode { get; set; }

        [JsonProperty("message")]
        public virtual string? Message { get; set; }

        [JsonProperty("errors")]
        public virtual IList<Error> Errors { get; set; }
    }

    public class Error{

        [JsonProperty("message")]
        public virtual string? Message { get; set; }

        [JsonProperty("field")]
        public virtual string? Field { get; set; }
    }
}