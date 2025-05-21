using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTickets
{
    public class PaymentDetails
    {
        private string _cardNumber;
        private string _expiryDate;
        private string _cvv;
        private string _cardHolderName;

        public string CardNumber
        {
            get => _cardNumber;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length != 16 || !value.All(char.IsDigit))
            {
                    throw new ArgumentException("Номер картки не може бути порожнім полем і повинен містити рівно 16 цифр.");
                }
                _cardNumber = value;
            }
        }
        public string ExpiryDate
        {
            get => _expiryDate;
            set
            {
                // Перевірка формату MM/YY (5 символів, слеш на 3 позиції)
                if (string.IsNullOrWhiteSpace(value) ||
                    value.Length != 5 ||
                    value[2] != '/' ||
                    !value.Remove(2, 1).All(char.IsDigit))
                {
                    throw new ArgumentException("Невірний формат дати. Використовуйте MM/YY (наприклад, 12/25)");
                }

                // Розбиваємо на місяць/рік
                string[] parts = value.Split('/');
                int month = int.Parse(parts[0]);
                int year = 2000 + int.Parse(parts[1]); // Перетворюємо YY у 20YY

                // Перевірка коректності місяця
                if (month < 1 || month > 12)
                {
                    throw new ArgumentException("Місяць має бути від 01 до 12");
                }

                // Перевірка чи дата не прострочена
                DateTime expiryDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                if (expiryDate < DateTime.Now)
                {
                    throw new ArgumentException("Термін дії картки вже закінчився");
                }

                _expiryDate = value;
            }
        }
        public string CVV
        {
            get => _cvv;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length != 3 || !value.All(char.IsDigit))
            {
                    throw new ArgumentException("CVV-код не може бути порожнім рядком і повинен містити рівно 3 цифри.");
                }
                _cvv = value;
            }
        }
        public string CardHolderName
        {
            get => _cardHolderName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Ім'я власника картки не може бути порожнім.");
                }
                _cardHolderName = value;
            }
        }
        //*
        public PaymentDetails(string cardNumber, string cardHolderName, string expiryDate, string cvv)
        {
            CardNumber = cardNumber;
            CardHolderName = cardHolderName;
            ExpiryDate = expiryDate;
            CVV = cvv;
        }
    }
}
