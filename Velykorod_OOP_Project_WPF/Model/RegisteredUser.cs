
using System.Text.Json;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using System.Reflection.Metadata;
using Document = QuestPDF.Fluent.Document;
using System.Windows;

namespace EventTickets
{
    public class RegisteredUser : User,  ITicketManagament
    {
        //зареєстрований користувач має кошик
        public ShoppingCart Cart { get; private set; } // Кошик користувача*
        public List<Ticket> PurchasedTickets { get; set; } = new List<Ticket>();// *
        //?*
        public RegisteredUser()
        {
            Cart = new ShoppingCart(); // Ініціалізуємо кошик
        }
        private const string PurchasedTicketsFileName = "purchased_tickets_{0}.json"; // {0} — ім'я користувача

        // Зберегти куплені квитки у файл
        public void SavePurchasedTickets()
        {
            try
            {
                var path = string.Format(PurchasedTicketsFileName, Username);
                var json = JsonSerializer.Serialize(PurchasedTickets);
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Помилка збереження квитків: {ex.Message}");
            }
        }

        // Завантажити куплені квитки з файлу
        public void LoadPurchasedTickets()
        {
            try
            {
                var path = string.Format(PurchasedTicketsFileName, Username);
                if (File.Exists(path))
                {
                    var json = File.ReadAllText(path);
                    PurchasedTickets = JsonSerializer.Deserialize<List<Ticket>>(json) ?? new List<Ticket>();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Помилка завантаження квитків: {ex.Message}");
            }
        }
        

        public bool Checkout(PaymentDetails payment)
        {
            // Перевірка вхідних параметрів
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            var ticketsInCart = Cart.GetEventsInCart();

            // Перевірка на порожній кошик
            if (ticketsInCart.Count == 0)
                throw new InvalidOperationException("Кошик порожній.");

            // 1. Перевірка актуальної доступності всіх квитків
            foreach (var ticket in ticketsInCart)
            {
               
                if (ticket.Event.AvailableTickets < ticket.Quantity)
                {
                    // Скасувати резервування для цієї події
                    ticket.Event.Release(ticket.Quantity);
                    // Видалити з кошика
                    Cart.RemoveTicketsByEvent(ticket.Event);
                    // Повідомити про помилку
                    throw new InvalidOperationException(
                        $"Подія '{ticket.Event.Name}' вже розпродана. Кошик оновлено."
                    );
                }
               
            }

            // 2. Спроба оплати
            decimal totalAmount = ticketsInCart.Sum(t => t.Quantity * t.Event.TicketPrice);
            if (!PaymentProcessor.ProcessPayment(payment, totalAmount))
                return false;

            // 3. Фіксація покупки
            foreach (var ticket in ticketsInCart)
            {
                // Списання квитків
                ticket.Event.AvailableTickets -= ticket.Quantity;
                // Скасування резервування
                ticket.Event.ReservedTickets -= ticket.Quantity;
                // Додавання до куплених
                PurchasedTickets.Add(ticket);
               
                EventRepository.UpdateEvent(ticket.Event); // Оновлення в репозиторії
            }

            // 4. Очищення кошика
            Cart.ClearCart();
            Cart.SaveCart(); // Зберегти зміни в кошику

            // 5. Оновлення даних про події
            EventRepository.SaveEvents();
            SavePurchasedTickets();

            return true;
        }
        //*
        public void AddToCart(Event selectedEvent, int quantity)
        {
           
            if (selectedEvent.AvailableTickets < quantity)
                throw new ArgumentException("Недостатньо доступних квитків");

            selectedEvent.Reserve(quantity);
            Cart.AddTicket(new Ticket(selectedEvent, quantity));
            EventRepository.SaveEvents(); // Збереження змін
        }
        public void DownloadTicketPDF(Ticket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException(nameof(ticket));

            string imagePath = ticket.Event.Illustration;

            Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    page.Header()
                        .AlignCenter()
                        .Text("Електронний квиток")
                        .Bold()
                        .FontSize(24)
                        .FontColor(Colors.Black);

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                    {
                        column.Spacing(15);

                        // Ілюстрація — БЕЗПЕЧНО
                        if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                        {
                            column.Item().Element(container =>
                            {
                                container.AlignLeft().Height(200).Image(imagePath);
                            });

                            column.Item().LineHorizontal(1).LineColor(Colors.Black);
                        }

                        // Інформація про квиток
                        column.Item().Text($"Подія: {ticket.Event.Name}").FontSize(14).Bold();
                        column.Item().Text($"Дата: {ticket.Event.Date:d}");
                        column.Item().Text($"Час: {ticket.Event.Time}");
                        column.Item().Text($"Місто: {ticket.Event.City}");
                        column.Item().Text($"Місце: {ticket.Event.Venue}");
                        column.Item().Text($"Кількість квитків: {ticket.Quantity}");
                        column.Item().Text($"Ціна за квиток: {ticket.Event.TicketPrice} грн");
                        column.Item().Text($"Загальна сума: {ticket.TotalPrice} грн").Bold();

                        column.Item().LineHorizontal(1).LineColor(Colors.Black);

                        column.Item().Text("Дякуємо за покупку!").Italic().FontSize(12).AlignCenter();
                        column.Item().Text("Телефон підтримки: +38 (099) 123-45-67").FontSize(10).AlignCenter();
                        column.Item().Text($"Дата генерації: {DateTime.Now:g}")
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken1)
                            .AlignRight();
                    });
                });
            })
            .GeneratePdf($"Ticket_{ticket.Event.Name}.pdf");

          
        }
        //*
        public void SaveEventsToJson(List<Event> events, string filePath)
        {
            if (events == null || events.Count == 0)
                throw new InvalidOperationException("Список подій порожній, немає чого зберігати.");

            var eventsToSave = events.Select(e => new
            {
                e.Name,
                e.Genre,
                e.Date,
                e.Time,
                e.City,
                e.Venue,
                e.TicketPrice,
                e.AvailableTickets,
                e.Illustration
                // ReservedTickets не включається
            }).ToList();

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string json = JsonSerializer.Serialize(eventsToSave, options);
            File.WriteAllText(filePath, json);
        }
    }
}
