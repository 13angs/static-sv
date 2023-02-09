namespace static_sv.Exceptions
{
    public class ErrorResponseException : Exception
    {
        public virtual int StatusCode { get; set; }
        public virtual string Description { get; set; } = "Bad request";
        public virtual IList<Error> Errors { get; set; }

        public ErrorResponseException(): base(){Errors=new List<Error>();}
        public ErrorResponseException(string message): base(message){Errors=new List<Error>();}
        public ErrorResponseException(string message, Exception innerException): base(message, innerException){Errors=new List<Error>();}
        public ErrorResponseException(int statusCode, string message, IList<Error> errors)
        {
            StatusCode=statusCode;
            Description=message;
            Errors=errors;
        }
    }
}