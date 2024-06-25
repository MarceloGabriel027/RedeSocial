using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedeSocial.Migrations
{
    /// <inheritdoc />
    public partial class RedeSocialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    usuarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    usuarioEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    usuarioSenha = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    usuarioImagem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    usuarioNome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    usuarioTelefone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    usuarioEndereco = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    usuarioCPF = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario", x => x.usuarioId);
                });

            migrationBuilder.CreateTable(
                name: "bloqueados",
                columns: table => new
                {
                    idBloqueio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idUsuario = table.Column<int>(type: "int", nullable: false),
                    idUsuarioBloqueado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bloqueados", x => x.idBloqueio);
                    table.ForeignKey(
                        name: "FK_bloqueados_usuario_idUsuario",
                        column: x => x.idUsuario,
                        principalTable: "usuario",
                        principalColumn: "usuarioId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_bloqueados_usuario_idUsuarioBloqueado",
                        column: x => x.idUsuarioBloqueado,
                        principalTable: "usuario",
                        principalColumn: "usuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "post",
                columns: table => new
                {
                    postId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    usuarioId = table.Column<int>(type: "int", nullable: false),
                    postTxt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    postArquivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    postDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    postStatus = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post", x => x.postId);
                    table.ForeignKey(
                        name: "FK_post_usuario_usuarioId",
                        column: x => x.usuarioId,
                        principalTable: "usuario",
                        principalColumn: "usuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "comentarios",
                columns: table => new
                {
                    comentarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    usuarioId = table.Column<int>(type: "int", nullable: false),
                    postId = table.Column<int>(type: "int", nullable: false),
                    comentario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    data = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comentarios", x => x.comentarioId);
                    table.ForeignKey(
                        name: "FK_comentarios_post_postId",
                        column: x => x.postId,
                        principalTable: "post",
                        principalColumn: "postId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_comentarios_usuario_usuarioId",
                        column: x => x.usuarioId,
                        principalTable: "usuario",
                        principalColumn: "usuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bloqueados_idUsuario",
                table: "bloqueados",
                column: "idUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_bloqueados_idUsuarioBloqueado",
                table: "bloqueados",
                column: "idUsuarioBloqueado");

            migrationBuilder.CreateIndex(
                name: "IX_comentarios_postId",
                table: "comentarios",
                column: "postId");

            migrationBuilder.CreateIndex(
                name: "IX_comentarios_usuarioId",
                table: "comentarios",
                column: "usuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_post_usuarioId",
                table: "post",
                column: "usuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bloqueados");

            migrationBuilder.DropTable(
                name: "comentarios");

            migrationBuilder.DropTable(
                name: "post");

            migrationBuilder.DropTable(
                name: "usuario");
        }
    }
}
