using System;
namespace contracts.Requests
{
    public class ReplyConversationRequest
    {
        public string KodeLaporan { set; get; }
        public long ID { set; get; }
        public int Status { set; get; }
        public string Comment { set; get; }
        public Microsoft.AspNetCore.Http.IFormFile File { get; set; }

    }
}
