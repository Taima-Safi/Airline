using Airline.Shared.Enum;

namespace Airline.Database.Model;

public class UserModel : BaseModel
{
    public string Email { get; set; }
    public UserType Type { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string PhoneNumber { get; set; }
    public string HashPassword { get; set; }
    public string PassportNumber { get; set; }
    public string CodePhoneNumber { get; set; }

    public ICollection<BookModel> Books { get; set; }
}
