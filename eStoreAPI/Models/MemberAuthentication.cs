namespace eStoreAPI.Models
{
    public class MemberAuthentication
    {
        public int MemberId { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
    }
}
