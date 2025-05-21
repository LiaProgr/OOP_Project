using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace EventTickets
{
    [Serializable]
    public class Event : IComparable<Event>, INotifyPropertyChanged
    {
        private string _name;
        private Genre _genre;
        private DateOnly _date;
        private TimeOnly _time;
        private City _city;
        private string _venue;
        private decimal _ticketPrice;
        private int _availableTickets;
        private string _illustration;
       
 
        private int _reservedTickets;//*
        public int ReservedTickets//*
        {
            get => _reservedTickets;
            set { _reservedTickets = value; OnPropertyChanged(); }
        }
        //*
        public void Reserve(int quantity)
        {
            // Перевірка доступності з урахуванням вже зарезервованих
            if (quantity > (AvailableTickets - ReservedTickets))
                throw new Exception("Недостатньо квитків");

            ReservedTickets += quantity; // Збільшуємо резерв, не змінюючи AvailableTickets
            OnPropertyChanged(nameof(ReservedTickets));
            EventRepository.UpdateEvent(this);
        }
        //*
        public void Release(int quantity)
        {
            ReservedTickets -= quantity; // Скасовуємо резерв
            OnPropertyChanged(nameof(ReservedTickets));
            EventRepository.UpdateEvent(this);
        }
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidOperationException("Назва не може бути порожньою.");
                _name = value;
            }
        }
        public Genre Genre
        {
            get => _genre;
            set
            {
                if (!Enum.IsDefined(typeof(Genre), value))
                    throw new InvalidOperationException("Невірний жанр.");
                _genre = value;
            }
        }
        public DateOnly Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }
        public TimeOnly Time
        {
            get => _time;
            set
            {
                _time = value;
                OnPropertyChanged();
            }
        }
        public City City
        {
            get => _city;
            set
            {
                if (!Enum.IsDefined(typeof(City), value))
                    throw new InvalidOperationException("Невірне місто.");
                _city = value;
        
            }
        }
        public string Venue
        {
            get => _venue;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidOperationException("Місце проведення не може бути порожнім.");
                _venue = value;
            
            }
        }
        public decimal TicketPrice
        {
            get => _ticketPrice;
            set
            {
                if (value < 0)
                    throw new InvalidOperationException("Ціна квитка не може бути від`ємною.");
                _ticketPrice = value;
           
            }
        }
        public int AvailableTickets
        {
            get => _availableTickets;
            set
            {
                if (value < 0)
                    throw new InvalidOperationException("Доступні квитки не можуть  бути від`ємними");
                _availableTickets = value;
                OnPropertyChanged();
            }
        }
        public string Illustration
        {
            get => _illustration;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidOperationException("Ілюстрація не може бути порожньою.");
                _illustration = value;
             
            }
        }
        //*
        public Event(string name, Genre genre, DateOnly date, TimeOnly time, City city, string venue, decimal ticketPrice, int availableTickets, string illustration)
        {
            Name = name;
            Genre = genre;
            Date = date;
            Time = time;
            City = city;
            Venue = venue;
            TicketPrice = ticketPrice;
            AvailableTickets = availableTickets;
            Illustration = illustration;
        }
        
        //*
        public Event()
        {
        }
        //?!
        public int CompareTo(Event other)
        {
            if (other == null) return 1;
            int dateComparison = Date.CompareTo(other.Date);
            if (dateComparison != 0)
            {
                return dateComparison;
            }
            return Time.CompareTo(other.Time);
        }
        // ІВЕННТ використовується *
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
