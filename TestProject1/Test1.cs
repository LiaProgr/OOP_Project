
using EventTickets;
namespace TestProject1
{
    [TestClass]
    public class Test1
    {
        private readonly Admin _admin = new Admin { Username = "admin1", Password = "adminpass" };
       public  string TestUsername = "testuser";
        public string TestPassword = "testpass";

        private RegisteredUser _testUser;

        // Тест 1: Успішний вхід
        [TestMethod]
        public void Login_Success()
        {
            Admin _admin = new Admin { Username = "admin1", Password = "adminpass" };
            UserRepository.users.Add(_admin);
            var user = UserRepository.GetUserByCredentials("admin1", "adminpass");
            Assert.IsNotNull(user);
            Assert.IsInstanceOfType(user, typeof(Admin));
            UserRepository.users.Remove(_admin);
        }
        // Тест 2: Порожній логін
        [TestMethod]
        public void Login_EmptyUsername()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                UserRepository.GetUserByCredentials("", "adminpass"));
        }
        // Тест 3: Додавання події (успіх)
        [TestMethod]
        public void AddEvent_Success()
        {  
            // Act
            _admin.AddEvent("Концерт", Genre.Концерт, new DateOnly(2025, 12, 12),
                new TimeOnly(18, 0), City.Київ, "Палац Спорту", "500", "100", "image.jpg");
            //Assert
            Assert.AreEqual(1, EventRepository.Events.Count);
            EventRepository.Events.Clear();
        }
        // Тест 4: Додавання події (помилка - пусті поля)
        [TestMethod]
        public void AddEvent_EmptyFields()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
                _admin.AddEvent("", Genre.Концерт, new DateOnly(2025, 12, 12),
                    new TimeOnly(18, 0), City.Київ, "", "500", "100", ""));
        }
        // Тест 5: Видалення події (успіх)
        [TestMethod]
        public void RemoveEvent_Success()
        {
            EventRepository.Events.Clear();
            // Arrange
            var ev = new Event("Тест", Genre.Театр, new DateOnly(2025, 1, 1),
                new TimeOnly(19, 0), City.Львів, "Театр", 200, 50, "img.jpg");

            EventRepository.AddEvent(ev);

            // Act
            _admin.RemoveEvent(ev);

            // Assert
            Assert.AreEqual(0, EventRepository.Events.Count);
            EventRepository.Events.Clear();
        }
        // Тест 6: Редагування події (успіх)
        [TestMethod]
        public void EditEvent_Success()
        {
            // Arrange
            EventRepository.Events.Clear(); // Очищаємо перед тестом
            var ev = new Event("Стара назва", Genre.Комедія, new DateOnly(2025, 1, 1),
                new TimeOnly(19, 0), City.Одеса, "Старе місце", 100, 20, "old.jpg");

            EventRepository.AddEvent(ev);

            // Act
            _admin.EditEvent(ev, "Нова назва", Genre.Театр, new DateOnly(2025, 2, 2),
                new TimeOnly(20, 0), City.Київ, "Нове місце", "150", "30", "new.jpg");

            // Assert
            Assert.AreEqual("Нова назва", ev.Name); // Використовуйте Assert.AreEqual замість Assert.Equalsзщ
            EventRepository.Events.Clear();
        }
       
        // ЗК.1.1 Успішна авторизація
        [TestMethod]
        public void Login_ValidCredentials_ReturnsUser()
        {
             TestUsername = "testuser";
             TestPassword = "testpass";
        _testUser = new RegisteredUser
           {
               Username = TestUsername,
               Password = TestPassword
          };
            UserRepository.users.Add(_testUser);
            // Act
            var result = UserRepository.GetUserByCredentials(TestUsername, TestPassword);

            // Assert
            //Assert.IsNotNull(result);
            Assert.AreEqual(TestUsername, result.Username);
            Assert.IsInstanceOfType(result, typeof(RegisteredUser));
        }

  
        // ЗК.1.2 Поле логін не може бути порожнім
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Login_EmptyUsername_ThrowsException()
        {
            // Act
            UserRepository.GetUserByCredentials("", TestPassword);
        }

        // ЗК.1.3 Поле пароль не може бути порожнім
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Login_EmptyPassword_ThrowsException()
        {
            // Act
            UserRepository.GetUserByCredentials(TestUsername, "");
        }

        // ЗК.2.1 Успішно збережений список у форматі.json
        [TestMethod]
        public void SaveEventsToJson_ValidEvents_Success()
        {
            var user = new RegisteredUser();
            var events = new List<Event>
        {
            new Event("Журавлі", Genre.Комедія, DateOnly.FromDateTime(DateTime.Now),
                     TimeOnly.FromDateTime(DateTime.Now), City.Київ, "Палац Україна",
                     100, 50, "image.jpg")
        };

            user.SaveEventsToJson(events, "test.json");
            Assert.IsTrue(File.Exists("test.json"));
            File.Delete("test.json");
        }

        // ЗК.2.2 Колекція пуста
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SaveEventsToJson_EmptyList_ThrowsException()
        {
            var user = new RegisteredUser();
            user.SaveEventsToJson(new List<Event>(), "test.json");
        }
        // ЗК.3.1 Гарантоване успішне додавання квитка до кошика
        [TestMethod]
        public void BuyTicket_ValidQuantity_AddsToCart()
        {
            var user = new RegisteredUser();
            var testEvent = new Event("Test", Genre.Концерт, DateOnly.FromDateTime(DateTime.Now),
                             TimeOnly.FromDateTime(DateTime.Now), City.Київ, "Venue", 100, 10, "img.jpg");
            EventRepository.AddEvent(testEvent);

            user.AddToCart(testEvent, 2);
            Assert.AreEqual(1, user.Cart.GetEventsInCart().Count);
            EventRepository.RemoveEvent(testEvent);
        }

        // ЗК.3.2 Не коректне значення кількості квитків
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BuyTicket_InvalidQuantity_ThrowsException()
        {
            var user = new RegisteredUser();
            var testEvent = new Event("Test", Genre.Концерт, DateOnly.FromDateTime(DateTime.Now),
                             TimeOnly.FromDateTime(DateTime.Now), City.Київ, "Venue", 100, 3, "img.jpg");
            EventRepository.AddEvent(testEvent);

            user.AddToCart(testEvent, 4);
            EventRepository.RemoveEvent(testEvent);
        }

        // ЗК.4.1 Додано до кошика успішно
        [TestMethod]
        public void AddTicket_ValidTicket_AddsToCart()
        {
            var cart = new ShoppingCart();
            var testEvent = new Event("Test", Genre.Концерт, DateOnly.FromDateTime(DateTime.Now),
                             TimeOnly.FromDateTime(DateTime.Now), City.Київ, "Venue", 100, 10, "img.jpg");
            var ticket = new Ticket(testEvent, 2);

            cart.AddTicket(ticket);
            Assert.AreEqual(1, cart.GetEventsInCart().Count);
        }

        // ЗК.5.1 Кошик очищено
        [TestMethod]
        public void ClearCart_WithItems_ClearsCart()
        {
            var cart = new ShoppingCart();
            var testEvent = new Event("Test", Genre.Концерт, DateOnly.
                FromDateTime(DateTime.Now),
                         TimeOnly.FromDateTime(DateTime.Now), City.Київ, "Venue", 100, 10, "img.jpg");
            cart.AddTicket(new Ticket(testEvent, 1));

            cart.ClearCart();
            Assert.AreEqual(0, cart.GetEventsInCart().Count);
        }

        // ЗК.6.1 Не можна оплатити (пустий кошик)
        [TestMethod]
        public void Checkout_EmptyCart_ThrowsException()
        {
            // Arrange

            var user = new RegisteredUser();
            //user.Cart.ClearCart();
            var payment = new PaymentDetails("4111111111111111", "TEST USER", "12/25", "123");

            // Act & Assert
            var ex = Assert.ThrowsException<InvalidOperationException>(() => user.Checkout(payment));
            Assert.AreEqual("Кошик порожній.", ex.Message);
        }

        // ЗК.7.1 Оплата здійснена успішно
        [TestMethod]
        public void ProcessPayment_ValidPayment_ReturnsTrue()
        {
            var result = PaymentProcessor.ProcessPayment(
                new PaymentDetails("1234567890123456", "Irina Willms", "12/25", "123"), 100);
            Assert.IsTrue(result);
        }

        // ЗК.7.2 Введіть ім'я власника картки
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Payment_CardHolderNameEmpty_ThrowsException()
        {
            var payment = new PaymentDetails("1234567890123456", "", "12/25", "123");
        }

        // ЗК.7.3 Введіть номер картки з 16 символів (15 symbols)
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Payment_CardNumberTooShort_ThrowsException()
        {
            var payment = new PaymentDetails("123456789012345", "Irina Willms", "12/25", "123");
        }

        // ЗК.7.4 Введіть номер картки з 16 символів (17 symbols)
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Payment_CardNumberTooLong_ThrowsException()
        {
            var payment = new PaymentDetails("12345678901234567", "Irina Willms", "12/25", "123");
        }

        // ЗК.7.5 Номер картки не може бути порожнім
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Payment_CardNumberEmpty_ThrowsException()
        {
            var payment = new PaymentDetails("", "Irina Willms", "12/25", "123");
        }

        // ЗК.7.6 Введіть дату у форматі MM/YY
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Payment_InvalidExpiryFormat_ThrowsException()
        {
            var payment = new PaymentDetails("1234567890123456", "Irina Willms", "25/1", "123");
        }

        // ЗК.7.7 Поле дати не може бути порожнім
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Payment_ExpiryEmpty_ThrowsException()
        {
            var payment = new PaymentDetails("1234567890123456", "Irina Willms", "", "123");
        }

        // ЗК.7.8 Введіть CVV-код з 3 символів (4 symbols)
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Payment_CVVTooLong_ThrowsException()
        {
            var payment = new PaymentDetails("1234567890123456", "Irina Willms", "12/25", "1233");
        }

        // ЗК.7.9 Введіть CVV-код з 3 символів (2 symbols)
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Payment_CVVTooShort_ThrowsException()
        {
            var payment = new PaymentDetails("1234567890123456", "Irina Willms", "12/25", "12");
        }

        // ЗК.7.10 Введіть CVV-код з 3 символів (empty)
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Payment_CVVEmpty_ThrowsException()
        {
            var payment = new PaymentDetails("1234567890123456", "Irina Willms", "12/25", "");
        }
    }
}
