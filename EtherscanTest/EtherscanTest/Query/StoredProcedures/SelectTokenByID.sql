USE EtherScanToken
GO
 
if OBJECT_ID('SelectTokenByID') is not null
begin 
   drop procedure SelectTokenByID
   
   if OBJECT_ID('SelectTokenByID') is not null
      print '-- FAILED DROPPING PROCEDURE SelectTokenByID --'
   else
      print '-- DROPPED PROCEDURE SelectTokenByID --'
end
else
   print '-- PROCEDURE SelectTokenByID was NOT FOUND --'
go


create procedure SelectTokenByID
(
	@Token		varchar = NULL  
)

as
begin

if object_id('tempdb..#Tokens') is not null
	drop table #Tokens
Select * 
into #Tokens
from dbo.Token

select * from #Tokens
where symbol LIKE '%' + @Token + '%'

end
GO

if OBJECT_ID('SelectTokens') is not null
   print '<<< CREATED PROCEDURE SelectTokenByID >>>'
else
   print '<<< FAILED CREATING PROCEDURE SelectTokenByID >>>'
go