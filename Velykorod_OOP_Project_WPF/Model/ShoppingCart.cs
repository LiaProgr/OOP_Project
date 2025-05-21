using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace EventTickets
{
    public class ShoppingCart: IShoppingCart
    {
        public  List<Ticket> ticketsOfCart { get; set; } = new List<Ticket>(); // Список квитків Кошику*
        private const string CartFileName = "cart.json";//*

        internal Action<object, EventArgs> CartUpdated;//?
        private DispatcherTimer _reservationTimer;//?
        //?
        public ShoppingCart()
        {
            _reservationTimer = new DispatcherTimer();
            _reservationTimer.Interval = TimeSpan.FromMinutes(1);
            _reservationTimer.Tick += (s, e) => CheckReservations();
            _reservationTimer.Start();
        }

        public void CheckReservations()
        {
            foreach (var ticket in ticketsOfCart.ToList())
            {
                if ((DateTime.Now - ticket.ReservationTime).TotalMinutes >= 1)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ticket.Event.ReservedTickets -= ticket.Quantity;
                        ticketsOfCart.Remove(ticket);
                        CartUpdated?.Invoke(this, EventArgs.Empty);
                        SaveCart();
                    });
                }
            }
        }
        //збереження кошика в файл*
        public void SaveCart()
        {
            try
            {
                var json = JsonSerializer.Serialize(ticketsOfCart);
                File.WriteAllText("cart.json", json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження кошика: {ex.Message}");
            }
        }
        //витягнення кошика з файлу*
        public void LoadCart()
        {
            try
            {
                if (File.Exists(CartFileName))
                {
                    var json = File.ReadAllText(CartFileName);
                    ticketsOfCart = JsonSerializer.Deserialize<List<Ticket>>(json) ?? new List<Ticket>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження кошика: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // поверення списку тікетів Кошика*
        public List<Ticket> GetEventsInCart()
        {
            return new List<Ticket>(ticketsOfCart);
        }
        //добавлення тікету до Кошику*
        public void AddTicket(Ticket ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentException("Квиток не може бути порожнім.");
            }
            ticketsOfCart.Add(ticket);
        }
        //видалення тікету за подією*
        public void RemoveTicketsByEvent(Event eventToRemove)
        {
            ticketsOfCart.RemoveAll(t => t.Event == eventToRemove);
        }
        //очищення кошику *
        public void ClearCart()
        {
            
            ticketsOfCart.Clear();
        }
       
    }
}

