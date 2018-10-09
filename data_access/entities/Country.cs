using System;
using System.ComponentModel.DataAnnotations;

namespace data_access.entities
{
    public class Country
    {
        [Key]
        public int CountryID { set; get; }
        public string Name { set; get; }
    }
}
