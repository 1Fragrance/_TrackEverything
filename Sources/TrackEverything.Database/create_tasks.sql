USE [TutorialDatabase]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Tasks](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nchar](20) NOT NULL,
	[time] [bigint] NOT NULL,
	[status] [int] NOT NULL,
	[creation_date] [datetime2] NOT NULL,
	[started_at] [datetime2] NULL,
	[ended_at] [datetime2] NULL,
	[project_id] [int] NULL,
 CONSTRAINT [PK_Tasks] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


