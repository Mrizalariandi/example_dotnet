using System;
using System.Threading.Tasks;
using contracts;
using contracts.Requests;
using contracts.Responses;

namespace services
{
    public interface IBantuanHukumService
    {
        Response<FormBantuanHukumResponse> request(FormBantuanHukumRequest request,string fname);

        DropDown[] getCountries();

        Response<FormBantuanHukumResponse> reply(ReplyConversationRequest request,string fileName,string userlogin);

        Response<ListPermohonanResponse[]> listpermohonan(FilterPermohonanRequest request);

        HeaderConversation getLastStatusPermohonanByKodeLaporan(string kodeLaporan);

        string getLastConversationByHeaderID(long headerId);

        //TODOs
        //(Filter by isRead dan keluarkan total berapa banyak pesan yang belum dibaca
        Response<ConversationResponse[]> listMessages(ListMessageRequest request);

        Response<ConversationResponse[]> listNotification(ListMessageRequest request);

        Response<string> getTemplateByKodeLaporan(string template_name,string KodeLaporan);

        // List status untuk user mengecek progress
        Response<ListStatusProgressMain> listStatusProgress(string KodeLaporan);

        // List email yang terkirim
        //void listLogEmail();

        // Resend Email
        //void resendEmail();

        // list Template 
        //void listTemplate();

        // Update Template
        //void updateTemplate();

        //list Block email
        Response<ListBlockEmail[]> listEmailBlocked();

        // entry block email

        Response<string> blockingEmail(ListBlockEmail email);

        Response<string> editBlockingEmail(ListBlockEmail email);

        Response<string> removeBlockingEmail(long id);

        Response<string> read(long ID);


        // Add Menu, Edit Menu, Delete Menu, List Menu

        //Add RoleMenu, Edit Role Menu, Delete
    }
}
