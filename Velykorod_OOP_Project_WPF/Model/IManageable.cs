using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTickets
{
    public interface IManageable
    {

        void RemoveEvent(Event eventToRemove);
        void EditEvent(Event existingEvent, string name, Genre genre, DateOnly date, TimeOnly time, City city, string venue, string ticketPriceInput, string availableTicketsInput, string illustration);
        void AddEvent(string name, Genre genre, DateOnly date, TimeOnly time, City city, string venue, string ticketPriceInput, string availableTicketsInput, string illustration);
    }
}
