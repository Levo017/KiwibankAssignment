using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibMgmt.Services
{
    public class ServiceResult<T>
    {
        public T? Result { get; set; }
        public long ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public bool IsSuccess => this.Result != null;
    }
}
