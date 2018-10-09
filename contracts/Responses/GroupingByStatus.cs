using System;
using System.Collections.Generic;

namespace contracts.Responses
{
    public class StatusBantuanRow{
        public int ID { set; get; }
        public string Name { set; get; }
    }
    public class GroupingByStatus
    {
        public long ID { set; get; }
        public IEnumerable<StatusBantuanRow> Status { set; get; }
    }

 

}
