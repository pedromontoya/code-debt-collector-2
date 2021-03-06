USE [master]
GO
/****** Object:  DATABASE [CodeBaseData]    Script Date: 9/10/2014 6:58:05 AM ******/
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'CodeBaseData')
CREATE DATABASE [CodeBaseData]
GO
ALTER DATABASE [CodeBaseData] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CodeBaseData].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [CodeBaseData] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [CodeBaseData] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [CodeBaseData] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [CodeBaseData] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [CodeBaseData] SET ARITHABORT OFF 
GO
ALTER DATABASE [CodeBaseData] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [CodeBaseData] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [CodeBaseData] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [CodeBaseData] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [CodeBaseData] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [CodeBaseData] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [CodeBaseData] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [CodeBaseData] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [CodeBaseData] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [CodeBaseData] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [CodeBaseData] SET  DISABLE_BROKER 
GO
ALTER DATABASE [CodeBaseData] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [CodeBaseData] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [CodeBaseData] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [CodeBaseData] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [CodeBaseData] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [CodeBaseData] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [CodeBaseData] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [CodeBaseData] SET RECOVERY FULL 
GO
ALTER DATABASE [CodeBaseData] SET  MULTI_USER 
GO
ALTER DATABASE [CodeBaseData] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [CodeBaseData] SET DB_CHAINING OFF 
GO
ALTER DATABASE [CodeBaseData] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [CodeBaseData] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
EXEC sys.sp_db_vardecimal_storage_format N'CodeBaseData', N'ON'
GO
USE [CodeBaseData]
GO
/****** Object:  StoredProcedure [dbo].[rspGetCodeDebtDetail]    Script Date: 9/10/2014 6:58:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[rspGetCodeDebtDetail]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'

CREATE   PROC [dbo].[rspGetCodeDebtDetail]
AS

	SELECT     CodeBase, Initials, Type, Criticality, Estimate, Comment, SourceFile, SourcePath
	FROM         CodeDebt WITH(NOLOCK)
	WHERE LastUpdatedDate BETWEEN DateAdd(dd,-1,getdate()) AND getdate()
	ORDER BY   CodeBase, Initials, Type, Criticality



' 
END
GO
/****** Object:  StoredProcedure [dbo].[rspGetCodeDebtSummaryByCodeBase]    Script Date: 9/10/2014 6:58:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[rspGetCodeDebtSummaryByCodeBase]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE  PROC [dbo].[rspGetCodeDebtSummaryByCodeBase]
AS

	SELECT     CodeBase, Criticality, SUM(Estimate) Estimates
	FROM         CodeDebt
	WHERE LastUpdatedDate BETWEEN DateAdd(dd,-1,getdate()) AND getdate()
	GROUP BY CodeBase, Criticality' 
END
GO
/****** Object:  StoredProcedure [dbo].[rspGetCodeDebtSummaryByCodeBaseType]    Script Date: 9/10/2014 6:58:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[rspGetCodeDebtSummaryByCodeBaseType]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROC [dbo].[rspGetCodeDebtSummaryByCodeBaseType]
AS

	SELECT     CodeBase, Type, Criticality, SUM(Estimate) Estimates
	FROM         CodeDebt
	WHERE LastUpdatedDate BETWEEN DateAdd(dd,-1,getdate()) AND getdate()
	GROUP BY CodeBase, Type, Criticality' 
END
GO
/****** Object:  StoredProcedure [dbo].[rspGetCodeDebtSummaryByInitialsCodeBase]    Script Date: 9/10/2014 6:58:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[rspGetCodeDebtSummaryByInitialsCodeBase]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROC [dbo].[rspGetCodeDebtSummaryByInitialsCodeBase]
AS

	SELECT     Initials, CodeBase, Criticality, SUM(Estimate) Estimates
	FROM         CodeDebt
	WHERE LastUpdatedDate BETWEEN DateAdd(dd,-1,getdate()) AND getdate()
	GROUP BY Initials, CodeBase, Criticality' 
END
GO
/****** Object:  Table [dbo].[CodeDebt]    Script Date: 9/10/2014 6:58:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CodeDebt]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CodeDebt](
	[CodeDebtRowId] [int] IDENTITY(1,1) NOT NULL,
	[LastUpdatedDate] [datetime] NOT NULL CONSTRAINT [DF_CodeDebt_LastUpdatedDate]  DEFAULT (getdate()),
	[LastUpdatedBy] [varchar](50) NOT NULL CONSTRAINT [DF_CodeDebt_LastUpdatedBy]  DEFAULT (user_name()),
	[CodeBase] [varchar](255) NULL,
	[SourceFile] [varchar](255) NULL,
	[SourcePath] [varchar](255) NULL,
	[Type] [varchar](8) NULL,
	[Initials] [varchar](8) NULL,
	[Criticality] [char](1) NULL,
	[Estimate] [numeric](8, 2) NULL,
	[Comment] [varchar](2048) NULL,
	[LineNumber] [int] NULL,
	[LineCount] [int] NULL,
 CONSTRAINT [PK_CodeDebt] PRIMARY KEY CLUSTERED 
(
	[CodeDebtRowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
USE [master]
GO
ALTER DATABASE [CodeBaseData] SET  READ_WRITE 
GO
