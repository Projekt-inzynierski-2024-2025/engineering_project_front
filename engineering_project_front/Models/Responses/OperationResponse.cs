namespace engineering_project_front.Models.Responses
{
    public class OperationResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
    }
}
