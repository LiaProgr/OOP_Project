using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTickets
{
    public class PaymentProcessor
    {
        //*поки що так
        public static bool ProcessPayment(PaymentDetails payment, decimal amount)
        {
            //Console.WriteLine($"Processing payment of {amount: C} using card ending with {payment.CardNumber}");
            return true; // Simulate

        }
    }
}

