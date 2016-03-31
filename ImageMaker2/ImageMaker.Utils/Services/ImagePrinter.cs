using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageMaker.Utils.Services
{
    public class ImagePrinter
    {
        public IEnumerable<string> GetAvailablePrinters()
        {
            string[] printers = new string[PrinterSettings.InstalledPrinters.Count];
            PrinterSettings.InstalledPrinters.CopyTo(printers, 0);
            return printers;
        }

        public void Print(byte[] buffer, string printerName, int copies = 1)
        {
            copies = 1;
            if (string.IsNullOrEmpty(printerName))
                return;

            try
            {
                using (var stream = new MemoryStream(buffer))
                {
                    _image = Image.FromStream(stream);
                }

                _printDocument = new PrintDocument();

                _printDocument.PrinterSettings.Copies = (short)copies;
                _printDocument.PrintController = new StandardPrintController();
                _printDocument.PrinterSettings.PrinterName = printerName;
                _printDocument.BeginPrint += printDocument_BeginPrint;
                _printDocument.PrintPage += printDocument_PrintPage;
                _printDocument.Print();
            }
            catch (Exception)
            {
            }
        }

        public void Print(byte[] buffer, int copies = 1)
        {
            try
            {
                copies = 1;
                using (var stream = new MemoryStream(buffer))
                {
                    _image = Image.FromStream(stream);
                }

                _printDocument = new PrintDocument();
                IEnumerable<string> printers = GetAvailablePrinters();
                string name = printers.FirstOrDefault(x => x.Contains("PDF"));

                //printer not found
                if (string.IsNullOrEmpty(name))
                    return;

                _printDocument.PrinterSettings.Copies = (short)copies;
                _printDocument.PrintController = new StandardPrintController();
                _printDocument.PrinterSettings.PrinterName = name;
                _printDocument.BeginPrint += printDocument_BeginPrint;
                _printDocument.PrintPage += printDocument_PrintPage;
                _printDocument.Print();
            }
            catch (Exception)
            {
            }
        }

        public void PrintAsync(byte[] buffer, string printerName, int copies = 1)
        {
            if (string.IsNullOrEmpty(printerName))
                return;

            Task.Run(() =>
            {
                try
                {
                    using (var stream = new MemoryStream(buffer))
                    {
                        _image = Image.FromStream(stream);
                    }

                    _printDocument = new PrintDocument();

                    _printDocument.PrinterSettings.Copies = (short)copies;
                    _printDocument.PrintController = new StandardPrintController();
                    _printDocument.PrinterSettings.PrinterName = printerName;
                    _printDocument.BeginPrint += printDocument_BeginPrint;
                    _printDocument.PrintPage += printDocument_PrintPage;
                    _printDocument.Print();
                }
                catch (Exception)
                {
                }

            });
        }

        public void PrintAsync(byte[] buffer, int copies = 1)
        {
            Task.Run(() =>
            {
                try
                {
                    copies = 1;
                    using (var stream = new MemoryStream(buffer))
                    {
                        _image = Image.FromStream(stream);
                    }

                    _printDocument = new PrintDocument();
                    IEnumerable<string> printers = GetAvailablePrinters();
                    string name = printers.FirstOrDefault(x => x.Contains("PDF"));

                    //printer not found
                    if (string.IsNullOrEmpty(name))
                        return;

                    _printDocument.PrinterSettings.Copies = (short)copies;
                    _printDocument.PrintController = new StandardPrintController();
                    _printDocument.PrinterSettings.PrinterName = name;
                    _printDocument.BeginPrint += printDocument_BeginPrint;
                    _printDocument.PrintPage += printDocument_PrintPage;
                    _printDocument.Print();
                }
                catch (Exception)
                {
                }

            });
        }

        private Image _image;
        private PrintDocument _printDocument;
        PrintAction _printAction = PrintAction.PrintToFile;

        private void printDocument_BeginPrint(object sender, PrintEventArgs e)
        {
            // Save our print action so we know if we are printing 
            // a preview or a real document.
            _printAction = e.PrintAction;

            // We ALWAYS want true here, as we will implement the 
            // margin limitations later in code.
            _printDocument.OriginAtMargins = true;

            // Set some preferences, our method should print a box with any 
            // combination of these properties being true/false.
            _printDocument.DefaultPageSettings.Landscape = _image.Height > _image.Width;
            _printDocument.DefaultPageSettings.Margins.Top = 0;
            _printDocument.DefaultPageSettings.Margins.Left = 0;
            _printDocument.DefaultPageSettings.Margins.Right = 0;
            _printDocument.DefaultPageSettings.Margins.Bottom = 0;
        }

        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;

            // If you set printDocumet.OriginAtMargins to 'false' this event 
            // will print the largest rectangle your printer is physically 
            // capable of. This is often 1/8" - 1/4" from each page edge.
            // ----------
            // If you set printDocument.OriginAtMargins to 'false' this event
            // will print the largest rectangle permitted by the currently 
            // configured page margins. By default the page margins are 
            // usually 1" from each page edge but can be configured by the end
            // user or overridden in your code.
            // (ex: printDocument.DefaultPageSettings.Margins)

            // Grab a copy of our "hard margins" (printer's capabilities) 
            // This varies between printer models. Software printers like 
            // CutePDF will have no "physical limitations" and so will return 
            // the full page size 850,1100 for a letter page size.
            RectangleF printableArea = e.PageSettings.PrintableArea;
            RectangleF realPrintableArea = new RectangleF(
                (e.PageSettings.Landscape ? printableArea.Y : printableArea.X),
                (e.PageSettings.Landscape ? printableArea.X : printableArea.Y),
                (e.PageSettings.Landscape ? printableArea.Height : printableArea.Width),
                (e.PageSettings.Landscape ? printableArea.Width : printableArea.Height)
                );

            // If we are printing to a print preview control, the origin won't have 
            // been automatically adjusted for the printer's physical limitations. 
            // So let's adjust the origin for preview to reflect the printer's 
            // hard margins.
            // ----------
            // Otherwise if we really are printing, just use the soft margins.
            g.TranslateTransform(
                ((_printAction == PrintAction.PrintToPreview) ? realPrintableArea.X : 0) - e.MarginBounds.X,
                ((_printAction == PrintAction.PrintToPreview) ? realPrintableArea.Y : 0) - e.MarginBounds.Y
            );

            // Draw the printable area rectangle in PURPLE
            Rectangle printedPrintableArea = Rectangle.Truncate(realPrintableArea);
            g.DrawImage(_image, new Rectangle(0, 0, printedPrintableArea.Width-7, printedPrintableArea.Height-7), new Rectangle(0, 0, _image.Width, _image.Height), GraphicsUnit.Pixel);
            //g.DrawRectangle(Pens.Purple, printedPrintableArea);

            //// Grab a copy of our "soft margins" (configured printer settings)
            //// Defaults to 1 inch margins, but could be configured otherwise by 
            //// the end user. You can also specify some default page margins in 
            //// your printDocument.DefaultPageSetting properties.
            //RectangleF marginBounds = e.MarginBounds;

            //// This intersects the desired margins with the printable area rectangle. 
            //// If the margins go outside the printable area on any edge, it will be 
            //// brought in to the appropriate printable area.
            //marginBounds.Intersect(realPrintableArea);

            //// Draw the margin rectangle in RED
            //Rectangle printedMarginArea = Rectangle.Truncate(marginBounds);
            //printedMarginArea.Width--;
            //printedMarginArea.Height--;
            //// g.DrawRectangle(Pens.Red, printedMarginArea);
        }
    }
}
