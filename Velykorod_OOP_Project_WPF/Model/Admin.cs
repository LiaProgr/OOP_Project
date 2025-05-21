
namespace EventTickets
{
    public class Admin : User, IManageable
    {
        
        // Видалення події з адміна і якщо адмін видаляє то і у користувачів з кошика має видалятися*
        public void RemoveEvent(Event eventToRemove)
        {
            if (eventToRemove == null)
            {
                throw new InvalidOperationException("Подія для дидалення не може бути порожньою.");
            }

            // Видаляємо подію з репозиторію
            EventRepository.RemoveEvent(eventToRemove);

            // Видаляємо всі квитки на цю подію з кошиків користувачів
            foreach (var user in UserRepository.GetRegisteredUsers())
            {
                user.Cart.RemoveTicketsByEvent(eventToRemove);
            }
        }
        // редагування події у адміна*
        public void EditEvent(Event existingEvent, string name, Genre genre, DateOnly date, TimeOnly time, City city, string venue, string ticketPriceInput, string availableTicketsInput, string illustration)
        {
            ValidateEventFields(name, genre, date, time, city, venue, ticketPriceInput, availableTicketsInput, illustration);

            decimal ticketPrice = decimal.Parse(ticketPriceInput);
            int availableTickets = int.Parse(availableTicketsInput);

            existingEvent.Name = name;
            existingEvent.Genre = genre;
            existingEvent.Date = date;
            existingEvent.Time = time;
            existingEvent.City = city;
            existingEvent.Venue = venue;
            existingEvent.TicketPrice = ticketPrice;
            existingEvent.AvailableTickets = availableTickets;
            existingEvent.Illustration = illustration;
        }
        //додавання події у адміна *
        public void AddEvent(string name, Genre genre, DateOnly date, TimeOnly time, City city, string venue, string ticketPriceInput, string availableTicketsInput, string illustration)
        {
            ValidateEventFields(name, genre, date, time, city, venue, ticketPriceInput, availableTicketsInput, illustration);

            decimal ticketPrice = decimal.Parse(ticketPriceInput);
            int availableTickets = int.Parse(availableTicketsInput);

            Event newEvent = new Event(name, genre, date, time, city, venue, ticketPrice, availableTickets, illustration);
            EventRepository.AddEvent(newEvent);
        }
        //перевірка на валідність*
        private void ValidateEventFields(string name, Genre genre, DateOnly date, TimeOnly time, City city, string venue, string ticketPriceInput, string availableTicketsInput, string illustration)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(venue) || string.IsNullOrWhiteSpace(illustration) ||
                string.IsNullOrWhiteSpace(ticketPriceInput) || string.IsNullOrWhiteSpace(availableTicketsInput) )
            {
                throw new InvalidOperationException("Поля Назва події, Назва установи, Ілюстрації, Ціна квитка,К-сть квитків повинні бути заповнені.");
            }

            if (!decimal.TryParse(ticketPriceInput, out decimal ticketPrice) || ticketPrice < 0)
            {
                throw new InvalidOperationException("Ціна квитка має бути дійсним числом і не може бути від`ємним.");
            }

            if (!int.TryParse(availableTicketsInput, out int availableTickets) || availableTickets < 0)
            {
                throw new InvalidOperationException("Доступні квитки мають бути дійсним числом і не можуть бути від`ємним.");
            }
        }
    }
}
