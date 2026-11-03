namespace Application.Common.Models
{
    public class Result
    {
        internal Result(bool succeeded, IEnumerable<string> errors)
        {
            Succeeded = succeeded;
            Errors = errors.ToArray();
        }

        public bool Succeeded { get; set; }
        public string[] Errors { get; set; }

        public static Result Success()
        {
            return new Result(true, Array.Empty<string>());
        }

        public static Result Failure(IEnumerable<string> errors)
        {
            return new Result(false, errors);
        }

        public static Result Failure(string error)
        {
            return new Result(false, new[] { error });
        }
    }

    public class Result<T> : Result
    {
        public T Data { get; set; }

        private Result(bool succeeded, IEnumerable<string> errors, T data = default) : base(succeeded, errors)
        {
            Data = data;
        }

        public static Result<T> Success(T data)
        {
            return new Result<T>(true, Array.Empty<string>(), data);
        }

        public new static Result<T> Failure(IEnumerable<string> errors)
        {
            return new Result<T>(false, errors);
        }

        public static Result<T> Failure(string error)
        {
            return new Result<T>(false, new[] { error });
        }
    }
}