using BusinessObjects;
using DataAccess.DAOs;
using DataAccess.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        public Task AddMember(Member member) => MemberDAO.Instance.AddMember(member);
        public Task DeleteMember(int id) => MemberDAO.Instance.DeleteMember(id);
        public Task<Member> GetMember(int id) => MemberDAO.Instance.GetMember(id);
        public Task<List<Member>> GetMembers() => MemberDAO.Instance.GetMembers();
        public Task<List<Member>> GetMembers(string query) => MemberDAO.Instance.GetMembers(query);
        public Task<Member> Login(string email, string password) => MemberDAO.Instance.Login(email, password);
        public Task UpdateMember(Member member) => MemberDAO.Instance.UpdateMember(member);
    }
}
