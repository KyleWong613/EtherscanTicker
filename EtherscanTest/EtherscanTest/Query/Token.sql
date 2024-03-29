USE EtherScanToken
GO
 DROP TABLE dbo.Token

GO

/****** Object:  Table [Stage].[[RecContext]]    Script Date: 2/27/2019 2:53:44 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO 
CREATE TABLE Token(
	ID int IDENTITY(1,1),
	symbol [varchar](255) NOT NULL,
	Price [varchar](255) NULL,
	ContractAddress [varchar](255) NULL,
	TotalSupply [numeric](19,0) NOT NULL,
	TotalHolders [numeric](19,0) NOT NULL,
	Name [varchar](255) NOT NULL,

 CONSTRAINT [PK_TokenID] PRIMARY KEY CLUSTERED 
(
	ID ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


