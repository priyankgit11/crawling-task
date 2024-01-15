USE [master]
GO

/****** Object:  Database [Ineichen]    Script Date: 1/15/2024 7:16:00 PM ******/
CREATE DATABASE [Ineichen]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Ineichen', FILENAME = N'C:\Users\DELL\Ineichen.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Ineichen_log', FILENAME = N'C:\Users\DELL\Ineichen_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Ineichen].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [Ineichen] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [Ineichen] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [Ineichen] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [Ineichen] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [Ineichen] SET ARITHABORT OFF 
GO

ALTER DATABASE [Ineichen] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [Ineichen] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [Ineichen] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [Ineichen] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [Ineichen] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [Ineichen] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [Ineichen] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [Ineichen] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [Ineichen] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [Ineichen] SET  DISABLE_BROKER 
GO

ALTER DATABASE [Ineichen] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [Ineichen] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [Ineichen] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [Ineichen] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [Ineichen] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [Ineichen] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [Ineichen] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [Ineichen] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [Ineichen] SET  MULTI_USER 
GO

ALTER DATABASE [Ineichen] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [Ineichen] SET DB_CHAINING OFF 
GO

ALTER DATABASE [Ineichen] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [Ineichen] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [Ineichen] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [Ineichen] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO

ALTER DATABASE [Ineichen] SET QUERY_STORE = OFF
GO

ALTER DATABASE [Ineichen] SET  READ_WRITE 
GO


