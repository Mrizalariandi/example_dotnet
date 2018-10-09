using System;
namespace contracts.Responses
{
    public class HeaderConversation
    {
        public int Status { set; get; }
        public long HeaderID { set; get; }
        public DateTime CreatedDate { set; get; }
        public string KodeLaporan { set; get; }
        public string IsiPermohonan { set; get; }
    }
}
