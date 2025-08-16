using TypicalTechTools.Models.Data;
using TypicalTechTools.Models.DTOs;

namespace TypicalTechTools.Models.Repositories
{
    /// <summary>
    /// Repository handling authentication and user account creation logic.
    /// </summary>
    /// <remarks>
    /// - Provides methods to authenticate a user and create a new user account.
    /// - Passwords are hashed using BCrypt.
    /// - New users are assigned the default "GUEST" role.
    /// </remarks>
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly TypicalTechToolsDBContext _context;
        public AuthenticationRepository(TypicalTechToolsDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Verifies the provided login credentials against stored user records.
        /// </summary>
        /// <param name="loginDTO">User credentials (username and password)</param>
        /// <returns>Authenticated AppUser object or null if authentication fails</returns>
        public AppUser Authenticate(LoginDTO loginDTO)
        {
            var userDetails = _context.AppUsers.Where(u => u.UserName == loginDTO.UserName)
                                                           .FirstOrDefault();
            if (userDetails == null)
            {
                return null;
            }

            if (BCrypt.Net.BCrypt.EnhancedVerify(loginDTO.Password, userDetails.Password))
            {
                return userDetails;
            }
            return null;
        }

        /// <summary>
        /// Creates a new user account if the username is not already taken.
        /// </summary>
        /// <param name="userDTO">New user registration details</param>
        /// <returns>Created AppUser object or null if username already exists</returns>
        public AppUser CreateUser(CreateUserDTO userDTO)
        {
            //Check to see if the database contains a user with the provided username
            var userDetails = _context.AppUsers.Where(u => u.UserName.Equals(userDTO.UserName))
                                               .FirstOrDefault();
            //If a user was found, return null to indicate you cannot create another user
            //with that username.
            if (userDetails != null)
            {
                return null;
            }
            //Map the values from the DTO to a proper user object, ensuring to hash and salt
            //the password as this is done.
            AppUser user = new AppUser
            {
                UserName = userDTO.UserName,
                Password = BCrypt.Net.BCrypt.EnhancedHashPassword(userDTO.Password),
                Role = "GUEST"
            };
            //Pass the changes to the database and finalise the save
            _context.AppUsers.Add(user);
            _context.SaveChanges();
            //return the created user's details to the caller.
            return user;
        }
    }
}
