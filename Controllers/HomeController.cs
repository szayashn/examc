using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace OneProject.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                return RedirectToAction("Auctions");
            }
            return View();
        }
        [HttpPost("create")]
        public IActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                if (dbContext.Users.Any(u => u.EmailReg == user.EmailReg))
                {
                    ModelState.AddModelError("EmailReg", "Email already in use!");

                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    user.PasswordReg = Hasher.HashPassword(user, user.PasswordReg);
                    dbContext.Add(user);
                    dbContext.SaveChanges();
                    HttpContext.Session.SetString("Email", user.EmailReg);
                    return RedirectToAction("Auctions");
                }
            }
            else
            {

                return View("Index");
            }
            return RedirectToAction("Index");
        }
        [HttpPost("login")]
        public IActionResult Login(LoginUser user)
        {
            if (ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.EmailReg == user.Email);

                if (userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index");
                }
                var hasher = new PasswordHasher<LoginUser>();

                var result = hasher.VerifyHashedPassword(user, userInDb.PasswordReg, user.Password);
                if (result == 0)
                {
                    ModelState.AddModelError("Email", "Wrong email or password");

                    return View("Index");
                }
                HttpContext.Session.SetString("Email", user.Email);
                return RedirectToAction("Auctions");
            }
            else
            {

                return View("Index");
            }
        }
        [HttpGet("success")]
        public IActionResult Success()
        {
            if (HttpContext.Session.GetString("Email") != null)
            {

                return View();
            }
            return RedirectToAction("Index");
        }
        [HttpGet("Auctions")]
        public IActionResult Auctions()
        {
            if (HttpContext.Session.GetString("Email") != null)
            {

                var all = dbContext.Auctions.Include(s => s.Users).ThenInclude(s => s.User).OrderBy(s => s.EndDate).Where(s => s.EndDate >= DateTime.Today).ToList();
                List<int> d = new List<int>();
                foreach (var x in all)
                {
                    int y = (x.EndDate - DateTime.Today).Days;
                    d.Add(y);
                }
                ViewBag.Days = d;
                ViewBag.AllAuctions = all;
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.CurrentBalance = dbContext.Users.Single(s => s.EmailReg == HttpContext.Session.GetString("Email")).Wallet;
                return View("Auctions");
            }
            return RedirectToAction("Index");
        }
        [HttpGet("NewAuction")]
        public IActionResult NewAuction()
        {
            if (HttpContext.Session.GetString("Email") != null)
            {

                return View();
            }
            return RedirectToAction("Index");
        }
        [HttpPost("AddAuction")]
        public IActionResult AddAuction(Auction auction)
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                if (auction.EndDate <= DateTime.Today)
                {
                    ModelState.AddModelError("EndDate", "End date must be in the future");
                    return View("NewAuction");
                }
                if (ModelState.IsValid)
                {

                    dbContext.Add(auction);
                    dbContext.SaveChanges();
                    Association a = new Association()
                    {
                        AuctionId = dbContext.Auctions.Last().AuctionId,
                        UserId = dbContext.Users.Single(s => s.EmailReg == HttpContext.Session.GetString("Email")).UserId
                    };
                    dbContext.Add(a);
                    dbContext.SaveChanges();
                }
                else
                {
                    return View("NewAuction");
                }
            }
            return RedirectToAction("Index");
        }
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        [HttpGet("Display/{id}")]
        public IActionResult Display(int id)
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                Auction a = dbContext.Auctions.FirstOrDefault(s => s.AuctionId == id);
                if (a == null)
                {
                    return RedirectToAction("Index");
                }
                int uid = dbContext.Associations.FirstOrDefault(s => s.AuctionId == id).UserId;
                ViewBag.Name = dbContext.Users.Single(s => s.UserId == uid).FirstName;
                ViewBag.Days = (a.EndDate - DateTime.Today).Days;

                if (a.BidderId == null)
                {
                    ViewBag.Bidder = dbContext.Users.FirstOrDefault(s => s.UserId == uid).FirstName;
                }
                else
                {
                    ViewBag.Bidder = dbContext.Users.FirstOrDefault(s => s.UserId == Int32.Parse(a.BidderId)).FirstName;
                }
                return View("Display", a);
            }
            return RedirectToAction("Index");
        }
        [HttpGet("Delete/{uid}/{aid}")]
        public IActionResult Delete(int uid, int aid)
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                var a = dbContext.Associations.Single(s => s.AuctionId == aid && s.UserId == uid);
                dbContext.Associations.Remove(a);
                var auc = dbContext.Auctions.Single(s => s.AuctionId == aid);
                dbContext.Auctions.Remove(auc);
                dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        [HttpPost("AddBid")]
        public IActionResult AddBid(Bid bid)
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                Auction a = dbContext.Auctions.FirstOrDefault(s => s.AuctionId == bid.Id);
                if (a == null)
                {
                    return RedirectToAction("index");
                }

                User currUser = dbContext.Users.FirstOrDefault(s => s.EmailReg == HttpContext.Session.GetString("Email"));

                if (currUser.Wallet >= bid.CurrentBid && bid.CurrentBid > a.Bid)
                {
                    if (a.BidderId == null)
                    {
                        currUser.Wallet -= bid.CurrentBid;
                        a.BidderId = currUser.UserId.ToString();
                        a.Bid = bid.CurrentBid;
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        User prevUser = dbContext.Users.FirstOrDefault(s => s.UserId == Int32.Parse(a.BidderId));
                        prevUser.Wallet += a.Bid;
                        dbContext.SaveChanges();
                        currUser.Wallet -= bid.CurrentBid;
                        a.BidderId = currUser.UserId.ToString();
                        a.Bid = bid.CurrentBid;
                        dbContext.SaveChanges();
                    }

                }

                return Redirect($"/Display/{bid.Id}");
            }
            return RedirectToAction("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
