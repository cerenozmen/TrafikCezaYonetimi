using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrafikCezaYonetimi.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AracCezaVeOnayGecmisi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Araclar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plaka = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    AracTipi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Marka = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Araclar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrafikCezalari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TutanakNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CezaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IhlalAciklamasi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Durum = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AracId = table.Column<int>(type: "int", nullable: false),
                    OlusturanKullaniciId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrafikCezalari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrafikCezalari_Araclar_AracId",
                        column: x => x.AracId,
                        principalTable: "Araclar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrafikCezalari_AspNetUsers_OlusturanKullaniciId",
                        column: x => x.OlusturanKullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OnayGecmisleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrafikCezasiId = table.Column<int>(type: "int", nullable: false),
                    KullaniciId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IslemTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IslemTipi = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OncekiDurum = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    YeniDurum = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnayGecmisleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnayGecmisleri_AspNetUsers_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OnayGecmisleri_TrafikCezalari_TrafikCezasiId",
                        column: x => x.TrafikCezasiId,
                        principalTable: "TrafikCezalari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Araclar_Plaka",
                table: "Araclar",
                column: "Plaka",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OnayGecmisleri_KullaniciId",
                table: "OnayGecmisleri",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_OnayGecmisleri_TrafikCezasiId",
                table: "OnayGecmisleri",
                column: "TrafikCezasiId");

            migrationBuilder.CreateIndex(
                name: "IX_TrafikCezalari_AracId",
                table: "TrafikCezalari",
                column: "AracId");

            migrationBuilder.CreateIndex(
                name: "IX_TrafikCezalari_Durum",
                table: "TrafikCezalari",
                column: "Durum");

            migrationBuilder.CreateIndex(
                name: "IX_TrafikCezalari_OlusturanKullaniciId",
                table: "TrafikCezalari",
                column: "OlusturanKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_TrafikCezalari_TutanakNo",
                table: "TrafikCezalari",
                column: "TutanakNo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OnayGecmisleri");

            migrationBuilder.DropTable(
                name: "TrafikCezalari");

            migrationBuilder.DropTable(
                name: "Araclar");
        }
    }
}
