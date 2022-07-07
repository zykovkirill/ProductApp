namespace ProductApp.Shared.Models
{
    public class BaseBuffer<T>
    {
        public string Id { get; set; }
        public T Entity { get; set; }
    }
}
