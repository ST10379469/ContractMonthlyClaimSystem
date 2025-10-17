namespace ContractMonthlyClaimSystem.Exceptions
{
    public class CustomUnauthorizedException : Exception
    {
        public CustomUnauthorizedException(string message) : base(message) { }

        public CustomUnauthorizedException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    public class ClaimValidationException : Exception
    {
        public Dictionary<string, string[]> Errors { get; }

        public ClaimValidationException(string message) : base(message)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ClaimValidationException(string message, Dictionary<string, string[]> errors) : base(message)
        {
            Errors = errors;
        }
    }

    public class FileValidationException : Exception
    {
        public FileValidationException(string message) : base(message) { }

        public FileValidationException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}