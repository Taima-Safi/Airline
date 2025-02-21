namespace Airline.Database.Model;

public class UserModel : BaseModel
{
    public string Email { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string PhonNumber { get; set; }
    public string PassportNumber { get; set; }
    public string CodePhoneNumber { get; set; }
}
