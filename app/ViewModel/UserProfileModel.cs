using app.DTO;
using app.DTO.User;
using app.Models;


namespace app.ViewModel;

public class UserProfileModel(UserPageDto userPageDto)
{
    public UserPageDto  User { get; init; }
}