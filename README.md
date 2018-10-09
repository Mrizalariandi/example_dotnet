# README #
> dotnet ef migrations add InitialCreate
> dotnet ef database update --context DBBakumContext


#Patch Database
USE dev_dbbakum;
drop TABLE dev_dbbakum.dbo.AspNetUserRoles;
drop TABLE dev_dbbakum.dbo.AspNetUserLogins;
drop TABLE dev_dbbakum.dbo.AspNetUserTokens;
drop TABLE dev_dbbakum.dbo.AspNetUserClaims;
drop TABLE dev_dbbakum.dbo.AspNetRoleClaims;
drop TABLE dev_dbbakum.dbo.AspNetUsers;
drop TABLE dev_dbbakum.dbo.AspNetRoles;

#Update DB Context

> dotnet ef database update --context DBBakumContext

#initial Data
Jalankan Postman untuk simulasi data:
http://localhost:5001/api/init

#how to install LDAP in docker
https://nordes.github.io/#/Articles/howto-openldap-with-contoso-users


#patch Juni 21 2018
dotnet ef database update --context 2018Juni1901