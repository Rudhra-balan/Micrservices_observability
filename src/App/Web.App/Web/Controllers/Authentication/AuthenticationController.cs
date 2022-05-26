using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Application.Services.Interface;
using DomainCore.Models.Login;
using Microsoft.Extensions.Logging;

using BuildingBlocks.Metrics;
using System.Threading;
using BuildingBlocks;

namespace Web.Controllers.Authentication
{
    public class AuthenticationController : Controller
    {
        #region Constructor
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IAuthentication _authentication;
        private readonly IIdentityContext _identityContext;
        private readonly IGenericMemoryCache _genericMemoryCache;
      
        public AuthenticationController(ILogger<AuthenticationController> logger,IAuthentication authentication, IIdentityContext identityContext, 
            IGenericMemoryCache genericMemoryCache)
        {
            _logger = logger;
         
             _authentication = authentication;
            _identityContext = identityContext;
            _genericMemoryCache = genericMemoryCache;
            Interlocked.Increment(ref DiagnosticsConstant.Requests);
        }

        #endregion

        #region Member Fields



        #endregion

        #region Public Action Methods

        /// <summary>
        ///     Load Login screen
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Index(string returnUrl, string errorMessage)
        {

            using (_logger.BeginScope("Loading login page"))
            {
                ViewBag.ReturnUrl = returnUrl;
                LoginUserViewModel loginUserViewModel = new LoginUserViewModel
                {
                    UserName = string.Empty,
                    Password = string.Empty,

                };
                loginUserViewModel.Message = errorMessage;

                return View(loginUserViewModel);
            }

        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LoginUserViewModel model, string returnUrl)
        {

            using (_logger.BeginScope("Authentication user using {@model}", model))
                try
                {

                    if (!ModelState.IsValid)
                    {
                        _logger.LogError("Invalid Request. Please try again");
                        return Index(string.Empty, "Invalid Request. Please try again");
                    }
                    var token = await _authentication.Login(model.UserName, model.Password);
                    var claimsPrincipal = _identityContext.CreateClaimsPrincipal(token.AccessToken);
                    var authenticationProperties = _identityContext.CreateAuthenticationProperties(token.AccessToken);
                    await HttpContext.SignInAsync(claimsPrincipal, authenticationProperties);
                    HttpContext.User = claimsPrincipal;
                    return RedirectToLocal(returnUrl);
                }
                catch (Exception ex)
                {

                    _logger.LogError(ex, "An Error Occured During the Authentication.");
                    model.Message = ex.Message;
                    return View(model);
                }

        }

        /// <summary>
        /// Log off
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            HttpContext.SignOutAsync();
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }


        #endregion

        private ActionResult RedirectToLocal(string returnUrl)
        {
          
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}