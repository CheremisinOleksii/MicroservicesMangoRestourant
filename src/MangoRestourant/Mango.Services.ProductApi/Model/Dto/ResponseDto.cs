using System.Collections.Generic;

namespace Mango.Services.ProductApi.Model.Dto
{
    public class ResponseDto
    {
        public bool IsSuccess { get; set; }
        public object Result { get; set; }
        public string DisplayMessage { get; set; }
        public List<string> ErrorMessages { get; set; }

        public ResponseDto()
        {
            IsSuccess = true;
        }
    }
}
