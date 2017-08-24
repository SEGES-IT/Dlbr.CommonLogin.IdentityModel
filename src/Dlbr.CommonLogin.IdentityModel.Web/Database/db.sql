USE [WebTestAppWifCookieCache]
GO

/****** Object:  Table [dbo].[SecurityTokenCacheEntries]    Script Date: 02/05/2013 12:42:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SecurityTokenCacheEntries](
	[Id] [nvarchar](256) NOT NULL,
	[SecurityTokenSerialized] [nvarchar](max) NULL,
	[TimeStamp] [datetime] NOT NULL,
 CONSTRAINT [PK_SecurityTokenCacheEntries] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


