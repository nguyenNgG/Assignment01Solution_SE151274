using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAOs
{
    public class MemberDAO
    {
        private static MemberDAO instance = null;
        private static readonly object instanceLock = new object();
        private MemberDAO() { }

        public static MemberDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new MemberDAO();
                    }
                    return instance;
                }
            }
        }

        public async Task<Member> GetMember(int id)
        {
            FStoreDBContext db = new FStoreDBContext();
            Member member = null;
            member = await db.Members.FirstOrDefaultAsync(m => m.MemberId == id);
            return member;
        }

        public async Task<List<Member>> GetMembers(string query)
        {
            FStoreDBContext db = new FStoreDBContext();
            query = query.ToLowerInvariant();
            List<Member> members = await db.Members.Where(m =>
            m.MemberId.ToString().Contains(query)
            || m.Email.ToLowerInvariant().Contains(query)
            || m.CompanyName.ToLowerInvariant().Contains(query)
            || m.City.ToLowerInvariant().Contains(query)
            || m.Country.ToLowerInvariant().Contains(query)
            ).ToListAsync();
            return members;
        }

        public async Task<List<Member>> GetMembers()
        {
            FStoreDBContext db = new FStoreDBContext();
            return await db.Members.ToListAsync();
        }

        public async Task AddMember(Member member)
        {
            FStoreDBContext db = new FStoreDBContext();
            db.Members.Add(member);
            await db.SaveChangesAsync();
        }

        public async Task UpdateMember(Member member)
        {
            FStoreDBContext db = new FStoreDBContext();
            db.Members.Update(member);
            await db.SaveChangesAsync();
        }

        public async Task DeleteMember(int id)
        {
            FStoreDBContext db = new FStoreDBContext();
            Member member = new Member { MemberId = id };
            db.Members.Attach(member);
            db.Members.Remove(member);
            await db.SaveChangesAsync();
        }
    }
}
