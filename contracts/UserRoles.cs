using System;
namespace contracts
{
    public  class UserRoles
    {
        public const string Admin =  "admin"; 
        public const string Operator = "operator";
        public const string Everyone= Admin + "," + Operator;
    }
}
