using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class MemberDAO
    {

        public static List<Member> GetMembers()
        {
            var listMembers = new List<Member>();
            try
            {
                using (var context = new EStoreContext())
                {
                    listMembers = context.Members.ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listMembers;
        }

        public static List<Member> Search(string keyword)
        {
            var listMembers = new List<Member>();
            try
            {
                using (var context = new EStoreContext())
                {
                    listMembers = context.Members
                        .Where(c => c.CompanyName.Contains(keyword) || c.Email.Contains(keyword))
                        .ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listMembers;
        }

        public static Member FindMemberById(int MemberId)
        {
            var Member = new Member();
            try
            {
                using (var context = new EStoreContext())
                {
                    Member = context.Members.SingleOrDefault(c => c.MemberId == MemberId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Member;
        }

        public static Member FindMemberByEmail(string email)
        {
            var Member = new Member();
            try
            {
                using (var context = new EStoreContext())
                {
                    Member = context.Members.FirstOrDefault(c => c.Email == email);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Member;
        }

        public static void SaveMember(Member Member)
        {
            try
            {
                using (var context = new EStoreContext())
                {
                    context.Members.Add(Member);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void UpdateMember(Member Member)
        {
            try
            {
                using (var context = new EStoreContext())
                {
                    context.Entry(Member).State =
                        Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void DeleteMember(Member Member)
        {
            try
            {
                using (var context = new EStoreContext())
                {
                    var MemberToDelete = context
                        .Members
                        .SingleOrDefault(c => c.MemberId == Member.MemberId);
                    context.Members.Remove(MemberToDelete);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
