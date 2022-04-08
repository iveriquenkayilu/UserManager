namespace UserManagerService.Shared.Models
{
    public class ResponseModel
    {
        public string Message { get; set; }
        public bool Error { get; set; }

        public static ResponseModel<T> Fail<T>(string message, T data = default) => new(data, message, true);

        public static ResponseModel<T> Success<T>(string message, T data = default) => new(data, message, false);

        public static ResponseModel Fail(string message) => new(message, true);

        public static ResponseModel Success(string message) => new(message, false);

        public ResponseModel(string message, bool error)
        {
            Message = message;
            Error = error;
        }
    }

    public class ResponseModel<T> : ResponseModel
    {
        public T Data { get; set; }

        public ResponseModel(T data, string message, bool error) : base(message, error)
        {
            Data = data;
        }
    }
}
