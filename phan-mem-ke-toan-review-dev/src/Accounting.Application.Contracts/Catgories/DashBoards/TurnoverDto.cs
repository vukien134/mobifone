using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;

namespace Accounting.Catgories.DashBoards
{
    public class TurnoverDto
    {
        public int Id { get; set; }
        public decimal? Amount { get; set; }
        public string Month { get; set; }
    }
}
