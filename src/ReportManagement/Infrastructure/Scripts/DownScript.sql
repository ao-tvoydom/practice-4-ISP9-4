USE [ReportDB]
GO
ALTER TABLE [dbo].[Sale] DROP CONSTRAINT [FK_Sale_Product]
GO
ALTER TABLE [dbo].[Sale] DROP CONSTRAINT [FK_Sale_Department]
GO
ALTER TABLE [dbo].[Sale] DROP CONSTRAINT [FK_Sale_BlockStatus]
GO
ALTER TABLE [dbo].[Product] DROP CONSTRAINT [FK_Product_Unit]
GO
ALTER TABLE [dbo].[Product] DROP CONSTRAINT [FK_Product_Section]
GO
ALTER TABLE [dbo].[Product] DROP CONSTRAINT [FK_Product_ProductStatus]
GO
ALTER TABLE [dbo].[Product] DROP CONSTRAINT [FK_Product_Brand]
GO
/****** Object:  Table [dbo].[Unit]    Script Date: 23.12.2021 15:30:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Unit]') AND type in (N'U'))
DROP TABLE [dbo].[Unit]
GO
/****** Object:  Table [dbo].[Section]    Script Date: 23.12.2021 15:30:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Section]') AND type in (N'U'))
DROP TABLE [dbo].[Section]
GO
/****** Object:  Table [dbo].[ProductStatus]    Script Date: 23.12.2021 15:30:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductStatus]') AND type in (N'U'))
DROP TABLE [dbo].[ProductStatus]
GO
/****** Object:  Table [dbo].[BlockStatus]    Script Date: 23.12.2021 15:30:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BlockStatus]') AND type in (N'U'))
DROP TABLE [dbo].[BlockStatus]
GO
/****** Object:  View [dbo].[PivotView]    Script Date: 23.12.2021 15:30:18 ******/
DROP VIEW [dbo].[PivotView]
GO
/****** Object:  View [dbo].[MainView]    Script Date: 23.12.2021 15:30:18 ******/
DROP VIEW [dbo].[MainView]
GO
/****** Object:  Table [dbo].[Sale]    Script Date: 23.12.2021 15:30:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Sale]') AND type in (N'U'))
DROP TABLE [dbo].[Sale]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 23.12.2021 15:30:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Product]') AND type in (N'U'))
DROP TABLE [dbo].[Product]
GO
/****** Object:  Table [dbo].[Brand]    Script Date: 23.12.2021 15:30:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Brand]') AND type in (N'U'))
DROP TABLE [dbo].[Brand]
GO
/****** Object:  Table [dbo].[Department]    Script Date: 23.12.2021 15:30:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Department]') AND type in (N'U'))
DROP TABLE [dbo].[Department]
GO
USE [master]
GO
/****** Object:  Database [ReportDB]    Script Date: 23.12.2021 15:30:18 ******/
DROP DATABASE [ReportDB]
GO
