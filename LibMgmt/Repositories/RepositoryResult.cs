namespace LibMgmt.Repositories
{
    public class RepositoryResult<T>
    {
        public T? Result { get; set; }
        public RepositoryErrors? Error { get; set; }
        public string? Message { get; set; }

        public static RepositoryResult<T> CreateRepositoryResult<T>(T? result, RepositoryErrors? error = null, string? message = null)
        {
            return new RepositoryResult<T>()
            {
                Result = result,
                Error = error,
                Message = message
            };
        }
    }
}
