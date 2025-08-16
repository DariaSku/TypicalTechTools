using TypicalTechTools.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TypicalTechTools.Models.Data;
using TypicalTechTools.Models.Repositories;
using TypicalTechTools.Models.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;


namespace TypicalTools.Controllers
{
    /// <summary>
    /// Handles user authentication, login, logout, and registration.
    /// </summary>
    /// <remarks>
    /// Uses cookie-based authentication with claims for Role and UserName.
    ///
    /// Roles:
    /// - ADMIN: can upload and manage warranty files, manage products, and moderate comments.
    /// - GUEST (authenticated): can upload warranty files? leave comments and manage them during the session.
    ///
    /// Role checks are performed using User.Identity.IsAuthenticated and User.IsInRole("ADMIN") in views and controllers.
    /// </remarks>

    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationRepository _repository;

        public AuthenticationController(IAuthenticationRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Displays the login page with optional return URL.
        /// </summary>
        public IActionResult AdminLogin([FromQuery] string ReturnUrl)
        {
            LoginDTO login = new LoginDTO
            {
                ReturnUrl = string.IsNullOrWhiteSpace(ReturnUrl) ? "/Product/Index" : ReturnUrl
            };
            return View(login);
        }

        ////// <summary>
        /// Authenticates the user and sets up claims for cookie-based login.
        /// </summary>
        /// <param name="loginDTO">Login credentials and return URL.</param>
        /// <returns>Redirect to ReturnUrl on success, or back to login view with message.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid) 
            {
                ViewBag.LoginMessage = "Please fill in all required fields.";
                return View(loginDTO);
            }

            //Pass the login details to the repository to be validated
            var user = _repository.Authenticate(loginDTO);

            //If the validation failed, create an error message in the Viewbag
            //and return to the login page
            if (user == null)
            {
                ViewBag.LoginMessage = "UserName or Password Incorrect";
                return View("AdminLogin", loginDTO); // if they arent allowed to login
            }

            //If the validation passed, store a value in the session data to mark
            //the user as logged in and redirect to the home page.
            //HttpContext.Session.SetString("Authenticated", "true");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role,user.Role ),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("Department","Marketing")
            };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme));

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = true,

                RedirectUri = loginDTO.ReturnUrl
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                principal, authProperties);

            //return RedirectToAction("Index", "Product");
            return Redirect(loginDTO.ReturnUrl);
        }

        /// <summary>
        /// Logs out the user and clears the authentication cookie.
        /// </summary>
        /// <returns>Redirect to Product Index page.</returns>
        public ActionResult LogOff()
        {
            //Change the login state in the sessin data to false then redirect to the home page.
            //HttpContext.Session.SetString("Authenticated", "false");
            HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Product");
        }

        /// <summary>
        /// Displays the user registration form.
        /// </summary>
        /// <returns>Create view.</returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates a new user if passwords match and username is unique.
        /// </summary>
        /// <param name="userDTO">New user registration data.</param>
        /// <returns>Create view with confirmation or error message.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateUserDTO userDTO)
        {
            //Check that the User password and confirmation match
            if (userDTO.Password.Equals(userDTO.PasswordConfirmation) == false)
            {
                //If not send a message to the viewbag and reload the create page
                ViewBag.CreateUserError = "Password and Confirmation do not match";
                return View(userDTO);
            }
            //Try to add a user by passing it to the repository and storing the result
            var user = _repository.CreateUser(userDTO);
            //If the create failed, generate another error message
            if (user == null)
            {
                ViewBag.CreateUserError = "Username already exists. Choose a different username.";
                return View(userDTO);
            }
            //Create a confirmation message
            ViewBag.CreateUserConfirmation = "New User Added.";
            //Clear the model so it does not get added to the form.
            ModelState.Clear();
            return View();
            //return RedirectToAction("AdminLogin", "Authentication");
        }
    }
}
