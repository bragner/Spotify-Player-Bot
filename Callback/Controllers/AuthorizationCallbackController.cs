using Microsoft.AspNetCore.Mvc;
using SpotifyPlayer.Services.Authorization;
using System.Linq;

namespace SpotifyPlayer.Callback.Controllers
{
    [Route("callback")]
    public class AuthorizationCallbackController : Controller
    {
        private readonly IAuthorizationMapper _authorizationMapper;

        public AuthorizationCallbackController(IAuthorizationMapper authorizationMapper)
        {
            _authorizationMapper = authorizationMapper;
        }
        public IActionResult Callback()
        {
            var code = Request.Query["code"].FirstOrDefault();
            ViewBag.Code = _authorizationMapper.Set(code);
            return View();
        }
    }
}