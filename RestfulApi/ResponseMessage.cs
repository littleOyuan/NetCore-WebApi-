using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestfulApi
{
    public class ResponseMessage
    {
        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; }

        public object Data { get; set; }

        public ResponseMessage(object data)
        {
            IsSuccess = true;
            Data = data;
        }

        public ResponseMessage(bool isSuccess, string errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }
    }
}
