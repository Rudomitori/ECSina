using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECSina.Db.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataEntity_DataEntity_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "DataEntity",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DataComponent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataComponent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataComponent_DataEntity_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "DataEntity",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DataComponent_DataEntity_EntityId",
                        column: x => x.EntityId,
                        principalTable: "DataEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ForumId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Message_DataEntity_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "DataEntity",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Message_DataEntity_ForumId",
                        column: x => x.ForumId,
                        principalTable: "DataEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ForumComponent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumComponent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForumComponent_DataComponent_Id",
                        column: x => x.Id,
                        principalTable: "DataComponent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HierarchyComponent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HierarchyComponent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HierarchyComponent_DataComponent_Id",
                        column: x => x.Id,
                        principalTable: "DataComponent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HierarchyComponent_DataEntity_ParentId",
                        column: x => x.ParentId,
                        principalTable: "DataEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PasswordComponent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordComponent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordComponent_DataComponent_Id",
                        column: x => x.Id,
                        principalTable: "DataComponent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolesComponent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesComponent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolesComponent_DataComponent_Id",
                        column: x => x.Id,
                        principalTable: "DataComponent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TopicComponent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicComponent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TopicComponent_DataComponent_Id",
                        column: x => x.Id,
                        principalTable: "DataComponent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserComponent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Login = table.Column<string>(type: "text", nullable: false),
                    NormalizedLogin = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserComponent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserComponent_DataComponent_Id",
                        column: x => x.Id,
                        principalTable: "DataComponent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataComponent_CreatedById",
                table: "DataComponent",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DataComponent_EntityId",
                table: "DataComponent",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_DataEntity_CreatedById",
                table: "DataEntity",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HierarchyComponent_ParentId",
                table: "HierarchyComponent",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_AuthorId",
                table: "Message",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_ForumId",
                table: "Message",
                column: "ForumId");

            migrationBuilder.CreateIndex(
                name: "IX_UserComponent_NormalizedLogin",
                table: "UserComponent",
                column: "NormalizedLogin",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ForumComponent");

            migrationBuilder.DropTable(
                name: "HierarchyComponent");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "PasswordComponent");

            migrationBuilder.DropTable(
                name: "RolesComponent");

            migrationBuilder.DropTable(
                name: "TopicComponent");

            migrationBuilder.DropTable(
                name: "UserComponent");

            migrationBuilder.DropTable(
                name: "DataComponent");

            migrationBuilder.DropTable(
                name: "DataEntity");
        }
    }
}
