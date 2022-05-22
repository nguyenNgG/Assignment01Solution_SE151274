using BusinessObjects;
using DataAccess.Repositories.Interfaces;
using eStoreAPI.Constants;
using eStoreAPI.Models;
using eStoreAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;

namespace eStoreAPI.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        IMemberRepository repository;
        IConfiguration configuration;

        public MemberController(IMemberRepository _repository, IConfiguration _configuration)
        {
            repository = _repository;
            configuration = _configuration;
        }

        [HttpGet("authorize")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MemberAuthentication))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Authorize()
        {
            MemberAuthentication auth = SessionHelper.GetFromSession<MemberAuthentication>(HttpContext.Session, SessionValue.Authentication);
            if (auth != null && auth.IsAdmin)
            {
                return Ok(auth);
            }
            return BadRequest();
        }

        [HttpGet("authenticate")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MemberAuthentication))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Authenticate()
        {
            MemberAuthentication auth = SessionHelper.GetFromSession<MemberAuthentication>(HttpContext.Session, SessionValue.Authentication);
            if (auth != null)
            {
                return Ok(auth);
            }
            return BadRequest();
        }

        [HttpGet("current")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Current()
        {
            MemberAuthentication auth = SessionHelper.GetFromSession<MemberAuthentication>(HttpContext.Session, SessionValue.Authentication);
            if (auth != null)
            {
                return Ok(auth.MemberId);
            }
            return BadRequest();
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MemberAuthentication))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Login(LoginForm loginForm)
        {
            if (loginForm == null)
            {
                return BadRequest();
            }

            if (SessionHelper.GetFromSession<MemberAuthentication>(HttpContext.Session, SessionValue.Authentication) != null)
            {
                return BadRequest();
            }

            string adminEmail = configuration.GetValue<string>("Admin:Email");
            string adminPassword = configuration.GetValue<string>("Admin:Password");

            bool isAdmin = (loginForm.Email == adminEmail && loginForm.Password == adminPassword);
            Member member;
            if (isAdmin)
            {
                member = new Member
                {
                    Email = adminEmail,
                    MemberId = -993901,
                };
            }
            else
            {
                member = await repository.Login(loginForm.Email, loginForm.Password);
            }

            if (member != null)
            {
                MemberAuthentication auth = new MemberAuthentication
                {
                    Email = member.Email,
                    MemberId = member.MemberId,
                    IsAdmin = isAdmin,
                };

                SessionHelper.SaveToSession<MemberAuthentication>(HttpContext.Session, auth, SessionValue.Authentication);
                Cart cart = new Cart();
                SessionHelper.SaveToSession<Cart>(HttpContext.Session, cart, SessionValue.Cart);

                return Ok(auth);
            }
            return BadRequest();
        }

        // GET: by id
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Member))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Member>> GetMember(int id)
        {
            Member member = await repository.GetMember(id);
            if (member == null)
            {
                return NotFound();
            }
            return Ok(member);
        }

        // GET: list
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Member>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Member>>> GetMembers(string query)
        {
            List<Member> members = null;
            if (query != null && query.Length > 0)
            {
                members = await repository.GetMembers(query);
            }
            else
            {
                members = await repository.GetMembers();
            }

            if (members == null)
            {
                return NotFound();
            }
            return Ok(members);
        }

        // ADD
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Member))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<Member>> AddMember(Member member)
        {
            try
            {
                await repository.AddMember(member);
                return CreatedAtAction("GetMember", new { id = member.MemberId }, member);
            }
            catch
            {
                if (await repository.GetMember(member.MemberId) != null)
                {
                    return Conflict();
                }
                return BadRequest();
            }
        }

        // UPDATE
        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Member))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Member>> UpdateMember(int id, Member member)
        {
            if (id != member.MemberId)
            {
                return BadRequest();
            }

            try
            {
                MemberAuthentication auth = SessionHelper.GetFromSession<MemberAuthentication>(HttpContext.Session, SessionValue.Authentication);
                if (auth != null)
                {
                    if (auth.MemberId == id || auth.IsAdmin)
                    {
                        await repository.UpdateMember(member);
                        return Ok(member);
                    }
                }
                return BadRequest();
            }
            catch (DbUpdateException)
            {
                if (await repository.GetMember(member.MemberId) == null)
                {
                    return NotFound();
                }
                return BadRequest();
            }
        }

        // DELETE
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Member>> DeleteMember(int id)
        {
            try
            {
                await repository.DeleteMember(id);
                return NoContent();
            }
            catch (DbUpdateException)
            {
                if (await repository.GetMember(id) == null)
                {
                    return NotFound();
                }
                return BadRequest();
            }
        }

    }
}
