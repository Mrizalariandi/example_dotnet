using System;
namespace contracts.Requests
{
    public class FilterPermohonanRequest
    {
        public string KodeLaporan { set; get; }
        public string TanggalMulai { set; get; }
        public string TanggalAkhir { set; get; }
        public string Konten { set; get; }
        public int[] Status { set; get; }

        /// <summary>
        /// Field Name to sort e.g: 'KodeLaporan_desc' / 'KodeLaporan'
        /// </summary>
        /// <value>The sort field.</value>
        public string SortField { set; get; }


        public int pageIndex { set; get; }
        public int pageSize { set; get; }

    }
}
