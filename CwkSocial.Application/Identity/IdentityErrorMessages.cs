using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CwkSocial.Application.Identity
{
    public class IdentityErrorMessages
    {
        public const string NonExistentIdentityUser = "Unable to find a user with the specified username.";

        public const string IncorrectPassword = "Provided password is not valid. Login failed.";

        public const string IdentityUserAlreadyExists = "There is already user with this username";


    }
}
