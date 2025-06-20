using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;

namespace DataAccess.Repositories
{
    public interface IMemberRepository
    {
        void SaveMember(Member Member);
        Member GetMemberById(int id);
        Member GetMemberByEmail(string email);
        List<Member> GetMembers();
        List<Member> Search(string keyword);
        void UpdateMember(Member Member);
        void DeleteMember(Member Member);
        List<Order> GetOrders(int MemberId);
    }
}
