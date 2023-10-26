using System.Text.RegularExpressions;

namespace Meble.Server.Helpers
{
    public class ValidationHelpers
    {
        public static (bool isValid, string? errorMessage) IsPhoneNumberValid(string phoneNumber)
        {
            string phoneRegex = @"^\+\d{2,4} \d{3}-\d{3}-\d{3}$";
            if (Regex.IsMatch(phoneNumber, phoneRegex))
            {
                return (true, null); 
            }
            else
            {
                return (false, "Invalid phone number format");
            }
        }
        public static (bool isValid, string? errorMessage) IsEmailValid(string email)
        {
            string emailRegex = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w{2,}$";
            if (Regex.IsMatch(email, emailRegex))
            {
                return (true, null); 
            }
            else
            {
                return (false, "Invalid email address"); 
            }
        }
        public static (bool isValid, string? errorMessage) IsPasswordValid(string password)
        {
            string passwordRegex = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*\W).{8,16}$";
            if (Regex.IsMatch(password, passwordRegex))
            {
                return (true, null); 
            }
            else
            {
                return (false, "Invalid password format"); 
            }
        }
        public static (bool isValid, string? errorMessage) IsPriceValid(decimal price)
        {
            if (price > 0)
            {
                return (true, null); 
            }
            else
            {
                return (false, "Price must be greater than 0"); 
            }
        }
    }
}
