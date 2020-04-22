using System;
namespace TravelExpenses.Common.Models
{
    public class Response<T> where T : class
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public object Result { get; set; }
    }
}
