using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class StudentIsActiveDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Default is already applied in InitialCleanArchitecture (new DB + legacy ALTER). Keep migration for history alignment.
            migrationBuilder.Sql(
                """
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Students') AND name = N'IsActive')
                   AND NOT EXISTS (
                       SELECT 1 FROM sys.default_constraints dc
                       INNER JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id
                       WHERE dc.parent_object_id = OBJECT_ID(N'dbo.Students') AND c.name = N'IsActive')
                BEGIN
                    ALTER TABLE [dbo].[Students] ADD CONSTRAINT [DF_Students_IsActive] DEFAULT (CAST(1 AS bit)) FOR [IsActive];
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DECLARE @sql nvarchar(max);
                SELECT @sql = N'ALTER TABLE [dbo].[Students] DROP CONSTRAINT [' + dc.name + N'];'
                FROM sys.default_constraints dc
                INNER JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id
                WHERE dc.parent_object_id = OBJECT_ID(N'dbo.Students') AND c.name = N'IsActive' AND dc.name = N'DF_Students_IsActive';
                IF @sql IS NOT NULL EXEC sp_executesql @sql;
                """);
        }
    }
}
