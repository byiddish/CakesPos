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
    public class StatementManager
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=CakesPos;Integrated Security=True";

        public void CreateStatementPDF(StatementsModel s, string path)
        {
            var customer = s.Orders.FirstOrDefault().customer;
            var doc5 = new Document();

            PdfPTable table = new PdfPTable(5);
            /*
            * default table width => 80%
            */
            table.WidthPercentage = 100;
            // then set the column's __relative__ widths
            table.SetWidths(new Single[] { 3, 3, 6, 3, 3 });
            /*
            * by default tables 'collapse' on surrounding elements,
            * so you need to explicitly add spacing
            */
            table.SpacingBefore = 10;


            PdfPCell cell1 = new PdfPCell(new Phrase("Date"));
            cell1.BackgroundColor = new BaseColor(204, 204, 204);
            table.AddCell(cell1);

            PdfPCell cell2 = new PdfPCell(new Phrase("Invoice"));
            cell2.BackgroundColor = new BaseColor(204, 204, 204);
            table.AddCell(cell2);

            PdfPCell cell3 = new PdfPCell(new Phrase("Description"));
            cell3.BackgroundColor = new BaseColor(204, 204, 204);
            table.AddCell(cell3);

            //PdfPCell cell4 = new PdfPCell(new Phrase("Payment"));
            //cell4.BackgroundColor = new BaseColor(204, 204, 204);
            //table.AddCell(cell4);

            PdfPCell cell5 = new PdfPCell(new Phrase("Amount"));
            cell5.BackgroundColor = new BaseColor(204, 204, 204);
            table.AddCell(cell5);

            PdfPCell cell6 = new PdfPCell(new Phrase("Balance"));
            cell6.BackgroundColor = new BaseColor(204, 204, 204);
            table.AddCell(cell6);

            Phrase BillFrom = new Phrase();
            Chunk a = new Chunk("Elegant Cakes \n");
            Chunk csz = new Chunk("922 46th Street \nBrooklyn NY 11219\n");
            Chunk p = new Chunk("Phone: 785-438-8547 \n");
            Chunk f = new Chunk("Fax: 787-854-7785 \n");
            Chunk e = new Chunk("Email: siegelmancake@gmail.com \n______________________________\n\n");
            BillFrom.Add(a);
            BillFrom.Add(csz);
            BillFrom.Add(p);
            BillFrom.Add(f);
            BillFrom.Add(e);

            Phrase BillTo = new Phrase();
            Chunk cn = new Chunk(customer.FirstName + " " + customer.LastName + " \n");
            Chunk ca = new Chunk(customer.Address + " \n");
            Chunk ccsz = new Chunk(customer.City + " " + customer.State + " " + customer.Zip + " \n");
            //Chunk cp = new Chunk(customer.Phone + " \n");
            //Chunk cf = new Chunk(customer.Cell + " \n");
            BillTo.Add(cn);
            BillTo.Add(ca);
            BillTo.Add(ccsz);
            //BillTo.Add(cp);
            //BillTo.Add(cf);

            Font headerFont = FontFactory.GetFont("Verdana", 36, BaseColor.BLUE);
            Paragraph header = new Paragraph("Statement", headerFont);
            header.Alignment = 2;

            Font invoiceFont = FontFactory.GetFont("Verdana", 16);
            Paragraph title = new Paragraph("Statement #" + s.Statement.Id, invoiceFont);
            title.Alignment = 2;

            Paragraph date = new Paragraph("Date: " + s.Statement.Date.ToShortDateString(), invoiceFont);
            date.Alignment = 2;


            //Paragraph subTotal = new Paragraph("Subtotal: " + "$450.00");
            //subTotal.Alignment = 2;
            //Paragraph discount = new Paragraph("Discount: " + "$0.00");
            //discount.Alignment = 2;
            Paragraph total = new Paragraph("_________________\nTotal Due: " + s.Statement.Balance.ToString("C"));
            total.Alignment = 2;


            Paragraph paymentMessage = new Paragraph("\nMake all checks payable to\nElegant Cakes");
            paymentMessage.Alignment = 2;

            Font greetingFont = FontFactory.GetFont("Ariel", 18);
            Paragraph greeting1 = new Paragraph("Thank you for your bussiness!", greetingFont);

            Paragraph greeting2 = new Paragraph("If you have any questions with this invoice, please contact\nSamson 917-654-8899 siegelmancake@gmail.com");

            Font hFont = FontFactory.GetFont("Verdana", 20, BaseColor.LIGHT_GRAY);
            Paragraph companyName = new Paragraph("Elegant Cakes", hFont);

            Paragraph billToHeader = new Paragraph("Bill To:\n", hFont);

            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            double balance = 0;
            double discount = 0;
            foreach (OrderDetailsViewModel o in s.Orders)
            {
                var orderDate = o.order.OrderDate.ToShortDateString();
                var invoice = o.order.Id;
                var descripton = o.orderedProducts.Sum(pr => pr.quantity) + " Items ordered";
                //var payment = "";
                var amount = cpr.GetTotalByOrderId(o.order.Id, o.order.CustomerId);
                balance += amount;

                table.AddCell(orderDate);
                table.AddCell(invoice.ToString());
                table.AddCell(descripton);
                //table.AddCell(payment);
                table.AddCell(amount.ToString("C"));
                table.AddCell(balance.ToString("C"));

                foreach (Payment payment in o.payments)
                {
                    DateTime paymentDate = (DateTime)payment.Date;
                    var invoiceBlank = "Payment";
                    var pDescripton = "Thank you for your payment!";
                    //var payment = "";
                    var pAmount = (double)payment.Payment1;
                    balance -= pAmount;
                    table.AddCell(paymentDate.ToShortDateString());
                    table.AddCell(invoiceBlank);
                    table.AddCell(pDescripton);
                    //table.AddCell(payment);
                    table.AddCell(pAmount.ToString("C"));
                    table.AddCell(balance.ToString("C"));
                }
            }

            discount = balance - (double)s.Statement.Balance;

            Paragraph disc = new Paragraph("Discount: " + discount.ToString("C"));
            disc.Alignment = 2;


            Paragraph balanceDue = new Paragraph("Balance Due: " + s.Statement.Balance.ToString("C"));
            balanceDue.Alignment = 2;




            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);

            PdfWriter.GetInstance(doc5, fs);


            doc5.Open();
            doc5.Add(header);
            doc5.Add(date);
            doc5.Add(title);
            doc5.Add(balanceDue);
            doc5.Add(companyName);
            doc5.Add(BillFrom);
            doc5.Add(billToHeader);
            doc5.Add(BillTo);
            doc5.Add(table);
            //doc5.Add(subTotal);
            //doc5.Add(discount);
            doc5.Add(disc);
            doc5.Add(total);
            doc5.Add(paymentMessage);
            doc5.Add(greeting2);
            doc5.Add(greeting1);
            doc5.Close();
        }


        public void EmailStatement(string file, string email, string date)
        {
            MailMessage mail = new MailMessage();
            mail.From = new System.Net.Mail.MailAddress("\"Elegant Cakes\" <siegelmancake@gmail.com>");

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
            string st = "Dear valued customer,<br /><br /> Thank you for giving us the opportunity to do business with you!<br /><br />Please see attached Statement...<br /><br />Sincerely,<br /><br />Samson";

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
            mail.Subject = "Statement " + date;
            smtp.Send(mail);
        }
    }
}