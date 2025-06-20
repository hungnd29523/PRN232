using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eStoreClient.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using eStoreClient.Utils;

namespace eStoreClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient client = null;
        private string MemberApiUrl = "";

        public HomeController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            MemberApiUrl = "https://localhost:7139/api/Member";
        }
        [HttpGet]
        public IActionResult Index()
        {
            string User = HttpContext.Session.GetString("USERNAME");
            if (User == null)
            {
                ////TempData["ErrorMessage"] = "You must login to access this page.";
                return View();
            }
            if (User == "admin@estore.com")
                return RedirectToAction("Index", "Member");
            else if (User != "admin@estore.com")
                return RedirectToAction("Profile", "Member");

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(BusinessObject.Models.Member loginRequest)
        {
            HttpResponseMessage response = await client.GetAsync(MemberApiUrl);
            string stringData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();

            BusinessObject.Models.Member admin = new BusinessObject.Models.Member
            {
                Email = config["Credentials:Email"],
                Password = config["Credentials:Password"],

            };

            List<BusinessObject.Models.Member> listMember = JsonSerializer.Deserialize<List<BusinessObject.Models.Member>>(stringData, options);
            listMember.Add(admin);
            BusinessObject.Models.Member account = listMember.Where(c => c.Email == loginRequest.Email && c.Password == loginRequest.Password).FirstOrDefault();

            if (account != null)
            {
                HttpContext.Session.SetInt32("USERID", account.MemberId);
                HttpContext.Session.SetString("USERNAME", account.Email);
                if (account.Email == "admin@estore.com")
                    return RedirectToAction("Index", "Member");
                else
                    return RedirectToAction("Profile", "Member");
            }
            else
            {
                ViewData["ErrorMessage"] = "Email or password is invalid.";
                return View();
            }
        }
        [HttpGet]
        public IActionResult Register()
        {
            string Role = HttpContext.Session.GetString("ROLE");
            if (Role == "Admin")
                return RedirectToAction("Index", "Member");
            else if (Role == "Customer")
                return RedirectToAction("Profile", "Member");

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(BusinessObject.Models.Member memberRequest)
        {
            BusinessObject.Models.Member member = await ApiHandler.DeserializeApiResponse<BusinessObject.Models.Member>(MemberApiUrl + "/Email/" + memberRequest.Email, HttpMethod.Get);
            if (memberRequest.Email.Equals("admin@estore.com") ||
                (member != null && member.MemberId != 0))
            {
                ViewData["ErrorMessage"] = "Email already exists.";
                return View("Register");
            }

            await ApiHandler.DeserializeApiResponse(MemberApiUrl, HttpMethod.Post, memberRequest);

            ViewData["SuccessMessage"] = "Register new account successfully.";
            return View("Index");
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

    }
}
