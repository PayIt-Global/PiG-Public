using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayItGlobal.DTOs.Shared
{
    public record UserCreateModel(Guid Id, string FirstName, string LastName, string Avatar);

    public record UserViewModel(Guid Id, string FirstName, string LastName, string Avatar, DateTime LastSeen);

    public record UserUpdatedModel(Guid Id, Guid? TypingToUserId, DateTime LastSeen);

}
