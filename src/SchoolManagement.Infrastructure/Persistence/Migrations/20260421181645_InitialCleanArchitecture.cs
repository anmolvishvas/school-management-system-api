using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCleanArchitecture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Idempotent: fresh installs and existing legacy SchoolDB (tables already present).
            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[dbo].[Students]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [dbo].[Students] (
                        [Id] int NOT NULL IDENTITY(1,1),
                        [Name] nvarchar(200) NOT NULL,
                        [Class] nvarchar(50) NOT NULL,
                        [Section] nvarchar(50) NOT NULL,
                        [Email] nvarchar(256) NULL,
                        [Phone] nvarchar(30) NULL,
                        [DateOfBirth] date NULL,
                        [AddressLine1] nvarchar(200) NULL,
                        [AddressLine2] nvarchar(200) NULL,
                        [City] nvarchar(100) NULL,
                        [State] nvarchar(100) NULL,
                        [PostalCode] nvarchar(20) NULL,
                        [ParentGuardianName] nvarchar(200) NULL,
                        [ParentGuardianPhone] nvarchar(30) NULL,
                        [ParentGuardianEmail] nvarchar(256) NULL,
                        [AdmissionNumber] nvarchar(50) NULL,
                        [DateOfAdmission] date NULL,
                        [EmergencyContact] nvarchar(200) NULL,
                        [Notes] nvarchar(2000) NULL,
                        [IsActive] bit NOT NULL CONSTRAINT [DF_Students_IsActive_Initial] DEFAULT (1),
                        CONSTRAINT [PK_Students] PRIMARY KEY ([Id])
                    );
                END
                ELSE
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Students') AND name = N'Email')
                        ALTER TABLE [dbo].[Students] ADD [Email] nvarchar(256) NULL;
                    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Students') AND name = N'Phone')
                        ALTER TABLE [dbo].[Students] ADD [Phone] nvarchar(30) NULL;
                    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Students') AND name = N'DateOfBirth')
                        ALTER TABLE [dbo].[Students] ADD [DateOfBirth] date NULL;
                    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Students') AND name = N'AddressLine1')
                        ALTER TABLE [dbo].[Students] ADD [AddressLine1] nvarchar(200) NULL;
                    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Students') AND name = N'AddressLine2')
                        ALTER TABLE [dbo].[Students] ADD [AddressLine2] nvarchar(200) NULL;
                    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Students') AND name = N'City')
                        ALTER TABLE [dbo].[Students] ADD [City] nvarchar(100) NULL;
                    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Students') AND name = N'State')
                        ALTER TABLE [dbo].[Students] ADD [State] nvarchar(100) NULL;
                    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Students') AND name = N'PostalCode')
                        ALTER TABLE [dbo].[Students] ADD [PostalCode] nvarchar(20) NULL;
                    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Students') AND name = N'ParentGuardianName')
                        ALTER TABLE [dbo].[Students] ADD [ParentGuardianName] nvarchar(200) NULL;
                    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Students') AND name = N'ParentGuardianPhone')
                        ALTER TABLE [dbo].[Students] ADD [ParentGuardianPhone] nvarchar(30) NULL;
                    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Students') AND name = N'ParentGuardianEmail')
                        ALTER TABLE [dbo].[Students] ADD [ParentGuardianEmail] nvarchar(256) NULL;
                    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Students') AND name = N'AdmissionNumber')
                        ALTER TABLE [dbo].[Students] ADD [AdmissionNumber] nvarchar(50) NULL;
                    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Students') AND name = N'DateOfAdmission')
                        ALTER TABLE [dbo].[Students] ADD [DateOfAdmission] date NULL;
                    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Students') AND name = N'EmergencyContact')
                        ALTER TABLE [dbo].[Students] ADD [EmergencyContact] nvarchar(200) NULL;
                    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Students') AND name = N'Notes')
                        ALTER TABLE [dbo].[Students] ADD [Notes] nvarchar(2000) NULL;
                    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Students') AND name = N'IsActive')
                        ALTER TABLE [dbo].[Students] ADD [IsActive] bit NOT NULL CONSTRAINT [DF_Students_IsActive_Legacy] DEFAULT (1);
                END
                """);

            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[dbo].[Users]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [dbo].[Users] (
                        [Id] int NOT NULL IDENTITY(1,1),
                        [Username] nvarchar(100) NOT NULL,
                        [PasswordHash] nvarchar(200) NOT NULL,
                        [Role] nvarchar(50) NOT NULL,
                        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
                    );
                END
                ELSE
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Users') AND name = N'Username')
                        ALTER TABLE [dbo].[Users] ADD [Username] nvarchar(100) NOT NULL;
                    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Users') AND name = N'PasswordHash')
                        ALTER TABLE [dbo].[Users] ADD [PasswordHash] nvarchar(200) NOT NULL;
                    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Users') AND name = N'Role')
                        ALTER TABLE [dbo].[Users] ADD [Role] nvarchar(50) NOT NULL;
                END

                /* Unique index requires fixed-length (or limited) types — legacy EF often used nvarchar(max). Error 1919 otherwise. */
                IF OBJECT_ID(N'[dbo].[Users]', N'U') IS NOT NULL
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM sys.columns c
                        INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
                        WHERE c.object_id = OBJECT_ID(N'dbo.Users') AND c.name = N'Username'
                          AND (ty.name IN (N'text', N'ntext')
                               OR (ty.name IN (N'varchar', N'nvarchar') AND c.max_length = -1)))
                        ALTER TABLE [dbo].[Users] ALTER COLUMN [Username] nvarchar(100) NOT NULL;

                    IF EXISTS (
                        SELECT 1
                        FROM sys.columns c
                        INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
                        WHERE c.object_id = OBJECT_ID(N'dbo.Users') AND c.name = N'PasswordHash'
                          AND (ty.name IN (N'text', N'ntext')
                               OR (ty.name IN (N'varchar', N'nvarchar') AND c.max_length = -1)))
                        ALTER TABLE [dbo].[Users] ALTER COLUMN [PasswordHash] nvarchar(200) NOT NULL;

                    IF EXISTS (
                        SELECT 1
                        FROM sys.columns c
                        INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
                        WHERE c.object_id = OBJECT_ID(N'dbo.Users') AND c.name = N'Role'
                          AND (ty.name IN (N'text', N'ntext')
                               OR (ty.name IN (N'varchar', N'nvarchar') AND c.max_length = -1)))
                        ALTER TABLE [dbo].[Users] ALTER COLUMN [Role] nvarchar(50) NOT NULL;
                END

                IF NOT EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE name = N'IX_Users_Username' AND object_id = OBJECT_ID(N'dbo.Users'))
                BEGIN
                    CREATE UNIQUE INDEX [IX_Users_Username] ON [dbo].[Users]([Username]);
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
