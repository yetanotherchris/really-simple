CREATE TABLE [dbo].[categories] 
(
	[id] [uniqueidentifier] NOT NULL,
	[title] [nvarchar] (300) NOT NULL,

	PRIMARY KEY NONCLUSTERED (id)
)

CREATE TABLE [dbo].[sites] 
(
	[id] [uniqueidentifier] NOT NULL,
	[url] [nvarchar] (300) NOT NULL,
	[title] [nvarchar] (300) NOT NULL,

	PRIMARY KEY NONCLUSTERED (id)
)

CREATE TABLE [dbo].[feeds] 
(
	[id] [uniqueidentifier] DEFAULT(newid()),
	[categoryid] [uniqueidentifier] NOT NULL,
	[siteid] [uniqueidentifier] NOT NULL,
	[url] [nvarchar] (300) NOT NULL,
	[type] [tinyint],
	[lastupdate] [datetime],
	[cleaner] [varchar] (100)

	PRIMARY KEY NONCLUSTERED (id)
)

CREATE TABLE [dbo].[items] 
(
	[id] [uniqueidentifier] NOT NULL,
	[feedid] [uniqueidentifier] NOT NULL,
	[link] [nvarchar] (300) NOT NULL,
	[title] [nvarchar] (200) NOT NULL,
	[content] [text]  NULL,
	[publishdate] [datetime],
	[retrievedate] [datetime],
	[imagefilename] [varchar] (300) NULL,
	[imageurl] [nvarchar] (400)

	PRIMARY KEY CLUSTERED (link)
)

