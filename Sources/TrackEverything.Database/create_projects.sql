USE [TutorialDatabase]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Projects](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nchar](20) NOT NULL,
	[shortname] [nchar](10) NOT NULL,
	[description] [text] NULL,
	[creation_date] [datetime2] NOT NULL,
	[status] [int] NOT NULL,
 CONSTRAINT [PK_Projects] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


