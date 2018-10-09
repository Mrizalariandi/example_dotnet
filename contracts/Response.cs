using System;

namespace contracts
{
    public class Response<T>
    {
        public T data { set; get; }
        public string message { set; get; }
        // 1 Success, 2 Error, 3 Information, 4 Warning
        public int message_type { set; get; }
        public int count { set; get; }
        public int totalpages { set; get; }
    }
}
