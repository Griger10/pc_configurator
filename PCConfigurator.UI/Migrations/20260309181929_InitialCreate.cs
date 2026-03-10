using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCConfigurator.UI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Manufacturers",
                columns: table => new
                {
                    ManufacturerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturers", x => x.ManufacturerId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "user")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "GPUs",
                columns: table => new
                {
                    GPUId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManufacturerId = table.Column<int>(type: "int", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Memory = table.Column<int>(type: "int", nullable: false),
                    MemoryType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PowerConsumption = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GPUs", x => x.GPUId);
                    table.ForeignKey(
                        name: "FK_GPUs_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "ManufacturerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Motherboards",
                columns: table => new
                {
                    MotherboardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManufacturerId = table.Column<int>(type: "int", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Socket = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Chipset = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RAMType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaxRAM = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motherboards", x => x.MotherboardId);
                    table.ForeignKey(
                        name: "FK_Motherboards_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "ManufacturerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Processors",
                columns: table => new
                {
                    ProcessorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManufacturerId = table.Column<int>(type: "int", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Socket = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Cores = table.Column<int>(type: "int", nullable: false),
                    Frequency = table.Column<decimal>(type: "decimal(4,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Processors", x => x.ProcessorId);
                    table.ForeignKey(
                        name: "FK_Processors_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "ManufacturerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RAM",
                columns: table => new
                {
                    RAMId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManufacturerId = table.Column<int>(type: "int", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Frequency = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RAM", x => x.RAMId);
                    table.ForeignKey(
                        name: "FK_RAM_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "ManufacturerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Storage",
                columns: table => new
                {
                    StorageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManufacturerId = table.Column<int>(type: "int", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Interface = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Storage", x => x.StorageId);
                    table.ForeignKey(
                        name: "FK_Storage_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "ManufacturerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Configurations",
                columns: table => new
                {
                    ConfigurationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProcessorId = table.Column<int>(type: "int", nullable: false),
                    MotherboardId = table.Column<int>(type: "int", nullable: false),
                    GPUId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configurations", x => x.ConfigurationId);
                    table.ForeignKey(
                        name: "FK_Configurations_GPUs_GPUId",
                        column: x => x.GPUId,
                        principalTable: "GPUs",
                        principalColumn: "GPUId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Configurations_Motherboards_MotherboardId",
                        column: x => x.MotherboardId,
                        principalTable: "Motherboards",
                        principalColumn: "MotherboardId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Configurations_Processors_ProcessorId",
                        column: x => x.ProcessorId,
                        principalTable: "Processors",
                        principalColumn: "ProcessorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Configurations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationRAM",
                columns: table => new
                {
                    ConfigurationRAMId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConfigurationId = table.Column<int>(type: "int", nullable: false),
                    RAMId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationRAM", x => x.ConfigurationRAMId);
                    table.ForeignKey(
                        name: "FK_ConfigurationRAM_Configurations_ConfigurationId",
                        column: x => x.ConfigurationId,
                        principalTable: "Configurations",
                        principalColumn: "ConfigurationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConfigurationRAM_RAM_RAMId",
                        column: x => x.RAMId,
                        principalTable: "RAM",
                        principalColumn: "RAMId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationStorage",
                columns: table => new
                {
                    ConfigurationStorageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConfigurationId = table.Column<int>(type: "int", nullable: false),
                    StorageId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationStorage", x => x.ConfigurationStorageId);
                    table.ForeignKey(
                        name: "FK_ConfigurationStorage_Configurations_ConfigurationId",
                        column: x => x.ConfigurationId,
                        principalTable: "Configurations",
                        principalColumn: "ConfigurationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConfigurationStorage_Storage_StorageId",
                        column: x => x.StorageId,
                        principalTable: "Storage",
                        principalColumn: "StorageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationRAM_ConfigurationId",
                table: "ConfigurationRAM",
                column: "ConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationRAM_RAMId",
                table: "ConfigurationRAM",
                column: "RAMId");

            migrationBuilder.CreateIndex(
                name: "IX_Configurations_GPUId",
                table: "Configurations",
                column: "GPUId");

            migrationBuilder.CreateIndex(
                name: "IX_Configurations_MotherboardId",
                table: "Configurations",
                column: "MotherboardId");

            migrationBuilder.CreateIndex(
                name: "IX_Configurations_ProcessorId",
                table: "Configurations",
                column: "ProcessorId");

            migrationBuilder.CreateIndex(
                name: "IX_Configurations_UserId",
                table: "Configurations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationStorage_ConfigurationId",
                table: "ConfigurationStorage",
                column: "ConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationStorage_StorageId",
                table: "ConfigurationStorage",
                column: "StorageId");

            migrationBuilder.CreateIndex(
                name: "IX_GPUs_ManufacturerId",
                table: "GPUs",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Motherboards_ManufacturerId",
                table: "Motherboards",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Processors_ManufacturerId",
                table: "Processors",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_RAM_ManufacturerId",
                table: "RAM",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Storage_ManufacturerId",
                table: "Storage",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Login",
                table: "Users",
                column: "Login",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfigurationRAM");

            migrationBuilder.DropTable(
                name: "ConfigurationStorage");

            migrationBuilder.DropTable(
                name: "RAM");

            migrationBuilder.DropTable(
                name: "Configurations");

            migrationBuilder.DropTable(
                name: "Storage");

            migrationBuilder.DropTable(
                name: "GPUs");

            migrationBuilder.DropTable(
                name: "Motherboards");

            migrationBuilder.DropTable(
                name: "Processors");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Manufacturers");
        }
    }
}
