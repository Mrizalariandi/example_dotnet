using System;
namespace contracts.Requests
{
    public class FilterRegisteredUser
    {
		public int pageSize;
		public int pageIndex;

		public string Role { set; get; }
    }
}
