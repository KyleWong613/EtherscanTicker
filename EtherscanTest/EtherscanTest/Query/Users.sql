USE SalesReport
GO
 DROP TABLE dbo.Users

GO

/****** Object:  Table [Stage].[[RecContext]]    Script Date: 2/27/2019 2:53:44 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO 
CREATE TABLE Users(
	UserID [uniqueidentifier] NOT NULL ,
	UserName [varchar](255) NOT NULL,
	UserLastName [varchar](255) NULL,
	UserEmail [varchar](255) NOT NULL,
	UserPassword [varchar](255) NOT NULL,
	UserRole [varchar](255) NOT NULL,
	UserReport [varchar](255) NOT NULL,
	loginID [numeric](19,0) NOT NULL,
 CONSTRAINT [PK_UserID] PRIMARY KEY CLUSTERED 
(
	UserID ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


