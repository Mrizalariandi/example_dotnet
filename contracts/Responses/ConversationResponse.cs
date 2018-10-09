using System;
namespace contracts.Responses
{
    public class ConversationResponse
    {
        public string KodeLaporan { set; get; }
        public long ID { set; get; }
        public string By { set; get; }
        public string Message { set; get; }
        public bool isUser { set; get; }
        public string ProfilePicture { set; get; }
        public bool isRead { set; get; }
		public short Status { get; set; }
        public DateTime Tanggal { set; get; }
        public string StatusText { set; get; }
        public string AttachmentFileName { get; set; }
    }
}
