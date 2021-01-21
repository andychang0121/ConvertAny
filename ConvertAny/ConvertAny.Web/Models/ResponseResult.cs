namespace ConvertAny.Web.Models
{
    public class ResponseResult
    {
        public bool IsOk { get; set; }

        public string Message { get; set; }
    }

    public class ResponseResult<T>
    {
        public bool IsOk { get; set; }

        public T Data { get; set; }

        public string Message { get; set; }
    }
}
