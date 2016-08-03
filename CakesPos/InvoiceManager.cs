using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using CakesPos.Data;
using System.Net.Mail;
using System.Net;
using System.Net.Mime;


namespace CakesPos
{
    public class InvoiceManager
    {
        public void CreateInvoicePDF(OrderDetailsViewModel order)
        {
            var doc5 = new Document();

            PdfPTable table = new PdfPTable(4);
            /*
            * default table width => 80%
            */
            table.WidthPercentage = 100;
            // then set the column's __relative__ widths
            //table.SetWidths(new Single[] { 4, 4, 4, 4 });
            /*
            * by default tables 'collapse' on surrounding elements,
            * so you need to explicitly add spacing
            */
            table.SpacingBefore = 10;


            PdfPCell cell1 = new PdfPCell(new Phrase("Product"));
            cell1.BackgroundColor = new BaseColor(204, 204, 204);
            table.AddCell(cell1);

            PdfPCell cell2 = new PdfPCell(new Phrase("Quantity"));
            cell2.BackgroundColor = new BaseColor(204, 204, 204);
            table.AddCell(cell2);

            PdfPCell cell3 = new PdfPCell(new Phrase("Unit Price"));
            cell3.BackgroundColor = new BaseColor(204, 204, 204);
            table.AddCell(cell3);

            PdfPCell cell4 = new PdfPCell(new Phrase("Price"));
            cell4.BackgroundColor = new BaseColor(204, 204, 204);
            table.AddCell(cell4);

            Phrase BillFrom = new Phrase();
            Chunk a = new Chunk("Siegelman Cakes \n");
            Chunk csz = new Chunk("922 46th Street \nBrooklyn NY 11219\n");
            Chunk p = new Chunk("Phone: 718-438-0772 \n");
            //Chunk f = new Chunk("Fax: 787-854-7785 \n");
            Chunk e = new Chunk("Email: siegelmancake@gmail.com \n______________________________\n\n");
            BillFrom.Add(a);
            BillFrom.Add(csz);
            BillFrom.Add(p);
            //BillFrom.Add(f);
            BillFrom.Add(e);

            Phrase BillTo = new Phrase();
            Chunk cn = new Chunk(order.customer.FirstName + " " + order.customer.LastName + " \n");
            Chunk ca = new Chunk(order.customer.Address + " \n");
            Chunk ccsz = new Chunk(order.customer.City + " " + order.customer.State + order.customer.Zip + " \n");
            //Chunk cp = new Chunk(order.customer.Phone1 + " \n");
            //Chunk cf = new Chunk(order.customer.Cell1 + " \n");
            BillTo.Add(cn);
            BillTo.Add(ca);
            BillTo.Add(ccsz);
            //BillTo.Add(cp);
            //BillTo.Add(cf);

            Font headerFont = FontFactory.GetFont("Verdana", 36, BaseColor.BLUE);
            Paragraph header = new Paragraph("Invoice", headerFont);
            header.Alignment = 2;

            Font invoiceFont = FontFactory.GetFont("Verdana", 16);
            Paragraph title = new Paragraph("Invoice: #" + order.order.Id, invoiceFont);
            title.Alignment = 2;

            Paragraph date = new Paragraph("Date: " + DateTime.Today.ToShortDateString(), invoiceFont);
            date.Alignment = 2;

            //Paragraph total = new Paragraph("_________________\nTotal Due: " + "$450.00");
            //total.Alignment = 2;


            Paragraph paymentMessage = new Paragraph("\nMake all checks payable to\nSiegelman Cakes");
            paymentMessage.Alignment = 2;

            Font greetingFont = FontFactory.GetFont("Ariel", 18);
            Paragraph greeting1 = new Paragraph("Thank you for your business!", greetingFont);

            Paragraph greeting2 = new Paragraph("If you have any questions with this invoice, please contact\n718-438-0772");

            Font hFont = FontFactory.GetFont("Verdana", 20, BaseColor.LIGHT_GRAY);
            Paragraph companyName = new Paragraph("Siegelman Cakes", hFont);

            Paragraph billToHeader = new Paragraph("Bill To:\n", hFont);

            double price = 0;
            double t = 0;
            double priceExcludingDis = 0;
            foreach (OrderDetailsProductModel pm in order.orderedProducts)
            {
                double catererDiscount = (double)pm.catererDiscount;
                if ((bool)order.customer.Caterer)
                {
                    if (catererDiscount < 1)
                    {
                        catererDiscount = (double)pm.unitPrice * (double)pm.quantity * catererDiscount;
                        t += ((double)pm.quantity * (double)pm.unitPrice - catererDiscount);
                        price = ((double)pm.quantity * (double)pm.unitPrice - catererDiscount);
                    }
                    else
                    {
                        t += (double)pm.unitPrice * (double)pm.quantity - (double)pm.quantity * catererDiscount;
                        price = (double)pm.unitPrice * (double)pm.quantity - (double)pm.quantity * catererDiscount;
                    }
                    //if (pm.categoryId == 1)
                    //{
                    //    catererDiscount = 5;
                    //    t += (double)pm.unitPrice * (double)pm.quantity - (double)pm.quantity * catererDiscount;
                    //    price = (double)pm.unitPrice * (double)pm.quantity - (double)pm.quantity * catererDiscount;
                    //}
                    //else if (pm.categoryId == 2 || pm.categoryId == 3)
                    //{
                    //    catererDiscount = (double)pm.unitPrice * (double)pm.quantity * 0.1;
                    //    t += ((double)pm.quantity * (double)pm.unitPrice - catererDiscount);
                    //    price = ((double)pm.quantity * (double)pm.unitPrice - catererDiscount);
                    //}
                    //else if (pm.categoryId == 4)
                    //{
                    //    catererDiscount = 2.5;
                    //    t += (double)pm.unitPrice * (double)pm.quantity - (double)pm.quantity * catererDiscount;
                    //    price = (double)pm.unitPrice * (double)pm.quantity - (double)pm.quantity * catererDiscount;
                    //}
                    //else
                    //{
                    //    t += (double)pm.unitPrice * pm.quantity;
                    //    price = (double)pm.unitPrice * (double)pm.quantity;
                    //}
                }
                else
                {
                    t += (double)pm.unitPrice * pm.quantity;
                    price = (double)pm.unitPrice * pm.quantity;
                }
                priceExcludingDis += (double)pm.unitPrice * pm.quantity;

                var quantity = pm.quantity.ToString();
                var unitPrice = pm.unitPrice.ToString("C");
                //var price = (pm.quantity * pm.unitPrice).ToString("C");
                table.AddCell(pm.productName);
                table.AddCell(quantity);
                table.AddCell(unitPrice);
                table.AddCell((pm.quantity * pm.unitPrice).ToString("C"));
            }

            Paragraph subTotal = new Paragraph("Subtotal: " + priceExcludingDis.ToString("C"));
            subTotal.Alignment = 2;
            double dis = (double)order.order.Discount;
            if (dis < 1)
            {
                dis = t * dis;
            }
            Paragraph discount = new Paragraph("Discount: " + (priceExcludingDis - t + dis).ToString("C"));
            discount.Alignment = 2;

            Paragraph deliveryCharge = new Paragraph("Delivery Charge: " + ((decimal)order.order.DeliveryCharge).ToString("C"));
            deliveryCharge.Alignment = 2;

            Paragraph total = new Paragraph("_________________\nTotal Due: " + ((decimal)t + (decimal)order.order.DeliveryCharge - (decimal)dis).ToString("C"));
            total.Alignment = 2;

            string path = @"C:\inetpub\sites\CakesPos\InvoicesPdf\" + order.order.Id + ".pdf";


            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);

            PdfWriter.GetInstance(doc5, fs);


            doc5.Open();
            doc5.Add(header);
            doc5.Add(title);
            doc5.Add(date);
            doc5.Add(companyName);
            doc5.Add(BillFrom);
            doc5.Add(billToHeader);
            doc5.Add(BillTo);
            doc5.Add(table);
            doc5.Add(subTotal);
            if(order.order.DeliveryOption=="Delivery")
            {
                doc5.Add(deliveryCharge);
            }
            doc5.Add(discount);
            doc5.Add(total);
            foreach (Payment pay in order.payments)
            {
                decimal payment = (decimal)pay.Payment1;
                Paragraph payments = new Paragraph("Thank you for your payment of " + payment.ToString("C"));
                doc5.Add(payments);
            }
            doc5.Add(paymentMessage);
            doc5.Add(greeting2);
            doc5.Add(greeting1);
            doc5.Close();
        }


        public void EmailInvoice(string file, string email)
        {
            MailMessage mail = new MailMessage();
            mail.From = new System.Net.Mail.MailAddress("\"Siegelman Cakes\" <siegelmancake@gmail.com>");

            // The important part -- configuring the SMTP client
            SmtpClient smtp = new SmtpClient();
            smtp.Port = 587;   // [1] You can try with 465 also, I always used 587 and got success
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network; // [2] Added this
            smtp.UseDefaultCredentials = false; // [3] Changed this
            smtp.Credentials = smtp.Credentials = new NetworkCredential("siegelmancake@gmail.com", "cake922t");  // [4] Added this. 
            smtp.Host = "smtp.gmail.com";

            //recipient address
            mail.To.Add(new MailAddress(email));

            //Formatted mail body
            mail.IsBodyHtml = true;
            string st = "Dear valued customer,<br /><br /> Thank you for giving us the opportunity to do business with you!<br /><br />Please see attached invoice...<br /><br />Sincerely,<br /><br />Samson";

            // Create  the file attachment for this e-mail message.
            Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
            // Add time stamp information for the file.
            ContentDisposition disposition = data.ContentDisposition;
            disposition.CreationDate = System.IO.File.GetCreationTime(file);
            disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
            disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
            // Add the file attachment to this e-mail message.
            mail.Attachments.Add(data);

            mail.Body = st;
            mail.Subject = "Invoice " + DateTime.Now.Date.ToShortDateString();
            smtp.Send(mail);
        }
    }
}