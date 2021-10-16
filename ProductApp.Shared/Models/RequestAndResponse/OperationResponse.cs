using System;

namespace ProductApp.Shared.Models
{
    public class OperationResponse<T> : BaseAPIResponse
    {

        public OperationResponse()
        {
            OperationDate = DateTime.UtcNow;
        }

        public T Record { get; set; }
        public DateTime OperationDate { get; set; }
    }
}
