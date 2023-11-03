using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos
{
    public class ListRequestDto<T> where T : class
    {
        public List<T> Data { get; set; }
    }
}
