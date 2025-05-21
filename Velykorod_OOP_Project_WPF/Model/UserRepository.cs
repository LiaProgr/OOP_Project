using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTickets
{
    public static class UserRepository
    {
        public static User CurrentUser { get;  set; }//*
        //загальний список наших юзерів*
        public static List<User> users = new List<User>
        {
        new Admin {Username = "Pavlo_Schuchevich", Password = "NPP&1413kin" },
        new RegisteredUser { Username = "Angelina_Polischuk", Password = "NAP&1413kin" },
         
        };
        //*
        public static User GetUserByCredentials(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException("Логін і пароль не можуть бути порожніми");
            var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);
            //if(user==null)
            //{ throw new ArgumentNullException("Невірний логін або пароль"); }
            // При успішній автентифікації зберігаємо користувача
            if (user != null)
            {
                CurrentUser = user;
            }

            return user;
        }
        //метод для отримання поточного користувача*
        public static User GetCurrentUser()
        {
            return CurrentUser;
        }
        //список зареєстрованих користувачів*
        public static List<RegisteredUser> GetRegisteredUsers()
        {
            return users.OfType<RegisteredUser>().ToList();
        }
        // зберігає корзину реєствованого користувача при виході*
        public static void SaveCartBeforeLogout()
        {
            var user = GetCurrentUser() as RegisteredUser;
            user?.SavePurchasedTickets(); // Додаємо збереження квитків
            user?.Cart.SaveCart(); // Зберігаємо перед виходом
                                   // Інша логіка виходу...
        }
        // Зберегти дані всіх зареєстрованих користувачів
        public static void SaveAllUsersData()
        {
            foreach (var user in users.OfType<RegisteredUser>())
            {
                EventRepository.SaveEvents();
                user.SavePurchasedTickets(); // Зберігаємо куплені квитки
                user.Cart.SaveCart(); // Зберігаємо кошик (якщо потрібно)
            }
        }


    }
}
