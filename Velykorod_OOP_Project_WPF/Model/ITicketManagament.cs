using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTickets
{
    public interface ITicketManagament
    {
        public void AddToCart(Event selectedEvent, int quantity);
        void DownloadTicketPDF(Ticket ticket);// Завантаження квитка у PDF
    }
}
