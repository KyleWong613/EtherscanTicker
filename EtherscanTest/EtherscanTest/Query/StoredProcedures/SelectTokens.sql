USE EtherScanToken
GO
 
if OBJECT_ID('SelectTokens') is not null
begin 
   drop procedure SelectTokens
   
   if OBJECT_ID('SelectTokens') is not null
      print '-- FAILED DROPPING PROCEDURE SelectTokens --'
   else
      print '-- DROPPED PROCEDURE SelectTokens --'
end
else
   print '-- PROCEDURE SelectTokens was NOT FOUND --'
go


create procedure SelectTokens
(
	@ContextUid		uniqueidentifier= NULL  
)

as
begin

if object_id('tempdb..#Tokens') is not null
	drop table #Tokens
Select * 
into #Tokens
from dbo.Token

select * from #Tokens

end
GO

if OBJECT_ID('SelectTokens') is not null
   print '<<< CREATED PROCEDURE SelectTokens >>>'
else
   print '<<< FAILED CREATING PROCEDURE SelectTokens >>>'
go