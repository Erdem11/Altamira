namespace Altamira.Api.Models
{
    public class ResponseBase<T>
    {
        public string Error { get; set; }
        public T Data { get; set; }
    }
}