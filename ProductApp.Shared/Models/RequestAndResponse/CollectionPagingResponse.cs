using System;
using System.Collections.Generic;

namespace ProductApp.Shared.Models
{

    public class CollectionResponse<T> : BaseAPIResponse
    {
        public CollectionResponse()
        {
            OperationDate = DateTime.Now;
        }

        public IEnumerable<T> Records { get; set; }
        public int Count { get; set; }
        public DateTime OperationDate { get; set; }
    }

    public class CollectionPagingResponse<T> : CollectionResponse<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int? NextPage { get; set; }

    }
}
