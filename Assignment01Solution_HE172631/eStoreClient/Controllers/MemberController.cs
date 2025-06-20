using eStoreClient.Models;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using eStoreClient.Utils;
using BusinessObject.Models;

namespace eStoreClient.Controllers
{
    public class MemberController : Controller
    {

        private readonly HttpClient client = null;
        private string MemberApiUrl = "";
        public MemberController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            MemberApiUrl = "https://localhost:7139/api/Member";
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("USERID");
            string Role = HttpContext.Session.GetString("USERNAME");

            if (userId == null)
            {
                //TempData["ErrorMessage"] = "You must login to access this page.";
                return RedirectToAction("Index", "Home");
            }
            else if (Role != "admin@estore.com")
            {
                TempData["ErrorMessage"] = "You don't have permission to access this page.";
                return RedirectToAction("Profile", "Member");
            }

            List<Member> listMembers = await ApiHandler.DeserializeApiResponse<List<Member>>(MemberApiUrl, HttpMethod.Get);

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            return View(listMembers);
        }
        public async Task<IActionResult> Search(string keyword)
        {
            List<Member> listMembers = await ApiHandler.DeserializeApiResponse<List<Member>>(MemberApiUrl + "/Search/" + keyword, HttpMethod.Get);

            ViewData["keyword"] = keyword;

            return View("Index", listMembers);
        }
        [HttpGet]
        public IActionResult Create()
        {
            int? userId = HttpContext.Session.GetInt32("USERID");
            string Role = HttpContext.Session.GetString("USERNAME");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to access this page.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["ErrorMessage"] = "You don't have permission to access this page.";
                return RedirectToAction("Profile", "Member");
            }

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Member memberRequest)
        {
            Member member = await ApiHandler.DeserializeApiResponse<Member>(MemberApiUrl + "/Email/" + memberRequest.Email, HttpMethod.Get);
            if (memberRequest.Email.Equals("admin@estore.com") ||
                (member != null && member.MemberId != 0))
            {
                TempData["ErrorMessage"] = "Email already exists.";
                return RedirectToAction("Create");
            }

            await ApiHandler.DeserializeApiResponse(MemberApiUrl, HttpMethod.Post, memberRequest);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            int? userId = HttpContext.Session.GetInt32("USERID");
            string Role = HttpContext.Session.GetString("USERNAME");

            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to access this page.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["ErrorMessage"] = "You don't have permission to access this page.";
                return RedirectToAction("Profile", "Member");
            }

            Member member = await ApiHandler.DeserializeApiResponse<Member>(MemberApiUrl + "/" + id, HttpMethod.Get);

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            return View(member);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Member memberRequest)
        {
            await ApiHandler.DeserializeApiResponse(MemberApiUrl + "/" + memberRequest.MemberId, HttpMethod.Put, memberRequest);

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            int? userId = HttpContext.Session.GetInt32("USERID");
            string Role = HttpContext.Session.GetString("USERNAME");

            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to access this page.";
                return RedirectToAction("Index", "Home");
            }
            else if (Role != "admin@estore.com")
            {
                TempData["ErrorMessage"] = "You don't have permission to access this page.";
                return RedirectToAction("Profile", "Member");
            }

            Member member = await ApiHandler.DeserializeApiResponse<Member>(MemberApiUrl + "/" + id, HttpMethod.Get);

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            return View(member);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Member memberRequest)
        {
            HttpResponseMessage response = await client.DeleteAsync(MemberApiUrl + "/" + memberRequest.MemberId);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");
            else
                return View();
        }
        // Customer
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            int? userId = HttpContext.Session.GetInt32("USERID");
            string Role = HttpContext.Session.GetString("USERNAME");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to access this page.";
                return RedirectToAction("Index", "Home");
            }
            else if (Role == "")
            {
                TempData["ErrorMessage"] = "You don't have permission to access this page.";
                return RedirectToAction("Index", "Member");
            }
            int userIdmember = HttpContext.Session.GetInt32("USERID").Value;

            Member member = await ApiHandler.DeserializeApiResponse<Member>(MemberApiUrl + "/" + userIdmember, HttpMethod.Get);

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            return View(member);
        }


        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            string Role = HttpContext.Session.GetString("USERNAME");
            int userId = HttpContext.Session.GetInt32("USERID").Value;

            Member member = await ApiHandler.DeserializeApiResponse<Member>(MemberApiUrl + "/" + userId, HttpMethod.Get);

            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to access this page.";
                return RedirectToAction("Index", "Home");
            }
            else if (Role != member.Email)
            {
                TempData["ErrorMessage"] = "You don't have permission to access this page.";
                return RedirectToAction("Index", "Member");
            }


            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            return View(member);
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile(Member memberRequest)
        {
            int? userId = HttpContext.Session.GetInt32("USERID");
            if (userId == null)
                return RedirectToAction("Index", "Home");

            memberRequest.MemberId = userId.Value;
            await ApiHandler.DeserializeApiResponse(MemberApiUrl + "/" + memberRequest.MemberId, HttpMethod.Put, memberRequest);

            TempData["SuccessMessage"] = "Edit profile information successfully.";

            return RedirectToAction("Profile", TempData);
        }
    }
}
