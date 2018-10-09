using System;
namespace contracts.Requests
{
    public class ListMessageRequest
    {
        public int pageIndex { set; get; }
        public string KodeLaporan { set; get; }
        ///isRead: 0 = keluarkan semua, 1 = hanya yang sudah dibaca, 2 = yang belum dibaca
        public int isRead { set; get; }
        // Kasih nilai true jika ingin menampilkan pesan dari user saja
        public bool IsUser { set; get; }
        public int Status { set; get; }
    }
}