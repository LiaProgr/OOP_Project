using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTickets
{
    [Serializable]
    public class Ticket
    {
        public Event Event { get; set; }//подія тікету *
        public int Quantity { get; set; }//к-сть тіектів ТІКЕТУ *
        public decimal TotalPrice => Event.TicketPrice * Quantity;//загальна ціна ТІКЕТУ*

        public DateTime ReservationTime { get; internal set; }
        //*
        public Ticket(Event @event, int quantity)
        {
            Event = @event ?? throw new ArgumentNullException(nameof(@event));
            Quantity = quantity > 0 ? quantity : throw new ArgumentException("Кількість квитків має бути більше 0.");
            ReservationTime = DateTime.Now; // Автоматична ініціалізація
        }
    }

}
