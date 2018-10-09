using System;
namespace contracts.Responses
{
    public class ListStatusProgressMain
    {

        public ListPermohonanResponse Header { set; get; }

        public ListStatusProgress[] Progress { set; get; }
    }

    public class ListStatusProgress
    {


        /// <summary>
        /// Ambil Tanggal Pertama Kali Status diCreate.
        /// </summary>
        /// <value>The tanggal.</value>
        public DateTime Tanggal { set; get; }
        /// <summary>
        /// Sedang Diprosess, Selesai dst
        /// </summary>
        /// <value>The status.</value>
        /// 
        public int Status { set; get; }

        public string StatusText { set; get; }

        public string KodeLaporan { set; get; }
    }
}
