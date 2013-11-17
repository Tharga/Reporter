using System.Drawing;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;

namespace Tharga.Reporter.Console
{
    class SkallSim1
    {
        public static Section GetDeliveryNoteSection(Color backLineColor)
        {
            //var backFieldColor = Color.FromArgb(backLineColor.A, backLineColor.R + 127, backLineColor.G + 127, backLineColor.B + 127);
            var backFieldColor = Color.White;
            //var backFieldColor = Color.FromArgb(255, 128, 128, 255);

            var section = ReportBusinessHelper.GetNoteBase(backLineColor, backFieldColor);

            #region Header

            section.Header.ElementList.Add(new Text { Value = "Följesedel", Left = UnitValue.Parse("10cm"), Top = UnitValue.Parse("0"), Font = new Engine.Entity.Font { Size = 22 } });
            section.Header.ElementList.Add(new Text { Value = "Nr:", Left = UnitValue.Parse("10cm"), Top = UnitValue.Parse("24px") });
            section.Header.ElementList.Add(new Text { Value = "{Number}", Left = UnitValue.Parse("12.5cm"), Top = UnitValue.Parse("24px") });
            section.Header.ElementList.Add(new Text { Value = "Datum:", Left = UnitValue.Parse("10cm"), Top = UnitValue.Parse("36px") });
            section.Header.ElementList.Add(new Text { Value = "{Date}", Left = UnitValue.Parse("12,5cm"), Top = UnitValue.Parse("36px") });


            #endregion
            #region Footer



            #endregion
            #region Pane


            ////Signature for the representative
            //var signRefPoint = new ReferencePoint("1cm", "18cm");
            //section.Pane.ElementList.Add(signRefPoint);
            //signRefPoint.ElementList.Add(new Line("0cm", "0cm", "8cm", "0cm", backLineColor, "1px"));
            //signRefPoint.ElementList.Add(Text.Create("{Representative.Name}", "0", "0cm"));

            section.Pane.ElementList.Add(GetDeliveryNoteTable(backLineColor, backFieldColor));

            #endregion

            return section;
        }

        private static Table GetDeliveryNoteTable(Color? backLineColor, Color? backFieldColor)
        {
            var orderItemTable = new Table
                {
                    Name = "OrderItems",
                    Height = ReportBusinessHelper.SumTop,
                    Width = UnitValue.Parse("100%"),
                    BorderColor = backLineColor ?? Color.Black,
                    BackgroundColor = backFieldColor,
                };
            //orderItemTable.AddColumn("{BoxCount.NumberOfBoxes}*{BoxCount.ItemsPerBox}", "Lådor", UnitValue.Parse("2cm"), alignment: Table.Alignment.Right, hideValue: "*");
            orderItemTable.AddColumn("{Description}", "Specifikation", widthMode: Table.WidthMode.Spring);
            orderItemTable.AddColumn("{AmountDescription}", "Lådor", UnitValue.Parse("2cm"), alignment: Table.Alignment.Right, hideValue: "");
            orderItemTable.AddColumn("{Count}", "Antal", widthMode: Table.WidthMode.Specific, width: UnitValue.Parse("3cm"), alignment: Table.Alignment.Right);
            orderItemTable.AddColumn("{NetNormalItemPrice}", "Pris", UnitValue.Parse("2cm"), alignment: Table.Alignment.Right, hideValue: "{NetSaleItemPrice}");
            orderItemTable.AddColumn("{DiscountPercentage}%", "Rabatt", UnitValue.Parse("2cm"), alignment: Table.Alignment.Right, hideValue: "0%");
            orderItemTable.AddColumn("{NetSaleItemPrice}", "Säljpris", UnitValue.Parse("2cm"), alignment: Table.Alignment.Right);
            orderItemTable.AddColumn("{NetSaleTotalPrice}", "Belopp", UnitValue.Parse("2cm"), alignment: Table.Alignment.Right);
            //orderItemTable.AddColumn("{VAT}", "Moms", UnitValue.Parse("2cm"), alignment: Table.Alignment.Right);

            return orderItemTable;
        }

    }
}