using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Interfaces
{
    public interface IMemberRepository
    {
        public Task<Member> Login(string email, string password);
        public Task<List<Member>> GetMembers();
        public Task<List<Member>> GetMembers(string query);
        public Task<Member> GetMember(int id);
        public Task AddMember(Member member);
        public Task UpdateMember(Member member);
        public Task DeleteMember(int id);
    }
}
