using Payment.Dtos;
using System;

namespace Payment
{
    public partial class InvoiceDetail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user"] == null)
                Response.Redirect("Login", true);

            var invoice = Session["invoice"] as InvoiceDto;
            if (invoice == null) return;

            if (invoice.Status == true) payBtn.Visible = false;

            serviceName.Text = invoice.ServiceName;

            amount.Text = invoice.Amount.ToString();

            if (invoice.PaymentDate == null)
                paymentDate.Text = "--.--.--";
            else paymentDate.Text = invoice.PaymentDate.Value.ToString("d");

            dueDate.Text = invoice.DueDate.ToString("d");

            issueDate.Text = invoice.IssueDate.ToString("d");

            if (invoice.ReceiptNumber != null)
                receiptNumber.Text = invoice.ReceiptNumber;
            else receiptNumber.Text = "----------";

            if (invoice.Status == true) status.Text = "Оплачено";
            else status.Text = "Не оплачено";
        }

        protected void payBtn_Click(object sender, EventArgs e)
        {
            if (Session["invoice"] == null) return;
            Response.Redirect("PayInvoice", true);
        }


        protected void logoutBtn_Click(object sender, EventArgs e)
        {
            Session["user"] = null;
            Response.Redirect("Login", true);
        }

        protected void myInvBtn_Click(object sender, EventArgs e)
        {
            Session["invoice"] = null;
            Response.Redirect("MyInvoices", true);
        }
    }
}