using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceGraduation.Infrastrucutre.Migrations
{
    /// <inheritdoc />
    public partial class RemovedCartTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brand",
                columns: table => new
                {
                    BrandCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BrandName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brand", x => x.BrandCode);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Category__371BA954FEB97F11", x => x.CategoryCode);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    CustomerCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    UserType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.CustomerCode);
                });

            migrationBuilder.CreateTable(
                name: "SubCategory",
                columns: table => new
                {
                    SubCategoryCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CategoryCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SubCateg__E7EB078530608626", x => x.SubCategoryCode);
                    table.ForeignKey(
                        name: "FK_SubCategory_Category",
                        column: x => x.CategoryCode,
                        principalTable: "Category",
                        principalColumn: "CategoryCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    LogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TableName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChangedBy = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ChangeDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AuditLog__5E5499A88188D4A4", x => x.LogID);
                    table.ForeignKey(
                        name: "FK__AuditLog__Change__4C6B5938",
                        column: x => x.ChangedBy,
                        principalTable: "Customer",
                        principalColumn: "CustomerCode",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderNumber = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    OrderStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PaymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CustomerCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: true, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Orders__CAC5E742C8EA43C2", x => x.OrderNumber);
                    table.ForeignKey(
                        name: "FK_Orders_Customer",
                        column: x => x.CustomerCode,
                        principalTable: "Customer",
                        principalColumn: "CustomerCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Weight = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    CategoryCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SubCategoryCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BrandCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Product__B40CC6EDFA1F3B04", x => x.ProductID);
                    table.ForeignKey(
                        name: "FK_Product_Brand",
                        column: x => x.BrandCode,
                        principalTable: "Brand",
                        principalColumn: "BrandCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Product_Category",
                        column: x => x.CategoryCode,
                        principalTable: "Category",
                        principalColumn: "CategoryCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Product_SubCategory",
                        column: x => x.SubCategoryCode,
                        principalTable: "SubCategory",
                        principalColumn: "SubCategoryCode");
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    InvoiceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    CustomerCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TotalBeforeDiscount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    DiscountPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    ShippingValue = table.Column<decimal>(type: "decimal(10,2)", nullable: true, defaultValue: 50m),
                    InvoiceDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    TotalDiscount = table.Column<decimal>(type: "decimal(20,8)", nullable: true, computedColumnSql: "([TotalBeforeDiscount]*([DiscountPercent]/(100)))", stored: true),
                    TotalAfterDiscount = table.Column<decimal>(type: "decimal(21,8)", nullable: true, computedColumnSql: "([TotalBeforeDiscount]-[TotalBeforeDiscount]*([DiscountPercent]/(100)))", stored: true),
                    Total = table.Column<decimal>(type: "decimal(22,8)", nullable: true, computedColumnSql: "(([TotalBeforeDiscount]-[TotalBeforeDiscount]*([DiscountPercent]/(100)))+[ShippingValue])", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Invoices__D796AAD549EF6932", x => x.InvoiceID);
                    table.ForeignKey(
                        name: "FK__Invoices__Custom__671F4F74",
                        column: x => x.CustomerCode,
                        principalTable: "Customer",
                        principalColumn: "CustomerCode");
                    table.ForeignKey(
                        name: "FK__Invoices__OrderN__662B2B3B",
                        column: x => x.OrderNumber,
                        principalTable: "Orders",
                        principalColumn: "OrderNumber");
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    PaymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TransactionID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    PaymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OrderNumber = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payment__9B556A58B5CEF764", x => x.PaymentID);
                    table.ForeignKey(
                        name: "fk_payment_orders",
                        column: x => x.OrderNumber,
                        principalTable: "Orders",
                        principalColumn: "OrderNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Shipping",
                columns: table => new
                {
                    ShippingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ShippingMethod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Carrier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrackingNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ShippingStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EstimatedDeliveryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    OrderNumber = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Shipping__5FACD46094216DD1", x => x.ShippingID);
                    table.ForeignKey(
                        name: "fk_shipping_orders",
                        column: x => x.OrderNumber,
                        principalTable: "Orders",
                        principalColumn: "OrderNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    OrderDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: true, defaultValue: 0m),
                    DiscountValue = table.Column<decimal>(type: "decimal(20,8)", nullable: true, computedColumnSql: "(([Price]*[DiscountPercent])/(100))", stored: true),
                    FinalPrice = table.Column<decimal>(type: "decimal(21,8)", nullable: true, computedColumnSql: "([Price]-([Price]*[DiscountPercent])/(100))", stored: true),
                    OrderNumber = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    SubTotal = table.Column<double>(type: "float", nullable: true, computedColumnSql: "([Quantity]*([Price]-([Price]*[DiscountPercent])/(100)))", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__OrderDet__D3B9D30C8E4F1F3C", x => x.OrderDetailID);
                    table.ForeignKey(
                        name: "FK__OrderDeta__Produ__68487DD7",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_orderdetail_orders",
                        column: x => x.OrderNumber,
                        principalTable: "Orders",
                        principalColumn: "OrderNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductImage",
                columns: table => new
                {
                    ImageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: true),
                    ImageURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProductI__7516F4EC3EA58CB2", x => x.ImageID);
                    table.ForeignKey(
                        name: "FK__ProductIm__Produ__5EBF139D",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductReview",
                columns: table => new
                {
                    ReviewID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    ReviewText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    CustomerCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProductR__74BC79AEA53AB25F", x => x.ReviewID);
                    table.ForeignKey(
                        name: "FK_Reviews_Customer",
                        column: x => x.CustomerCode,
                        principalTable: "Customer",
                        principalColumn: "CustomerCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__ProductRe__Produ__7E37BEF6",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_ChangedBy",
                table: "AuditLog",
                column: "ChangedBy");

            migrationBuilder.CreateIndex(
                name: "UQ__Brand__2206CE9B6FC305AD",
                table: "Brand",
                column: "BrandName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Brand__44292CC74BAD8DC1",
                table: "Brand",
                column: "BrandCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Category__737584F6BA133EFE",
                table: "Category",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IDX_Customer_Email",
                table: "Customer",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "UQ__Customer__066785210EEA4C72",
                table: "Customer",
                column: "CustomerCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Customer__A9D105346050AA0D",
                table: "Customer",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CustomerCode",
                table: "Invoices",
                column: "CustomerCode");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_OrderNumber",
                table: "Invoices",
                column: "OrderNumber");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_OrderNumber",
                table: "OrderDetail",
                column: "OrderNumber");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_ProductID",
                table: "OrderDetail",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IDX_Order_Date",
                table: "Orders",
                column: "OrderDate");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerCode",
                table: "Orders",
                column: "CustomerCode");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_OrderNumber",
                table: "Payment",
                column: "OrderNumber");

            migrationBuilder.CreateIndex(
                name: "UQ__Payment__55433A4A975A59E3",
                table: "Payment",
                column: "TransactionID",
                unique: true,
                filter: "[TransactionID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IDX_Product_Barcode",
                table: "Product",
                column: "Barcode");

            migrationBuilder.CreateIndex(
                name: "IX_Product_BrandCode",
                table: "Product",
                column: "BrandCode");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CategoryCode",
                table: "Product",
                column: "CategoryCode");

            migrationBuilder.CreateIndex(
                name: "IX_Product_SubCategoryCode",
                table: "Product",
                column: "SubCategoryCode");

            migrationBuilder.CreateIndex(
                name: "UQ__Product__177800D3C1F80B60",
                table: "Product",
                column: "Barcode",
                unique: true,
                filter: "[Barcode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImage_ProductID",
                table: "ProductImage",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReview_CustomerCode",
                table: "ProductReview",
                column: "CustomerCode");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReview_ProductID",
                table: "ProductReview",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IDX_Shipping_Tracking",
                table: "Shipping",
                column: "TrackingNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Shipping_OrderNumber",
                table: "Shipping",
                column: "OrderNumber");

            migrationBuilder.CreateIndex(
                name: "UQ__Shipping__784DB3D9105751C4",
                table: "Shipping",
                column: "TrackingNumber",
                unique: true,
                filter: "[TrackingNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_CategoryCode",
                table: "SubCategory",
                column: "CategoryCode");

            migrationBuilder.CreateIndex(
                name: "UQ__SubCateg__737584F670BC1034",
                table: "SubCategory",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLog");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "ProductImage");

            migrationBuilder.DropTable(
                name: "ProductReview");

            migrationBuilder.DropTable(
                name: "Shipping");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Brand");

            migrationBuilder.DropTable(
                name: "SubCategory");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Category");
        }
    }
}
