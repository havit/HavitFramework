using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Havit.EFCoreTests.Entity.Migrations
{
	/// <inheritdoc />
	public partial class Migration_Role_Fix : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_Role
	(
	Id int NOT NULL,
	Name nvarchar(MAX) NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Role SET (LOCK_ESCALATION = TABLE)
GO
IF EXISTS(SELECT * FROM dbo.Role)
	 EXEC('INSERT INTO dbo.Tmp_Role (Id, Name)
		SELECT Id, Name FROM dbo.Role WITH (HOLDLOCK TABLOCKX)')
GO
ALTER TABLE dbo.User_AdditionalRoles
	DROP CONSTRAINT FK_User_AdditionalRoles_Role_RoleId
GO
ALTER TABLE dbo.User_PrimaryRoles
	DROP CONSTRAINT FK_User_PrimaryRoles_Role_RoleId
GO
ALTER TABLE dbo.User_SecondaryRoles
	DROP CONSTRAINT FK_User_SecondaryRoles_Role_RoleId
GO
DROP TABLE dbo.Role
GO
EXECUTE sp_rename N'dbo.Tmp_Role', N'Role', 'OBJECT' 
GO
ALTER TABLE dbo.Role ADD CONSTRAINT
	PK_Role PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.User_SecondaryRoles ADD CONSTRAINT
	FK_User_SecondaryRoles_Role_RoleId FOREIGN KEY
	(
	RoleId
	) REFERENCES dbo.Role
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.User_SecondaryRoles SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.User_PrimaryRoles ADD CONSTRAINT
	FK_User_PrimaryRoles_Role_RoleId FOREIGN KEY
	(
	RoleId
	) REFERENCES dbo.Role
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.User_PrimaryRoles SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.User_AdditionalRoles ADD CONSTRAINT
	FK_User_AdditionalRoles_Role_RoleId FOREIGN KEY
	(
	RoleId
	) REFERENCES dbo.Role
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.User_AdditionalRoles SET (LOCK_ESCALATION = TABLE)
GO
COMMIT");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			throw new NotSupportedException();
		}
	}
}
