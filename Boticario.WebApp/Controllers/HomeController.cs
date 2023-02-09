using System.Net;
using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;

using Boticario.WebApp.Data;
using Boticario.WebApp.Models;
using Boticario.WebApp.Code.Services;
using System;

namespace Boticario.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private List<Repos> _listRepos;
        private readonly Repositories _repositoriesServices;

        public HomeController(ILogger<HomeController> logger, AppDbContext context, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _context = context;
            _listRepos = new();
            _repositoriesServices = new Repositories(context, clientFactory);
        }

        public async Task<IActionResult> Index(int? id)
        {
            try
            {
                if (id != null)
                {
                    var httpStatusCode = await _repositoriesServices.GetAllReposFromGitHub();
                    if (httpStatusCode != HttpStatusCode.OK)
                    {
                        ViewData["StatusCode"] = httpStatusCode.ToString();
                    }
                }

                _listRepos = _context.Repositories.ToList();

                ViewData["TotalCount"] = _listRepos.Count;

                return View(_listRepos);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public IActionResult Delete()
        {
            var _result = _repositoriesServices.DeleteReposAllRepos();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {

            RepositoryJson model = await _repositoriesServices.GetReposByUserRepos(id);

            return View(model);
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