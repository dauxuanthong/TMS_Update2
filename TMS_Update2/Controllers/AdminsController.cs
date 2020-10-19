using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TMS_Update2.Models;
using TMS_Update2.ViewModels;

namespace TMS_Update2.Controllers
{
    public class AdminsController : Controller
    {
        private ApplicationDbContext _context;

        public AdminsController()
		{
            _context = new ApplicationDbContext();
		}

        public ActionResult Index()
		{
            var userInfor = (from user in _context.Users
                             select new
                             /*FROM-IN: xác định nguồn dữ liệu truy vấn (Users). 
                             Nguồn dữ liệu tập hợp những phần tử thuộc kiểu lớp triển khai giao diện IEnumrable*/
                             /*SELECT: chỉ ra các dữ liệu được xuất ra từ nguồn */
                             {
                                 UserId = user.Id,
                                 Username = user.UserName,
                                 Emailaddress = user.Email,
								 RoleName = (from userRole in user.Roles
									        //JOIN kết hợp 2 trường dữ liệu tương ứng
											join role in _context.Roles //JOIN-IN: chỉ ra nguồn kết nối vs nguồn của FROM   
											on userRole.RoleId          //ON: chỉ ra sự ràng buộc giữa các phần tử  
											equals role.Id              //EQUALS: chỉ ra căn cứ vs ràng buộc (userRole.RoleId ~~ role.Id)
                                            select role.Name).ToList()
							 }
                             ).ToList().Select(p => new UserInRolesViewModel()
                                                {
                                                    UserId = p.UserId,
                                                    Username = p.Username,
                                                    Email = p.Emailaddress,
                                                    Role = string.Join(",", p.RoleName)  //Lúc review nhớ hỏi thầy vì răn dùng cái ni 

                                                }
                                                );

            return View(userInfor);
		}

        /*.
		*//*SHOW ACCOUNT*//*
		[Authorize(Roles = "Admin")]
		[HttpGet]
		public ActionResult Index()
		{
			var Account = _context.Users.ToList();
			return View(Account);
		}
        .*/

		//DELETE ACCOUNT
		[HttpGet]
        public ActionResult Delete(string id)
		{
            var AccountInDB = _context.Users.SingleOrDefault(p => p.Id == id);

            if (AccountInDB == null)
			{
                return HttpNotFound();
			}

            _context.Users.Remove(AccountInDB);
            _context.SaveChanges();       
            return RedirectToAction("Index");
		}

        //Edit 
        [HttpGet]
        public ActionResult Edit(string id)
		{
            var AccountInDB = _context.Users.SingleOrDefault(p => p.Id == id);
            if (AccountInDB == null)
            {
                return HttpNotFound();
            }
            return View(AccountInDB);
        }

        //EDIT
        [Authorize (Roles = "Admin")]
        [HttpPost]
        public ActionResult Edit(ApplicationUser user)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var UsernameIsExist = _context.Users.
                                  Any(p => p.UserName.Contains(user.UserName));

            if (UsernameIsExist)
			{
                ModelState.AddModelError("UserName", "Username already existed");
                return View();
            }

            var AccountInDB = _context.Users.SingleOrDefault(p => p.Id == user.Id);

            if (AccountInDB == null)
            {
                return HttpNotFound();
            }

            AccountInDB.UserName = user.UserName;


            /*.
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            userId = user.Id;
            if (userId != null)
            {
                UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
                userManager.RemovePassword(userId);
                String newPassword = user.PhoneNumber;
                userManager.AddPassword(userId, newPassword);
            }
            .*/
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        //Change password
        [HttpGet]
        public ActionResult ChangePass(string id)
        {
            var AccountInDB = _context.Users.SingleOrDefault(p => p.Id == id);
            if (AccountInDB == null)
            {
                return HttpNotFound();
            }
            return View(AccountInDB);
        }

        //Change password
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult ChangePass (ApplicationUser user)
		{
            if (!ModelState.IsValid)
            {
                return View();
            }


            var AccountInDB = _context.Users.SingleOrDefault(p => p.Id == user.Id);

            if (AccountInDB == null)
            {
                return HttpNotFound();
            }

            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
			userId = user.Id;
            if (userId != null)
            {
                UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
                userManager.RemovePassword(userId);
                String newPassword = user.PhoneNumber;
                userManager.AddPassword(userId, newPassword);
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}