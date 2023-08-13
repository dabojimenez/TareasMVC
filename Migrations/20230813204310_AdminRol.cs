using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TareasMVC.Migrations
{
    /// <inheritdoc />
    public partial class AdminRol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"if not exists(select id from AspNetRoles where id = 'd24e6049-c2aa-48c8-bf60-239160234628')
                                        begin
	                                        insert AspNetRoles (id, Name, NormalizedName)
	                                        values ('d24e6049-c2aa-48c8-bf60-239160234628', 'admin', 'ADMIN')
                                        end");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"delete from AspNetRoles where id = 'd24e6049-c2aa-48c8-bf60-239160234628'");
        }
    }
}
