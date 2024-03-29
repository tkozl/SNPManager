USE [master]
GO
/****** Object:  Database [snpmanager]    Script Date: 06.01.2024 14:29:30 ******/
CREATE DATABASE [snpmanager]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'snpmanager', FILENAME = N'/var/opt/mssql/data/snpmanager.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'snpmanager_log', FILENAME = N'/var/opt/mssql/data/snpmanager_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [snpmanager] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [snpmanager].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [snpmanager] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [snpmanager] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [snpmanager] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [snpmanager] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [snpmanager] SET ARITHABORT OFF 
GO
ALTER DATABASE [snpmanager] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [snpmanager] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [snpmanager] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [snpmanager] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [snpmanager] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [snpmanager] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [snpmanager] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [snpmanager] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [snpmanager] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [snpmanager] SET  DISABLE_BROKER 
GO
ALTER DATABASE [snpmanager] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [snpmanager] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [snpmanager] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [snpmanager] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [snpmanager] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [snpmanager] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [snpmanager] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [snpmanager] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [snpmanager] SET  MULTI_USER 
GO
ALTER DATABASE [snpmanager] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [snpmanager] SET DB_CHAINING OFF 
GO
ALTER DATABASE [snpmanager] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [snpmanager] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [snpmanager] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [snpmanager] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [snpmanager] SET QUERY_STORE = OFF
GO
USE [snpmanager]
GO
/****** Object:  Table [dbo].[passwords]    Script Date: 06.01.2024 14:29:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[passwords](
	[pass_id] [int] IDENTITY(1,1) NOT NULL,
	[entry_id] [int] NOT NULL,
	[password] [varbinary](max) NOT NULL,
	[created_at] [datetime] NOT NULL,
 CONSTRAINT [PK_passwords] PRIMARY KEY CLUSTERED 
(
	[pass_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[entry_current_password]    Script Date: 06.01.2024 14:29:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[entry_current_password]
AS
SELECT entry_id, MAX(pass_id) AS pass_id
FROM     dbo.passwords
GROUP BY entry_id
GO
/****** Object:  View [dbo].[entry_old_passwords]    Script Date: 06.01.2024 14:29:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[entry_old_passwords]
AS
SELECT dbo.passwords.entry_id, dbo.passwords.pass_id, dbo.passwords.password, dbo.passwords.created_at
FROM     dbo.entry_current_password RIGHT OUTER JOIN
                  dbo.passwords ON dbo.entry_current_password.pass_id = dbo.passwords.pass_id
WHERE  (dbo.entry_current_password.pass_id IS NULL)
GO
/****** Object:  Table [dbo].[activity_log]    Script Date: 06.01.2024 14:29:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[activity_log](
	[activity_id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[activity_type_id] [int] NOT NULL,
	[ocurred_at] [datetime] NOT NULL,
	[ip] [varbinary](max) NULL,
	[obfuscated_ip] [text] NULL,
 CONSTRAINT [PK_activity_log] PRIMARY KEY CLUSTERED 
(
	[activity_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[last_incorrect_logins]    Script Date: 06.01.2024 14:29:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[last_incorrect_logins]
AS
SELECT dbo.activity_log.user_id, dbo.activity_log.activity_id, dbo.activity_log.ocurred_at
FROM     dbo.activity_log LEFT JOIN
                      (SELECT t .user_id, t .activity_id, t .ocurred_at
                       FROM      (SELECT *, ROW_NUMBER() OVER (PARTITION BY user_id
                                          ORDER BY ocurred_at DESC) AS row_number
                       FROM      dbo.activity_log
                       WHERE   activity_type_id = 1) t
WHERE  t .row_number = 1) last_login ON last_login.user_id = dbo.activity_log.user_id
WHERE  dbo.activity_log.activity_type_id = 2 AND dbo.activity_log.ocurred_at > last_login.ocurred_at
GO
/****** Object:  View [dbo].[incorrect_logins_24h]    Script Date: 06.01.2024 14:29:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[incorrect_logins_24h]
AS
SELECT user_id, COUNT(*) AS incorrect_logins_quantity
FROM     dbo.last_incorrect_logins
WHERE  (DATEDIFF(hour, GETDATE(), ocurred_at) < 24)
GROUP BY user_id
GO
/****** Object:  View [dbo].[locked_users]    Script Date: 06.01.2024 14:29:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[locked_users]
AS
SELECT last_lock.user_id
FROM     (SELECT t .user_id, t .ocurred_at
                  FROM      (SELECT *, ROW_NUMBER() OVER (PARTITION BY user_id
                                     ORDER BY ocurred_at DESC) AS row_number
                  FROM      dbo.activity_log
                  WHERE   activity_type_id = 3) t
WHERE  t .row_number = 1) last_lock LEFT JOIN
    (SELECT t .user_id, t .ocurred_at
     FROM      (SELECT *, ROW_NUMBER() OVER (PARTITION BY user_id
                        ORDER BY ocurred_at DESC) AS row_number
     FROM      dbo.activity_log
     WHERE   activity_type_id = 4) t
WHERE  t .row_number = 1) last_unlock ON last_lock.user_id = last_unlock.user_id
WHERE  DATEDIFF(hour, last_lock.ocurred_at, GETDATE()) < 1 AND last_unlock.ocurred_at < last_lock.ocurred_at
GO
/****** Object:  Table [dbo].[users]    Script Date: 06.01.2024 14:29:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[user_id] [int] IDENTITY(1,1) NOT NULL,
	[email_hash] [varbinary](max) NOT NULL,
	[encrypted_email] [varbinary](max) NOT NULL,
	[created_at] [datetime] NOT NULL,
	[deleted_at] [datetime] NULL,
	[email_verified] [bit] NOT NULL,
	[secret_2fa] [text] NULL,
	[encryption_type_id] [int] NOT NULL,
	[email_verify_token] [varchar](max) NULL,
	[email_verify_token_exp] [datetime] NULL,
	[user_del_token] [varchar](max) NULL,
	[user_del_token_exp] [datetime] NULL,
	[user_unlock_token] [varchar](max) NULL,
 CONSTRAINT [PK_users] PRIMARY KEY CLUSTERED 
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[unlocked_users]    Script Date: 06.01.2024 14:29:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[unlocked_users]
AS
SELECT dbo.users.user_id
FROM     dbo.users LEFT OUTER JOIN
                  dbo.locked_users ON dbo.users.user_id = dbo.locked_users.user_id
WHERE  (dbo.locked_users.user_id IS NULL)
GO
/****** Object:  Table [dbo].[special_directories]    Script Date: 06.01.2024 14:29:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[special_directories](
	[special_directory_id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[special_directory_type_id] [int] NOT NULL,
 CONSTRAINT [PK_special_directories_1] PRIMARY KEY CLUSTERED 
(
	[special_directory_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[directories]    Script Date: 06.01.2024 14:29:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[directories](
	[directory_id] [int] IDENTITY(1,1) NOT NULL,
	[special_directory_id] [int] NOT NULL,
	[parent_id] [int] NULL,
	[directory_name] [varbinary](max) NOT NULL,
	[deleted_at] [datetime] NULL,
	[deleted_by] [varbinary](max) NULL,
	[moved_at] [varbinary](max) NULL,
 CONSTRAINT [PK_directories] PRIMARY KEY CLUSTERED 
(
	[directory_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[users_directories]    Script Date: 06.01.2024 14:29:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[users_directories]
AS
SELECT dbo.special_directories.user_id, dbo.directories.directory_id, dbo.directories.special_directory_id, dbo.directories.parent_id, dbo.directories.directory_name, dbo.directories.deleted_at, dbo.directories.deleted_by, 
                  dbo.directories.moved_at
FROM     dbo.directories INNER JOIN
                  dbo.special_directories ON dbo.directories.special_directory_id = dbo.special_directories.special_directory_id
GO
/****** Object:  Table [dbo].[entries]    Script Date: 06.01.2024 14:29:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[entries](
	[entry_id] [int] IDENTITY(1,1) NOT NULL,
	[directory_id] [int] NULL,
	[special_directory_id] [int] NOT NULL,
	[entry_name] [varbinary](max) NOT NULL,
	[username] [varbinary](max) NOT NULL,
	[note] [varbinary](max) NOT NULL,
	[created_at] [varbinary](max) NOT NULL,
	[pass_lifetime] [varbinary](max) NOT NULL,
	[deleted_at] [datetime] NULL,
	[deleted_by] [varchar](max) NULL,
	[moved_at] [varbinary](max) NULL,
 CONSTRAINT [PK_entries] PRIMARY KEY CLUSTERED 
(
	[entry_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[users_entries]    Script Date: 06.01.2024 14:29:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[users_entries]
AS
SELECT dbo.special_directories.user_id, dbo.entries.entry_id, dbo.entries.directory_id, dbo.entries.special_directory_id, dbo.entries.entry_name, dbo.entries.username, dbo.entries.note, dbo.entries.created_at, dbo.entries.pass_lifetime, 
                  dbo.entries.deleted_at, dbo.entries.deleted_by, dbo.entries.moved_at
FROM     dbo.special_directories INNER JOIN
                  dbo.entries ON dbo.special_directories.special_directory_id = dbo.entries.special_directory_id
GO
/****** Object:  View [dbo].[entry_user_password]    Script Date: 06.01.2024 14:29:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[entry_user_password]
AS
SELECT dbo.passwords.password, dbo.users_entries.user_id, dbo.users_entries.entry_id, dbo.users_entries.directory_id, dbo.users_entries.special_directory_id, dbo.users_entries.entry_name, dbo.users_entries.username, 
                  dbo.users_entries.note, dbo.users_entries.created_at AS entry_created_at, dbo.users_entries.pass_lifetime, dbo.users_entries.deleted_at, dbo.users_entries.deleted_by, dbo.passwords.created_at AS password_created_at, 
                  dbo.users_entries.moved_at
FROM     dbo.entry_current_password INNER JOIN
                  dbo.passwords ON dbo.entry_current_password.pass_id = dbo.passwords.pass_id INNER JOIN
                  dbo.users_entries ON dbo.users_entries.entry_id = dbo.entry_current_password.entry_id
GO
/****** Object:  Table [dbo].[activity_type]    Script Date: 06.01.2024 14:29:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[activity_type](
	[activity_type_id] [int] IDENTITY(1,1) NOT NULL,
	[activity_name] [text] NOT NULL,
 CONSTRAINT [PK_activity_type] PRIMARY KEY CLUSTERED 
(
	[activity_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[encryption]    Script Date: 06.01.2024 14:29:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[encryption](
	[encryption_type_id] [int] IDENTITY(1,1) NOT NULL,
	[encryption_name] [text] NOT NULL,
 CONSTRAINT [PK_encryption] PRIMARY KEY CLUSTERED 
(
	[encryption_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[parameters]    Script Date: 06.01.2024 14:29:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[parameters](
	[parameter_id] [int] IDENTITY(1,1) NOT NULL,
	[entry_id] [int] NOT NULL,
	[parameter_name] [varbinary](max) NOT NULL,
	[parameter_value] [varbinary](max) NOT NULL,
	[deleted_at] [datetime] NULL,
	[deleted_by] [varbinary](max) NULL,
 CONSTRAINT [PK_parameters] PRIMARY KEY CLUSTERED 
(
	[parameter_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[related_windows]    Script Date: 06.01.2024 14:29:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[related_windows](
	[window_id] [int] IDENTITY(1,1) NOT NULL,
	[entry_id] [int] NOT NULL,
	[window_name] [varbinary](max) NOT NULL,
	[deleted_at] [datetime] NULL,
	[deleted_by] [varbinary](max) NULL,
 CONSTRAINT [PK_related_windows] PRIMARY KEY CLUSTERED 
(
	[window_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[special_directories_types]    Script Date: 06.01.2024 14:29:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[special_directories_types](
	[special_directory_type_id] [int] IDENTITY(1,1) NOT NULL,
	[special_directory_type_name] [text] NOT NULL,
 CONSTRAINT [PK_special_directories_types] PRIMARY KEY CLUSTERED 
(
	[special_directory_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_users_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[activity_log]  WITH CHECK ADD  CONSTRAINT [FK_activity_log_activity_type] FOREIGN KEY([activity_type_id])
REFERENCES [dbo].[activity_type] ([activity_type_id])
GO
ALTER TABLE [dbo].[activity_log] CHECK CONSTRAINT [FK_activity_log_activity_type]
GO
ALTER TABLE [dbo].[activity_log]  WITH CHECK ADD  CONSTRAINT [FK_activity_log_users] FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[activity_log] CHECK CONSTRAINT [FK_activity_log_users]
GO
ALTER TABLE [dbo].[directories]  WITH CHECK ADD  CONSTRAINT [FK_directories_directories] FOREIGN KEY([parent_id])
REFERENCES [dbo].[directories] ([directory_id])
GO
ALTER TABLE [dbo].[directories] CHECK CONSTRAINT [FK_directories_directories]
GO
ALTER TABLE [dbo].[directories]  WITH CHECK ADD  CONSTRAINT [FK_directories_special_directories1] FOREIGN KEY([special_directory_id])
REFERENCES [dbo].[special_directories] ([special_directory_id])
GO
ALTER TABLE [dbo].[directories] CHECK CONSTRAINT [FK_directories_special_directories1]
GO
ALTER TABLE [dbo].[entries]  WITH CHECK ADD  CONSTRAINT [FK_entries_directories] FOREIGN KEY([directory_id])
REFERENCES [dbo].[directories] ([directory_id])
GO
ALTER TABLE [dbo].[entries] CHECK CONSTRAINT [FK_entries_directories]
GO
ALTER TABLE [dbo].[entries]  WITH CHECK ADD  CONSTRAINT [FK_entries_special_directories] FOREIGN KEY([special_directory_id])
REFERENCES [dbo].[special_directories] ([special_directory_id])
GO
ALTER TABLE [dbo].[entries] CHECK CONSTRAINT [FK_entries_special_directories]
GO
ALTER TABLE [dbo].[parameters]  WITH CHECK ADD  CONSTRAINT [FK_parameters_entries] FOREIGN KEY([entry_id])
REFERENCES [dbo].[entries] ([entry_id])
GO
ALTER TABLE [dbo].[parameters] CHECK CONSTRAINT [FK_parameters_entries]
GO
ALTER TABLE [dbo].[passwords]  WITH CHECK ADD  CONSTRAINT [FK_passwords_entries] FOREIGN KEY([entry_id])
REFERENCES [dbo].[entries] ([entry_id])
GO
ALTER TABLE [dbo].[passwords] CHECK CONSTRAINT [FK_passwords_entries]
GO
ALTER TABLE [dbo].[related_windows]  WITH CHECK ADD  CONSTRAINT [FK_related_windows_entries] FOREIGN KEY([entry_id])
REFERENCES [dbo].[entries] ([entry_id])
GO
ALTER TABLE [dbo].[related_windows] CHECK CONSTRAINT [FK_related_windows_entries]
GO
ALTER TABLE [dbo].[special_directories]  WITH CHECK ADD  CONSTRAINT [FK_special_directories_special_directories_types] FOREIGN KEY([special_directory_type_id])
REFERENCES [dbo].[special_directories_types] ([special_directory_type_id])
GO
ALTER TABLE [dbo].[special_directories] CHECK CONSTRAINT [FK_special_directories_special_directories_types]
GO
ALTER TABLE [dbo].[special_directories]  WITH CHECK ADD  CONSTRAINT [FK_special_directories_users] FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[special_directories] CHECK CONSTRAINT [FK_special_directories_users]
GO
ALTER TABLE [dbo].[users]  WITH CHECK ADD  CONSTRAINT [FK_users_encryption] FOREIGN KEY([encryption_type_id])
REFERENCES [dbo].[encryption] ([encryption_type_id])
GO
ALTER TABLE [dbo].[users] CHECK CONSTRAINT [FK_users_encryption]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "passwords"
            Begin Extent = 
               Top = 7
               Left = 48
               Bottom = 170
               Right = 258
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 900
         Table = 1176
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1356
         SortOrder = 1416
         GroupBy = 1350
         Filter = 1356
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'entry_current_password'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'entry_current_password'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "entry_current_password"
            Begin Extent = 
               Top = 7
               Left = 48
               Bottom = 126
               Right = 242
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "passwords"
            Begin Extent = 
               Top = 7
               Left = 290
               Bottom = 170
               Right = 484
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1176
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1356
         SortOrder = 1416
         GroupBy = 1350
         Filter = 1356
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'entry_old_passwords'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'entry_old_passwords'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = -127
      End
      Begin Tables = 
         Begin Table = "entry_current_password"
            Begin Extent = 
               Top = 20
               Left = 438
               Bottom = 139
               Right = 632
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "passwords"
            Begin Extent = 
               Top = 139
               Left = 131
               Bottom = 302
               Right = 325
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "users_entries"
            Begin Extent = 
               Top = 175
               Left = 742
               Bottom = 338
               Right = 936
            End
            DisplayFlags = 280
            TopColumn = 8
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1176
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1356
         SortOrder = 1416
         GroupBy = 1350
         Filter = 1356
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'entry_user_password'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'entry_user_password'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "last_incorrect_logins"
            Begin Extent = 
               Top = 7
               Left = 48
               Bottom = 148
               Right = 258
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 900
         Table = 1176
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1356
         SortOrder = 1416
         GroupBy = 1350
         Filter = 1356
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'incorrect_logins_24h'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'incorrect_logins_24h'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1176
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1356
         SortOrder = 1416
         GroupBy = 1356
         Filter = 1356
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'last_incorrect_logins'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'last_incorrect_logins'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1176
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1356
         SortOrder = 1416
         GroupBy = 1350
         Filter = 1356
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'locked_users'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'locked_users'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "users"
            Begin Extent = 
               Top = 7
               Left = 48
               Bottom = 170
               Right = 295
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "locked_users"
            Begin Extent = 
               Top = 7
               Left = 343
               Bottom = 104
               Right = 537
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'unlocked_users'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'unlocked_users'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "directories"
            Begin Extent = 
               Top = 7
               Left = 48
               Bottom = 170
               Right = 273
            End
            DisplayFlags = 280
            TopColumn = 3
         End
         Begin Table = "special_directories"
            Begin Extent = 
               Top = 7
               Left = 321
               Bottom = 148
               Right = 581
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1176
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1356
         SortOrder = 1416
         GroupBy = 1350
         Filter = 1356
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_directories'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_directories'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "special_directories"
            Begin Extent = 
               Top = 31
               Left = 81
               Bottom = 172
               Right = 343
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "entries"
            Begin Extent = 
               Top = 26
               Left = 471
               Bottom = 189
               Right = 719
            End
            DisplayFlags = 280
            TopColumn = 7
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1176
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1356
         SortOrder = 1416
         GroupBy = 1350
         Filter = 1356
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_entries'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_entries'
GO
USE [master]
GO
ALTER DATABASE [snpmanager] SET  READ_WRITE 
GO
