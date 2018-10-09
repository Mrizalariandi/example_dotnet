using System;
namespace contracts
{
    public class EmailConfig
    {
     
        public String Domain { get; set; }

        public int Port { get; set; }

        public String UsernameEmail { get; set; }

        public String UsernamePassword { get; set; }

        public String FromEmail { get; set; }
    
    }
}
