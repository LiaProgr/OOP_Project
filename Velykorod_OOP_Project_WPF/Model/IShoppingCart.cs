

namespace EventTickets
{
    public interface IShoppingCart
    {
        public void RemoveTicketsByEvent(Event eventToRemove);
        void AddTicket(Ticket ticket);
        void ClearCart();
        List<Ticket> GetEventsInCart();
        public void LoadCart();
        public void SaveCart();
        public void CheckReservations();
    }
}
