
namespace BookAPI.Models
{
    // Create  a User class with the following properties:
    // Id (int) - a unique identifier for the user.
    // Username (string) - the username of the user.
    // Password (string) - the password of the user.
    // Email (string) - the email address of the user.
    // MobileNumber (string) - the mobile number of the user.

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }

    }
}