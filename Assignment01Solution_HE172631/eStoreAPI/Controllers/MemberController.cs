using BusinessObject.DTO;
using DataAccess.Repositories.Repository;
using DataAccess.Repositories;
using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;

namespace eStoreAPI.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private IMemberRepository repository = new MemberRepository();

        [HttpGet]
        public ActionResult<IEnumerable<Member>> GetCustomers() => repository.GetMembers();

        [HttpGet("SearchByUnitPrice/{keyword}")]
        public ActionResult<IEnumerable<Member>> Search(string keyword) => repository.Search(keyword);

        [HttpGet("{id}")]
        public ActionResult<Member> GetCustomerById(int id) => repository.GetMemberById(id);

        [HttpGet("Email/{email}")]
        public ActionResult<Member> GetCustomerByEmail(string email) => repository.GetMemberByEmail(email);

        [HttpPost]
        public IActionResult PostMember(MemberRequest memberreq)
        {
            var member = new Member
            {
                CompanyName = memberreq.CompanyName,
                Email = memberreq.Email,
                City = memberreq.City,
                Country = memberreq.Country,
                Password = memberreq.Password,


            };
            repository.SaveMember(member);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteMember(int id)
        {
            var c = repository.GetMemberById(id);
            if (c == null)
            {
                return NotFound();
            }
            repository.DeleteMember(c);
            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult PutMember(int id, MemberRequest memberReq)
        {
            var mTmp = repository.GetMemberById(id);
            if (mTmp == null)
            {
                return NotFound();
            }

            mTmp.CompanyName = memberReq.CompanyName;
            mTmp.Email = memberReq.Email;
            mTmp.City = memberReq.City;
            mTmp.Country = memberReq.Country;

            if (memberReq.Password != null && mTmp.Password != memberReq.Password)
            {
                mTmp.Password = memberReq.Password;
            }

            repository.UpdateMember(mTmp);
            return NoContent();
        }
    }
}
