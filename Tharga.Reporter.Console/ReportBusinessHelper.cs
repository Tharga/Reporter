using System.Drawing;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Entity.Element;
using Image = Tharga.Reporter.Engine.Entity.Element.Image;
using Rectangle = Tharga.Reporter.Engine.Entity.Element.Rectangle;

namespace Tharga.Reporter.Console
{
    public static class ReportBusinessHelper
    {
        public static UnitValue SumTop = UnitValue.Parse("15cm");

        internal static Section GetNoteBase(Color backLineColor, Color backFieldColor)
        {
            var section = new Section
                {
                    Margin = new UnitRectangle
                        {
                            Left = UnitValue.Parse("2cm"),
                            Right = UnitValue.Parse("1cm"),
                            Top = UnitValue.Parse("1cm"),
                            Bottom = UnitValue.Parse("0,5cm")
                        }
                };

            #region Header


            section.Header.Height = UnitValue.Parse("6cm");
            section.Header.ElementList.Add(new Rectangle { Left = UnitValue.Parse("9,5cm"), Top = UnitValue.Parse("0cm"), Width = UnitValue.Parse("8,5cm"), Height = UnitValue.Parse("2cm"), BorderColor = backLineColor, BorderWidth = UnitValue.Parse("1px"), BackgroundColor = backFieldColor });
            section.Header.ElementList.Add(new Image { Source = "{Company.Logotype}", Height = UnitValue.Parse("3,5cm"), Top = UnitValue.Parse("0cm"), IsBackground = true });

            //Customer
            var customerArea = new ReferencePoint()
                {
                    Left = UnitValue.Parse("10cm"),
                    Top = UnitValue.Parse("2,5cm"),
                    Stack = ReferencePoint.StackMethod.Vertical,
                    Name = "CustomerArea"
                };
            section.Header.ElementList.Add(customerArea);
            customerArea.ElementList.Add(new Text { Value = "Kund nr: {Customer.Number}" });
            customerArea.ElementList.Add(new Text { Value = "{Customer.Name}" });
            //customerArea.ElementList.Add(Text.Create("Att: {Customer.DeliveryAddress.Representative.Name} ({Customer.DeliveryAddress.Representative.BuyerNumber})", hideValue: "Customer.DeliveryAddress.Representative.Name"));
            customerArea.ElementList.Add(new Text { Value = "{Customer.DeliveryAddress.Note}" });
            customerArea.ElementList.Add(new Text { Value = "{Customer.DeliveryAddress.StreetName}" });
            customerArea.ElementList.Add(new Text { Value = "{Customer.DeliveryAddress.PostalCode} {Customer.DeliveryAddress.City}" });
            customerArea.ElementList.Add(new Text { Value = "{Customer.DeliveryAddress.Country}" });

            var refArea = new ReferencePoint { Left = UnitValue.Parse("3cm"), Top = UnitValue.Parse("2,5cm"), Stack = ReferencePoint.StackMethod.Vertical };
            section.Header.ElementList.Add(refArea);
            refArea.ElementList.Add(new Text { Value = "Vår Referens" });
            refArea.ElementList.Add(new Text { Value = "{User.Name}" });
            refArea.ElementList.Add(new Text { Value = " " });
            refArea.ElementList.Add(new Text { Value = "Er Referens" }); //TODO: This should be hidden if there is no value for {Customer.DeliveryAddress.Representative.Name}.
            refArea.ElementList.Add(new Text { Value = "{Customer.DeliveryAddress.Representative.Name}" });


            #endregion
            #region Footer


            const string top = "3px";
            section.Footer.Height = UnitValue.Parse("2cm");
            var addressRefPoint = new ReferencePoint { Top = UnitValue.Parse(top), Stack = ReferencePoint.StackMethod.Vertical };
            section.Footer.ElementList.Add(addressRefPoint);
            addressRefPoint.ElementList.Add(new Text { Value = "Postadress", Font = new Engine.Entity.Font { Color = backFieldColor, Size = 6 } });
            addressRefPoint.ElementList.Add(new Text { Value = "{Company.StreetName}", Font = new Engine.Entity.Font { Color = backLineColor, Size = 8 } });
            addressRefPoint.ElementList.Add(new Text { Value = "{Company.PostalCode} {Company.City}", Font = new Engine.Entity.Font { Color = backLineColor, Size = 8 } });

            var hallRefPoint = new ReferencePoint { Left = UnitValue.Parse("4cm"), Top = UnitValue.Parse(top), Stack = ReferencePoint.StackMethod.Vertical };
            section.Footer.ElementList.Add(hallRefPoint);
            hallRefPoint.ElementList.Add(new Text { Value = "Telefon", Font = new Engine.Entity.Font { Color = backLineColor, Size = 6 } });
            hallRefPoint.ElementList.Add(new Text { Value = "{Company.Phone}", Font = new Engine.Entity.Font { Color = backLineColor, Size = 8 } });
            hallRefPoint.ElementList.Add(new Text { Value = "Fax", Font = new Engine.Entity.Font { Color = backLineColor, Size = 6 } });
            hallRefPoint.ElementList.Add(new Text { Value = "{Company.Fax}", Font = new Engine.Entity.Font { Color = backLineColor, Size = 8 } });

            var tradRefPoint = new ReferencePoint { Left = UnitValue.Parse("8,5cm"), Top = UnitValue.Parse(top), Stack = ReferencePoint.StackMethod.Vertical };
            section.Footer.ElementList.Add(tradRefPoint);
            tradRefPoint.ElementList.Add(new Text { Value = "{EMail1}", Font = new Engine.Entity.Font { Color = backLineColor, Size = 8 } });
            tradRefPoint.ElementList.Add(new Text { Value = "{EMail2}", Font = new Engine.Entity.Font { Color = backLineColor, Size = 8 } });
            tradRefPoint.ElementList.Add(new Text { Value = "{EMail3}", Font = new Engine.Entity.Font { Color = backLineColor, Size = 8 } });

            var bg = new ReferencePoint { Left = UnitValue.Parse("16cm"), Top = UnitValue.Parse(top), Stack = ReferencePoint.StackMethod.Vertical };
            section.Footer.ElementList.Add(bg);
            bg.ElementList.Add(new Text { Value = "Bankgiro", Font = new Engine.Entity.Font { Color = backLineColor, Size = 6 } });
            bg.ElementList.Add(new Text { Value = "{Bankgiro}", Font = new Engine.Entity.Font { Color = backLineColor, Size = 8 } });


            #endregion
            #region Pane


            //Order summary
            var orderSummaryBv = new ReferencePoint { Top = SumTop };
            section.Pane.ElementList.Add(orderSummaryBv);
            orderSummaryBv.ElementList.Add(new Line { Width = UnitValue.Parse("100%"), Height="0", Color = backLineColor });
            orderSummaryBv.ElementList.Add(new Line { Top = UnitValue.Parse("1cm"), Width = UnitValue.Parse("100%"), Height = "0", Color = backLineColor });

            var orderSummaryPe = new ReferencePoint { Left = UnitValue.Parse("10cm"), Top = SumTop };
            section.Pane.ElementList.Add(orderSummaryPe);
            orderSummaryPe.ElementList.Add(new Line { Height = UnitValue.Parse("1cm"), Width = "0", Color = backLineColor });
            var peTitle = new Text { Value = "Summa exkl. moms", Font = new Engine.Entity.Font { Color = backLineColor, Size = 6 }, Left = UnitValue.Parse("2px") };
            orderSummaryPe.ElementList.Add(peTitle);
            var netSaleTotalPriceText = new Text { Value = "{NetSaleTotalPrice}", Top = UnitValue.Parse("0,5cm"), Width = UnitValue.Parse("2,45cm"), TextAlignment = TextBase.Alignment.Right };
            orderSummaryPe.ElementList.Add(netSaleTotalPriceText);

            var orderSummaryVat = new ReferencePoint { Left = UnitValue.Parse("12,5cm"), Top = SumTop };
            section.Pane.ElementList.Add(orderSummaryVat);
            orderSummaryVat.ElementList.Add(new Line { Height = UnitValue.Parse("1cm"), Width = "0", Color = backLineColor });
            var vatTiel = new Text { Value = "Moms", Font = new Engine.Entity.Font { Color = backLineColor, Size = 6 }, Left = UnitValue.Parse("2px") };
            orderSummaryVat.ElementList.Add(vatTiel);
            var vatSaleTotalPriceText = new Text { Value = "{VatSaleTotalPrice}", Top = UnitValue.Parse("0,5cm"), Width = UnitValue.Parse("2,45cm"), TextAlignment = TextBase.Alignment.Right };
            orderSummaryVat.ElementList.Add(vatSaleTotalPriceText);

            var orderSummaryGross = new ReferencePoint { Left = UnitValue.Parse("15cm"), Top = SumTop };
            section.Pane.ElementList.Add(orderSummaryGross);
            orderSummaryGross.ElementList.Add(new Line { Top = UnitValue.Parse("1cm"), Height = "0", Color = backLineColor });
            var grossTitle = new Text { Value = "Att betala", Font = new Engine.Entity.Font { Color = backLineColor, Size = 6 }, Left = UnitValue.Parse("2px") };
            orderSummaryGross.ElementList.Add(grossTitle);
            var grossSaleTotalPriceText = new Text { Value = "{GrossSaleTotalPrice}", Top = UnitValue.Parse("0,5cm"), Width = UnitValue.Parse("2,95cm"), TextAlignment = TextBase.Alignment.Right };
            orderSummaryGross.ElementList.Add(grossSaleTotalPriceText);

            //Extra info
            var vatInfoReference = new ReferencePoint { Left = UnitValue.Parse("0"), Top = SumTop };
            var vatInfoText = new Text { Value = "Org.nr {Company.OrgNo}. Innehar F-skattsedel. Vat nr {Company.VatNo}.", Left = UnitValue.Parse("2px"), Top = UnitValue.Parse("1cm"), Font = new Engine.Entity.Font { Size = 6, Color = backLineColor } };
            vatInfoReference.ElementList.Add(vatInfoText);
            //var lateInfoText = new Text { Value = "Vid betalning efter förfallodagen debiteras 18%", Left = UnitValue.Parse("2px"), Top = UnitValue.Parse("1,5cm"), FontSize = 10, FontColor = backLineColor };
            //vatInfoReference.ElementList.Add(lateInfoText);

            //TODO: Add to template
            var signText = new Text { Value = "Er kvittens", Left = UnitValue.Parse("2px"), Top = UnitValue.Parse("1,5cm"), Font = new Engine.Entity.Font { Size = 10, Color = backLineColor } };
            vatInfoReference.ElementList.Add(signText);
            var signLine = new Line { Left = UnitValue.Parse("2cm"), Top = UnitValue.Parse("2cm"), Width = UnitValue.Parse("5cm"), Height = "0", Color = backLineColor };
            vatInfoReference.ElementList.Add(signLine);

            section.Pane.ElementList.Add(vatInfoReference);

            //Pane
            //section.Pane.ElementList.Add(orderItemTable);
            //section.Pane.ElementList.Add(new Rectangle {Color = backLineColor, Thickness = UnitValue.Parse("1px")});
            section.Pane.ElementList.Add(new Rectangle { BorderColor = backLineColor });


            #endregion

            return section;
        }

        internal static void AssignFooterData(ref DocumentData documentData)
        {
            //TODO: Load this information from the service
            documentData.Add("Company.Name", "Skalleberg HTRG");
            documentData.Add("Company.StreetName", "Ormbackavägen 67");
            documentData.Add("Company.PostalCode", "175 61");
            documentData.Add("Company.City", "JÄRFÄLLA");
            documentData.Add("Company.Phone", "08-91 99 66");
            documentData.Add("Company.Fax", "08-91 50 67");
            documentData.Add("Company.OrgNo", "916619-5488");
            documentData.Add("Company.VatNo", "SE916619548801");
            documentData.Add("Bankgiro", "364-1230");
            documentData.Add("EMail1", "lennart@skalleberg.se");
            documentData.Add("EMail2", "roger@skalleberg.se");
            documentData.Add("EMail3", "oddbjorn@skalleberg.se");
        }
    }
}