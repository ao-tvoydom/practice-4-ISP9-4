USE [ReportDB]
GO
ALTER TABLE [dbo].[Sale] DROP CONSTRAINT [FK_Sale_Product]
GO
ALTER TABLE [dbo].[Sale] DROP CONSTRAINT [FK_Sale_Department]
GO
ALTER TABLE [dbo].[Product] DROP CONSTRAINT [FK_Product_Brand]
GO
/****** Object:  View [dbo].[MainView]    Script Date: 22.12.2021 12:36:09 ******/
DROP VIEW [dbo].[MainView]
GO
/****** Object:  Table [dbo].[Sale]    Script Date: 22.12.2021 12:36:10 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Sale]') AND type in (N'U'))
DROP TABLE [dbo].[Sale]
GO
/****** Object:  Table [dbo].[Brand]    Script Date: 22.12.2021 12:36:10 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Brand]') AND type in (N'U'))
DROP TABLE [dbo].[Brand]
GO
/****** Object:  Table [dbo].[Department]    Script Date: 22.12.2021 12:36:10 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Department]') AND type in (N'U'))
DROP TABLE [dbo].[Department]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 22.12.2021 12:36:10 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Product]') AND type in (N'U'))
DROP TABLE [dbo].[Product]
GO
USE [master]
GO
/****** Object:  Database [ReportDB]    Script Date: 22.12.2021 12:36:10 ******/
DROP DATABASE [ReportDB]
GO
