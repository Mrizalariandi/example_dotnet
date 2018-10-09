using System;
using data_access;
using System.Linq;
using services;
using contracts;
using contracts.Responses;
using contracts.Requests;
using data_access.entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Options;


namespace repositories
{
    public class TemplateEmailConstanta{
        public const string EMAIL_BALASAN_AWAL_LAPORAN = "EMAIL_BALASAN_AWAL_LAPORAN";
        public const string BALASAN_LAPORAN_DITERIMA = "BALASAN_LAPORAN_DITERIMA";
        public const string BALASAN_LAPORAN_DITOLAK = "BALASAN_LAPORAN_DITOLAK";
        public const string BALASAN_LAPORAN_DIANALISA = "BALASAN_LAPORAN_DIANALISA";
        public const string BALASAN_LAPORAN_SELESAI = "BALASAN_LAPORAN_SELESAI";

    }

    public class BantuanHukumRepository:CoreRepository,IBantuanHukumService
    {
        private EmailConfig emailConfig;
        
        public BantuanHukumRepository(DBBakumContext db,IOptions<EmailConfig> emailConfig):base(db){

            this.initialDataStatusPengaduan();
            this.emailConfig = emailConfig.Value;
        }

        private void initialDataStatusPengaduan(){

            /// Status bantuan. 1. Sedang Diproses 2. Laporan Diterima 3. Laporan Dianalisa 4. Laporan Ditolak 5. Laporan  Selesai

            if (this.Db.StatusBantuan.Count() == 0)
            {
                this.Db.StatusBantuan.Add(new StatusBantuan { Name = "Sedang Di Proses" });
                this.Db.StatusBantuan.Add(new StatusBantuan { Name = "Laporan diterima" });
                this.Db.StatusBantuan.Add(new StatusBantuan { Name = "Laporan dianalisa" });
                this.Db.StatusBantuan.Add(new StatusBantuan { Name = "Laporan ditolak" });
                this.Db.StatusBantuan.Add(new StatusBantuan { Name = "Laporan selesai" });
                this.Db.SaveChanges();
            }

        }

        public Conversation createConversation(string kodeLaporan, long replyForID,string message, int status,string replyBy,string fileName){

            Conversation msg = new Conversation();
            msg.Message = message;
            msg.CreatedBy = replyBy;
            msg.CreatedDate = DateTime.Now;


            // add attachment
            if(!string.IsNullOrWhiteSpace(fileName)){
                msg.FileName = fileName;
            }

            // get Header
            var header = this.Db.BantuanHukum.FirstOrDefault(a => a.KodeLaporan == kodeLaporan);
            if(header!=null){
                msg.Header = header;
            }

            // get Status
            var statusPengaduan = this.Db.StatusBantuan.FirstOrDefault(a => a.ID == status);
            if (statusPengaduan != null) {
                msg.Status = statusPengaduan;
            }else{
                //msg.Status.ID =Convert.ToInt16( status);
                throw new Exception("Warning!!!. Please see your STATUS configuration in your database. Your ID status is "+status+" not match with our configuration");
            }

            // get ParentID
            var dataMain = this.Db.Conversations.FirstOrDefault(a => a.ID == replyForID);
            if(dataMain!=null){
                msg.ParentID = dataMain;
            }

            this.Db.Conversations.Add(msg);
            this.Db.SaveChanges();


            /*Update isRead by Status pengaduan && hanya user saja =>>> Request a tgl 22 agustus 2018*/
            /*Berlaku hanya pas action selain dari masyarakat*/
            if (!replyBy.Contains("user_"))
            {
                var queryUpdate = this.Db.Conversations.Where(x => x.Header.ID == header.ID  && x.IsRead == false && x.CreatedBy.Contains("user_"));
                if (queryUpdate.Count() > 0)
                {
                    foreach (var item in queryUpdate)
                    {
                        var update = this.Db.Conversations.FirstOrDefault(aa => aa.ID == item.ID);
                        update.IsRead = true;
                        this.Db.Conversations.Update(update);
                        this.Db.SaveChanges();
                    }

                }
            }
            /*update end*/


            return msg;
        }

        public DropDown[] getCountries()
        {
            var query = this.Db.Countries.Select(a => new DropDown { Value = a.CountryID, Text = a.Name });

            return query.OrderBy(a => a.Text).ToArray();
        }

        private Template initialTemplate(string name){

            Template template = new Template();
            switch (name)
            {
                case TemplateEmailConstanta.EMAIL_BALASAN_AWAL_LAPORAN:

                    template = new Template
                    {
                        TemplateName = name,
                        Title = "Laporan Bantuan Hukum Online - Divkum Polri",
                        Message = "Yang Terhormat {NAMALENGKAP}, Terima Kasih Atas Laporan Anda. Berikut Kode Laporan anda {KODELAPORAN} silahkan melakukan pengecekan secara berkala untuk mengetahui progress lebih lanjut. <br><br><br> Terima Kasih, <br>Tim Bantuan Hukum Online<br> Divkum Polri"
                    };

                    break;


                case TemplateEmailConstanta.BALASAN_LAPORAN_DITERIMA:

                    template = new Template
                    {
                        TemplateName = name,
                        Title = "Laporan Bantuan Hukum Online - Divkum Polri",
                        Message = "Yang Terhormat {NAMALENGKAP}, Permohonan Anda sudah kami terima. Berikut Kode Laporan anda {KODELAPORAN} silahkan melakukan pengecekan secara berkala untuk mengetahui progress lebih lanjut. <br><br><br> Terima Kasih, <br>Tim Bantuan Hukum Online<br> Divkum Polri"
                    };

                    break;

                case TemplateEmailConstanta.BALASAN_LAPORAN_DITOLAK:

                    template = new Template
                    {
                        TemplateName = name,
                        Title = "Laporan Bantuan Hukum Online - Divkum Polri",
                        Message = "Yang Terhormat {NAMALENGKAP}, Permohonan Anda kami tolak. Berikut Kode Laporan anda {KODELAPORAN} silahkan melakukan pengecekan secara berkala untuk mengetahui progress lebih lanjut. <br><br><br> Terima Kasih, <br>Tim Bantuan Hukum Online<br> Divkum Polri"
                    };

                    break;

                case TemplateEmailConstanta.BALASAN_LAPORAN_DIANALISA:

                    template = new Template
                    {
                        TemplateName = name,
                        Title = "Laporan Bantuan Hukum Online - Divkum Polri",
                        Message = "Yang Terhormat {NAMALENGKAP}, Permohonan Anda Akan kami Analisa. Berikut Kode Laporan anda {KODELAPORAN} silahkan melakukan pengecekan secara berkala untuk mengetahui progress lebih lanjut. <br><br><br> Terima Kasih, <br>Tim Bantuan Hukum Online<br> Divkum Polri"
                    };

                    break;


                case TemplateEmailConstanta.BALASAN_LAPORAN_SELESAI:

                    template = new Template
                    {
                        TemplateName = name,
                        Title = "Laporan Bantuan Hukum Online - Divkum Polri",
                        Message = "Yang Terhormat {NAMALENGKAP}, Permohonan Anda sudah selesai diproses. Berikut Kode Laporan anda {KODELAPORAN} silahkan melakukan pengecekan secara berkala untuk mengetahui progress lebih lanjut. <br><br><br> Terima Kasih, <br>Tim Bantuan Hukum Online<br> Divkum Polri"
                    };

                    break;

                default:
                    break;
            }

            return template;
        }

        private Template getTemplate(string name){

            Template template = new Template();

            var datas = this.Db.Templates.Where(a => a.TemplateName == name);

            if(datas.Count()>0){
                template = datas.First();
            }else{
                // Insert Template
                template = this.initialTemplate(name);
                this.Db.Templates.Add(template);
                this.Db.SaveChanges();
            }

            return template;
        }

        private async Task<LogEmail> createEmail(string subject,string to,string body){

            LogEmail email = new LogEmail();
            email.Subject = subject;
            email.Body = body;
            email.To = to;
            email.CreatedDate = DateTime.Now;
            email.Sent = false;
            this.Db.LogEmails.Add(email);



            // Sending Email TODO
            EmailUtility emailutility = new EmailUtility(this.emailConfig);
            var data = await emailutility.SendEmailAsync(email.To, subject, body);
            if(string.IsNullOrWhiteSpace(data)){
                email.Sent = true;
            }else{
                email.Error = data;
            }
            await this.Db.SaveChangesAsync();
            return email;
        }

        public  Response<FormBantuanHukumResponse> request(FormBantuanHukumRequest request,string fname)
        {
            Response<FormBantuanHukumResponse> response = new Response<FormBantuanHukumResponse>();


            // Add New
            if(request.ID==0){

                var q = this.Db.LogEmails.Where(a => a.To == request.Email && a.Blocked == true);
                if(q.Count()>0){
                    response.message_type = 2;
                    response.message = "Maaf email anda termasuk dalam daftar blokir tidak bisa digunakan untuk pelaporan ini";
                    return response;
                }

                // Get Country ID
                var findKewarganegaraan = this.Db.Countries.Where(a => a.CountryID == request.Kewarganegaraan);

                Country  currentKewarganegaraan  = new Country();
                if (findKewarganegaraan.Count() > 0)
                {
                     currentKewarganegaraan = findKewarganegaraan.First();
                }

                // Get Status ' Sedang di Proses'
                var statusFind = this.Db.StatusBantuan.Where(a => a.ID==1);
                StatusBantuan status = new StatusBantuan();
                if(statusFind.Count()>0){
                    status = statusFind.First();
                }

                //string filename = string.Empty;

                string kodeLaporan = Guid.NewGuid().ToString().Substring(0,5);

                var data = new data_access.entities.BantuanHukum
                {
                    Agama = request.Agama,
                    Email = request.Email,
                    Identitas = request.Identitas,
                    IsiPermohonan  = request.IsiPermohonan,
                    JenisKelamin = request.JenisKelamin,
                    Kewarganegaraan = currentKewarganegaraan,
                    NamaLengkap = request.NamaLengkap,
                    NoIdentitas = request.NoIdentitas,
                    Pekerjaan = request.Pekerjaan,
                    Telpn = request.Telpn,
                    Umur = request.Umur,
                    FileName = fname,
                    LastStatus = status,
                    CreatedDate = DateTime.Now,
                    KodeLaporan  = kodeLaporan
                };
                this.Db.BantuanHukum.Add(data);
                this.Db.SaveChanges();

                // Generate Kode Laporan 'Update'

                var datas = this.Db.BantuanHukum.Where(a => a.ID == data.ID);
                if(datas.Count()>0){
                    data = datas.First();
                    data.KodeLaporan = kodeLaporan + data.ID;
                    data.LastStatus = status; // Update Last Status Pengaduan
                    this.Db.BantuanHukum.Update(data);
                    this.Db.SaveChanges();
                }


                // Create Conversation
                // Copy Message from header to Activity with Status 
                this.createConversation(data.KodeLaporan, 0, request.IsiPermohonan, status.ID, "user_"+data.KodeLaporan,string.Empty);


                // Get Template Email
                var template = this.getTemplate(TemplateEmailConstanta.EMAIL_BALASAN_AWAL_LAPORAN);

                string body =  template.Message;
                    body = body.Replace("{NAMALENGKAP}", request.NamaLengkap).Replace("{KODELAPORAN}", data.KodeLaporan);
                // Send Email
                var email =  this.createEmail(template.Title,request.Email,body).GetAwaiter().GetResult();


                response.data = new FormBantuanHukumResponse {ID = data.ID, KodeLaporan = data.KodeLaporan };

            }

            return response;
        }

        public Response<FormBantuanHukumResponse> reply(ReplyConversationRequest request,string fileName,string userlogin)
        {   

            Response<FormBantuanHukumResponse> response = new Response<FormBantuanHukumResponse>();



            // Update Status Header
            var header = this.Db.BantuanHukum.FirstOrDefault(a => a.KodeLaporan == request.KodeLaporan);
            if(header!=null){

                var status = this.Db.StatusBantuan.FirstOrDefault(a => a.ID == request.Status);
                if (status != null)
                {
                    header.LastStatus = status;
                }else{
                    // throw new Exception("Status Error");
                    response.message_type = 2;
                    response.message = "Status Error";
                }
            }

            // Create Conversation
            Conversation data = new Conversation();
            try
            {
                data = this.createConversation(request.KodeLaporan, request.ID, request.Comment, request.Status, userlogin,fileName);

               
            }
            catch(Exception exc){
                response.message = exc.Message;
                response.message_type = 2;
            }
            response.data = new FormBantuanHukumResponse { ID = data.ID, KodeLaporan = request.KodeLaporan };

            return response;

        }

        public Response<ListPermohonanResponse[]> listpermohonan(FilterPermohonanRequest request)
        {

            Response<ListPermohonanResponse[]> response = new Response<ListPermohonanResponse[]>();

            try
            {
                // Main Query
                IQueryable<ListPermohonanResponse> mainQuery = this.mainQueryHeader(request);

                // Sorting Asc or Descending
                switch (request.SortField)
                {
                    case "KodeLaporan_desc":
                        mainQuery = mainQuery.OrderByDescending(a => a.KodeLaporan);
                        break;
                    case "KodeLaporan_asc":
                        mainQuery = mainQuery.OrderBy(a => a.KodeLaporan);
                        break;
                    case "IsiPermohonan_desc":
                        mainQuery = mainQuery.OrderByDescending(a => a.IsiPermohonan);
                        break;
                    case "IsiPermohonan_asc":
                        mainQuery = mainQuery.OrderBy(a => a.IsiPermohonan);
                        break;
                    case "Tanggal_desc":
                        mainQuery = mainQuery.OrderByDescending(a => a.Tanggal);
                        break;
                    case "Tanggal_asc":
                        mainQuery = mainQuery.OrderBy(a => a.Tanggal);
                        break;
                    case "IsLaporanDitolak_desc":
                        mainQuery = mainQuery.OrderByDescending(a => a.IsLaporanDitolak);
                        break;
                    case "IsLaporanDitolak_asc":
                        mainQuery = mainQuery.OrderBy(a => a.IsLaporanDitolak);
                        break;
                    case "IsLaporanDianalisa_desc":
                        mainQuery = mainQuery.OrderByDescending(a => a.IsLaporanDianalisa);
                        break;
                    case "IsLaporanDianalisa_asc":
                        mainQuery = mainQuery.OrderBy(a => a.IsLaporanDianalisa);
                        break;
                    case "IsLaporanDiproses_desc":
                        mainQuery = mainQuery.OrderByDescending(a => a.IsLaporanDiproses);
                        break;
                    case "IsLaporanDiproses_asc":
                        mainQuery = mainQuery.OrderBy(a => a.IsLaporanDiproses);
                        break;
                    case "IsLaporanDiterima_desc":
                        mainQuery = mainQuery.OrderByDescending(a => a.IsLaporanDiterima);
                        break;
                    case "IsLaporanDiterima_asc":
                        mainQuery = mainQuery.OrderBy(a => a.IsLaporanDiterima);
                        break;
                    case "IsLaporanSelesai_desc":
                        mainQuery = mainQuery.OrderByDescending(a => a.IsLaporanSelesai);
                        break;
                    case "IsLaporanSelesai_asc":
                        mainQuery = mainQuery.OrderBy(a => a.IsLaporanSelesai);
                        break;
                    default:
                        mainQuery = mainQuery.OrderBy(a => a.KodeLaporan);
                        break;
                }


                // Paging
                var count = mainQuery.Count();
                var items = mainQuery.Skip((request.pageIndex - 1) * request.pageSize).Take(request.pageSize).ToArray();

                response.data = items;

                // Property Information for javascript client
                response.count = count;
                response.totalpages =  (int)Math.Ceiling(count / (double)request.pageSize);
                response.totalpages = response.totalpages < 0 ? 1 : response.totalpages;

            }
            catch (Exception exc)
            {
                response.message = exc.Message;
                response.message_type = 2;
            }

            return response;
        }

      

        private IQueryable<ListPermohonanResponse> mainQueryHeader(FilterPermohonanRequest request)
        {



                IQueryable<HeaderConversation> datas = from x in this.Db.BantuanHukum
                                                               select new HeaderConversation{HeaderID=x.ID,  
                    Status=x.LastStatus.ID, 
                    CreatedDate=x.CreatedDate,
                    KodeLaporan=x.KodeLaporan,
                    IsiPermohonan=x.IsiPermohonan};

                if (!string.IsNullOrWhiteSpace(request.TanggalMulai) && !string.IsNullOrWhiteSpace(request.TanggalAkhir))
                {

                    // Filter By Date
                    DateTime dtstart = DateTime.Parse(request.TanggalMulai.ToString());
                    DateTime dtend = DateTime.Parse(request.TanggalAkhir.ToString());
                    datas = datas.Where(a => a.CreatedDate >= dtstart && a.CreatedDate <= dtend);
                }

                IQueryable<HeaderConversation> queries = from x in datas
                              from z in this.Db.Conversations
                              where
                                            x.HeaderID == z.Header.ID
                                            select new HeaderConversation{ HeaderID = x.HeaderID, Status = z.Status.ID, KodeLaporan = x.KodeLaporan, IsiPermohonan = x.IsiPermohonan };

                // Filter By KodeLaporan
                if (!string.IsNullOrWhiteSpace(request.KodeLaporan))
                {
                    queries = queries.Where(a => a.KodeLaporan == request.KodeLaporan);
                }

                // Search By Content
                if (!string.IsNullOrWhiteSpace(request.Konten))
                {
                    queries = queries.Where(a => a.IsiPermohonan.Contains(request.Konten));

                }

                // Filter By Status
            if (request.Status != null && request.Status.Count() > 0)
                {
                    queries = from x in queries
                              from y in request.Status
                              where x.Status == y
                              select x;
                }

                //  Grouping By Header
                var groupHeader = from x in queries
                            group x by x.HeaderID
                                        into grp
                                        select new GroupingByStatus{ID=grp.Key, Status =

                  (from st in  (from yw in grp.Select(a=>a.Status) group yw by yw into ywg select ywg)
                    from stm in this.Db.StatusBantuan
                    where st.Key==stm.ID select new StatusBantuanRow{ ID=stm.ID,Name=stm.Name } )
                };

                //  var groupByStatus = from x in groupHeader.
            IQueryable<ListPermohonanResponse> response  = from g in groupHeader
                                    from header in this.Db.BantuanHukum
                                                       from country in this.Db.Countries

                                 where g.ID==header.ID
                                                       && header.Kewarganegaraan.CountryID==country.CountryID
                                                       select new ListPermohonanResponse { 
                    ID = g.ID,
                    KodeLaporan = header.KodeLaporan,
                    Tanggal = header.CreatedDate,
                    IsLaporanDiproses=(g.Status.Where(a=>a.ID==1).Count()>0),
                    IsLaporanDiterima = (g.Status.Where(a => a.ID == 2).Count() > 0),
                    IsLaporanDianalisa = (g.Status.Where(a => a.ID == 3).Count() > 0),
                    IsLaporanDitolak = (g.Status.Where(a => a.ID == 4).Count() > 0),
                    IsLaporanSelesai = (g.Status.Where(a => a.ID == 5).Count() > 0),
                    NamaLengkap=header.NamaLengkap,
                    Umur=header.Umur,
                    Pekerjaan=header.Pekerjaan,
                    JenisKelamin=header.JenisKelamin,
                    Agama=header.Agama,
                    Kewarganegaraan=header.Kewarganegaraan.CountryID,
                    Telpn=header.Telpn,
                    Identitas=header.Identitas,
                    NoIdentitas=header.NoIdentitas,
                    Email=header.Email,
                    IsiPermohonan=header.IsiPermohonan,
                    FileName=header.FileName,
                KewarganegaraanText = country.Name
                };

            return response;
        }

        public HeaderConversation getLastStatusPermohonanByKodeLaporan(string kodeLaporan)
        {

            var data = from x in this.Db.BantuanHukum
                       from y in this.Db.StatusBantuan

                       where
                                     x.LastStatus == y
                                     && x.KodeLaporan==kodeLaporan
                                     select new { ID = y.ID, Name = y.Name, HeaderID=x.ID};
            

            if(data.Count()>0){
                var dataCurrent =  data.First();
                return new HeaderConversation { Status =dataCurrent.ID,HeaderID = dataCurrent.HeaderID };

            }
            return new HeaderConversation{ };
        }

        public string getLastConversationByHeaderID(long headerId)
        {
            var query = this.Db.Conversations.Where(a => a.Header.ID == headerId).OrderByDescending(a => a.CreatedDate).Skip(0).Take(1);

            if(query.Count()==1){
                return query.First().CreatedBy;
            }
            return "";

        }

        public Response<ConversationResponse[]> listMessages(ListMessageRequest request)
        {
            Response<ConversationResponse[]> response = new Response<ConversationResponse[]>();


            // Query diubah menjadi ascending, permintaan tgl 22Agustus 2018.
            var query = from x in this.Db.Conversations
                        from y in this.Db.BantuanHukum
                                      from w in this.Db.StatusBantuan
                        where x.Header.ID == y.ID && x.Status.ID==w.ID
                                      orderby x.CreatedDate ascending
                        select new ConversationResponse 
                        {
                            isUser = x.CreatedBy.Contains("user_"+y.KodeLaporan),
                            By = x.CreatedBy.Contains("user_" + y.KodeLaporan) ? x.Header.NamaLengkap: x.CreatedBy,
                            ID = x.ID,
                            KodeLaporan = y.KodeLaporan,
                            Message = x.Message,
                            isRead = x.IsRead,
                            Status = x.Status.ID,
                            Tanggal = x.CreatedDate,
                            StatusText = w.Name,
                            AttachmentFileName = x.FileName
                        };

            if(!string.IsNullOrWhiteSpace(request.KodeLaporan))
            {
                query = query.Where(a => a.KodeLaporan == request.KodeLaporan);
            }

            ///isRead: 0 = keluarkan semua, 1 = hanya yang sudah dibaca, 2 = yang belum dibaca
            if(request.isRead>0){
                query = query.Where(a => a.isRead == (request.isRead==1));
            }


            if(request.IsUser){
                query = query.Where(a => a.isUser==request.IsUser);
            }

            // Status Filter
            if(request.Status!=0){
                query = query.Where(a => a.Status == request.Status);
            }


            response.count = query.Count();
            // Set Paging
            int perpage = 10;
            var num = (Convert.ToDecimal(response.count) / Convert.ToDecimal(perpage));
            response.totalpages = (int)Math.Ceiling(num);


            int take = request.pageIndex == 1 ? perpage : request.pageIndex * perpage;
            int skip = request.pageIndex > 1 ? (request.pageIndex - 1) * perpage : 0;
            response.data = query.Skip(skip).Take(take).ToArray();

            // Setelah final dipaging ambil foto profle Operator

            var dataUser = (from x in this.Db.Users

                            from y in response.data.Where(a => a.isUser == false).GroupBy(a=>a.By)
                            where x.UserName == y.Key
                            select new { profile = x.ProfilePicture, username = x.UserName}).ToArray();
            foreach(var item in response.data){
                if (item.isUser==false)
                {
                    var getProfile = dataUser.SingleOrDefault(a => a.username == item.By);
                    if(getProfile!=null){
                        item.ProfilePicture = "staticFiles/"+getProfile.profile;
                    }
                }
            }


            return response;
        }

        public Response<ConversationResponse[]> listNotification(ListMessageRequest request)
        {
            Response<ConversationResponse[]> response = new Response<ConversationResponse[]>();


            // Query diubah menjadi ascending, permintaan tgl 22Agustus 2018.
            var query = from x in this.Db.Conversations
                        from y in this.Db.BantuanHukum
                        from w in this.Db.StatusBantuan
                        where x.Header.ID == y.ID && x.Status.ID == w.ID
                        orderby x.CreatedDate descending
                        select new ConversationResponse
                        {
                            isUser = x.CreatedBy.Contains("user_" + y.KodeLaporan),
                            By = x.CreatedBy.Contains("user_" + y.KodeLaporan) ? x.Header.NamaLengkap : x.CreatedBy,
                            ID = x.ID,
                            KodeLaporan = y.KodeLaporan,
                            Message = x.Message,
                            isRead = x.IsRead,
                            Status = x.Status.ID,
                            Tanggal = x.CreatedDate,
                            StatusText = w.Name
                        };

            if (!string.IsNullOrWhiteSpace(request.KodeLaporan))
            {
                query = query.Where(a => a.KodeLaporan == request.KodeLaporan);
            }

            ///isRead: 0 = keluarkan semua, 1 = hanya yang sudah dibaca, 2 = yang belum dibaca
            if (request.isRead > 0)
            {
                query = query.Where(a => a.isRead == (request.isRead == 1));
            }


            if (request.IsUser)
            {
                query = query.Where(a => a.isUser == request.IsUser);
            }

            // Status Filter
            if (request.Status != 0)
            {
                query = query.Where(a => a.Status == request.Status);
            }


            response.count = query.Count();
            // Set Paging
            int perpage = 10;
            var num = (Convert.ToDecimal(response.count) / Convert.ToDecimal(perpage));
            response.totalpages = (int)Math.Ceiling(num);


            int take = request.pageIndex == 1 ? perpage : request.pageIndex * perpage;
            int skip = request.pageIndex > 1 ? (request.pageIndex - 1) * perpage : 0;
            response.data = query.Skip(skip).Take(take).ToArray();

            // Setelah final dipaging ambil foto profle Operator

            var dataUser = (from x in this.Db.Users

                            from y in response.data.Where(a => a.isUser == false).GroupBy(a => a.By)
                            where x.UserName == y.Key
                            select new { profile = x.ProfilePicture, username = x.UserName }).ToArray();
            foreach (var item in response.data)
            {
                if (item.isUser == false)
                {
                    var getProfile = dataUser.SingleOrDefault(a => a.username == item.By);
                    if (getProfile != null)
                    {
                        item.ProfilePicture = "staticFiles/" + getProfile.profile;
                    }
                }
            }


            return response;
        }

        public Response<ListStatusProgressMain> listStatusProgress(string KodeLaporan)
        {
            List<ListStatusProgress> datas = new List<ListStatusProgress>();

            Response<ListStatusProgressMain> response = new Response<ListStatusProgressMain> { };
            var data = this.mainQueryHeader(new FilterPermohonanRequest { KodeLaporan = KodeLaporan });

            foreach(var item in this.Db.StatusBantuan){
                
                if(item.ID==1 && data.Select(a=>a.IsLaporanDiproses).Count()>0){

                    // get date
                    var conv = this.Db.Conversations.Where(a => a.Header.KodeLaporan == KodeLaporan && a.Status.ID == item.ID)
                    .OrderBy(a => a.CreatedBy).Skip(0).Take(1);
                    if (conv.Count() > 0)
                    {
                        datas.Add(new ListStatusProgress { 
                            KodeLaporan = KodeLaporan, Status = item.ID, StatusText = item.Name, Tanggal = conv.First().CreatedDate });
                    }
                }

                if (item.ID == 2 && data.Select(a => a.IsLaporanDiterima).Count() > 0)
                {

                    // get date
                    var conv = this.Db.Conversations.Where(a => a.Header.KodeLaporan == KodeLaporan && a.Status.ID == item.ID)
                    .OrderBy(a => a.CreatedBy).Skip(0).Take(1);
                    if (conv.Count() > 0)
                    {
                        datas.Add(new ListStatusProgress { KodeLaporan = KodeLaporan, Status = item.ID, StatusText = item.Name, Tanggal = conv.First().CreatedDate });
                    }
                }

                if (item.ID == 3 && data.Select(a => a.IsLaporanDianalisa).Count() > 0)
                {

                    // get date
                    var conv = this.Db.Conversations.Where(a => a.Header.KodeLaporan == KodeLaporan && a.Status.ID == item.ID)
                    .OrderBy(a => a.CreatedBy).Skip(0).Take(1);
                    if (conv.Count() > 0)
                    {
                        datas.Add(new ListStatusProgress { KodeLaporan = KodeLaporan, Status = item.ID, StatusText = item.Name, Tanggal = conv.First().CreatedDate });
                    }
                }

                if (item.ID == 4 && data.Select(a => a.IsLaporanDitolak).Count() > 0)
                {

                    // get date
                    var conv = this.Db.Conversations.Where(a => a.Header.KodeLaporan == KodeLaporan && a.Status.ID == item.ID)
                    .OrderBy(a => a.CreatedBy).Skip(0).Take(1);
                    if (conv.Count() > 0)
                    {
                        datas.Add(new ListStatusProgress { KodeLaporan = KodeLaporan, Status = item.ID, StatusText = item.Name, Tanggal = conv.First().CreatedDate });
                    }
                }

                if (item.ID == 5 && data.Select(a => a.IsLaporanSelesai).Count() > 0)
                {

                    // get date
                    var conv = this.Db.Conversations.Where(a => a.Header.KodeLaporan == KodeLaporan && a.Status.ID == item.ID)
                    .OrderBy(a => a.CreatedBy).Skip(0).Take(1);
                    if (conv.Count() > 0)
                    {
                        datas.Add(new ListStatusProgress { KodeLaporan = KodeLaporan, Status = item.ID, StatusText = item.Name, Tanggal = conv.First().CreatedDate });
                    }
                }
            }

            var headers = this.listpermohonan(new FilterPermohonanRequest { KodeLaporan = KodeLaporan, pageIndex = 1, pageSize = 1 });
            if (headers.data.Count() > 0)
            {
                response.data = new ListStatusProgressMain
                {
                    Header = headers.data[0],
                    Progress = datas.ToArray()
                };
            }

            return response;
        }

        public Response<ListBlockEmail[]> listEmailBlocked()
        {
            Response<ListBlockEmail[]> response = new Response<ListBlockEmail[]> { };


            var datas = from x in this.Db.LogEmails
                        where x.Blocked
                        select new ListBlockEmail { Email=x.To };
            response.data = datas.ToArray();
            return response;
        }

        public Response<string> blockingEmail(ListBlockEmail email)
        {
            Response<string> response = new Response<string> { };

            var q = this.Db.LogEmails.Where(a => a.To == email.Email && a.Blocked==true);

            if (q.Count() == 0)
            {
                this.Db.LogEmails.Add(new LogEmail { Blocked = true, To = email.Email });
                this.Db.SaveChanges();
            }else{
                response.message_type = 2;
                response.message = "Email sudah terdaftar dalam daftar blocking";
            }

            return response;        
        }

        public Response<string> getTemplateByKodeLaporan(string template_name, string KodeLaporan)
        {
            Response<string> response = new Response<string>();

            var header = this.Db.BantuanHukum.Where(a => a.KodeLaporan == KodeLaporan);

            if(header.Count()>0){
                var u = header.First();

                if (template_name == TemplateEmailConstanta.BALASAN_LAPORAN_DITERIMA)
                {
                    var tmp = this.getTemplate(TemplateEmailConstanta.BALASAN_LAPORAN_DITERIMA);
                    response.data = tmp.Message.Replace("{NAMALENGKAP}",u.NamaLengkap).Replace("{KODELAPORAN}",u.KodeLaporan);
                }
                if (template_name == TemplateEmailConstanta.BALASAN_LAPORAN_DIANALISA)
                {
                    var tmp = this.getTemplate(TemplateEmailConstanta.BALASAN_LAPORAN_DIANALISA);
                    response.data = tmp.Message.Replace("{NAMALENGKAP}", u.NamaLengkap).Replace("{KODELAPORAN}", u.KodeLaporan);
                }
                if (template_name == TemplateEmailConstanta.BALASAN_LAPORAN_DITOLAK)
                {
                    var tmp = this.getTemplate(TemplateEmailConstanta.BALASAN_LAPORAN_DITOLAK);
                    response.data = tmp.Message.Replace("{NAMALENGKAP}", u.NamaLengkap).Replace("{KODELAPORAN}", u.KodeLaporan);
                }
                if (template_name == TemplateEmailConstanta.BALASAN_LAPORAN_SELESAI)
                {
                    var tmp = this.getTemplate(TemplateEmailConstanta.BALASAN_LAPORAN_SELESAI);
                    response.data = tmp.Message.Replace("{NAMALENGKAP}", u.NamaLengkap).Replace("{KODELAPORAN}", u.KodeLaporan);
                }
            }

            return response;
        }

        public Response<string> read(long ID)
        {
            Response<string> response = new Response<string>();

            if(ID>0){
                var ids = this.Db.Conversations.FirstOrDefault(a => a.ID==ID);

                if(ids!=null){
                    ids.IsRead = true;
                    this.Db.Conversations.Update(ids);
                    this.Db.SaveChanges();

                    response.data = "Reading this message";
                    response.message_type = 1;
                }
            }

            return response;
        }

        public Response<string> editBlockingEmail(ListBlockEmail email)
        {
            Response<string> response = new Response<string>();

            if (email.Id > 0)
            {
                var ids = this.Db.LogEmails.FirstOrDefault(a => a.ID == email.Id);

                if (ids != null)
                {
                    ids.To = email.Email;
                    this.Db.LogEmails.Update(ids);
                    this.Db.SaveChanges();

                    response.data = "updated";
                    response.message_type = 1;
                }
            }

            return response;
        }

        public Response<string> removeBlockingEmail(long id )
        {
            Response<string> response = new Response<string>();

            if (id > 0)
            {
                var ids = this.Db.LogEmails.FirstOrDefault(a => a.ID ==id);

                if (ids != null)
                {
                    this.Db.LogEmails.Remove(ids);
                    this.Db.SaveChanges();

                    response.data = "deleted";
                    response.message_type = 1;
                }
            }

            return response;
        }
    }
}
