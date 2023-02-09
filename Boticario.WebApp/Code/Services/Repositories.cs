using System;
using System.Net;
using System.Text.Json;

using Boticario.WebApp.Data;
using Boticario.WebApp.Models;

namespace Boticario.WebApp.Code.Services
{
    public class Repositories
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly AppDbContext _context;
        private List<Repos> _listRepos;

        public Repositories(AppDbContext context, IHttpClientFactory clientFactory)
        {
            _context = context;
            _clientFactory = clientFactory;
            _listRepos = new();
        }

        /// <summary>
        /// Get All Repositorios filtrados por idiomas (csharp + python + ruby + fsharp + visual-basic)
        /// baseAdress: https://api.github.com/
        /// </summary>
        public async Task<HttpStatusCode> GetAllReposFromGitHub()
        {
            try
            {
                var _uri = new Uri("https://api.github.com/search/repositories?q=language:csharp+language:python+language:ruby+language:fsharp+language:visual-basic+is:featured&sort=stars&order=desc&&per_page=100");
                var request = new HttpRequestMessage(HttpMethod.Get, _uri);
                request.Headers.Add("Accept", "application/vnd.github.v3+json");
                request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

                var client = _clientFactory.CreateClient();

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    using var responseStream = await response.Content.ReadAsStreamAsync();
                    var _repositoriesJson = await JsonSerializer.DeserializeAsync<RepositoriesJson>(responseStream);

                    foreach (var item in _repositoriesJson.items)
                    {
                        if (!ReposExists(item.name, item.owner.login))
                        {
                            var _repos = new Repos { NameRepos = item.name, NameUser = item.owner.login, Topics = String.Join(", ", item.topics) };

                            _listRepos.Add(_repos);

                            _context.Add(_repos);
                        }
                    }

                    _context.SaveChanges();

                }

                return response.StatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Get Repositórios por Usuário e Nome do Repositorio
        /// /repos/{users}/{name}
        /// </summary>
        /// <param name="id"></param>
        public async Task<RepositoryJson> GetReposByUserRepos(int id)
        {
            RepositoryJson? _repositoryJson = null;

            try
            {
                Repos? _repos = _context.Repositories.Find(id);

                if (_repos == null) return null;

                var _uri = new Uri($"https://api.github.com/repos/{_repos.NameUser}/{_repos.NameRepos}");

                var request = new HttpRequestMessage(HttpMethod.Get, _uri);
                request.Headers.Add("Accept", "application/vnd.github.v3+json");
                request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

                var client = _clientFactory.CreateClient();

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    using var responseStream = await response.Content.ReadAsStreamAsync();

                    _repositoryJson = await JsonSerializer.DeserializeAsync<RepositoryJson>(responseStream);

                }

                return _repositoryJson;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Excluir todos os repositórios
        /// </summary>
        public bool DeleteReposAllRepos()
        {
            try
            {
                var items = _context.Repositories.ToList();
                foreach (var item in items)
                {
                    _context.Repositories.Remove(item);
                }

                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw new Exception(ex.Message, ex);
            }
        }

        private bool ReposExists(string name, string login)
        {
            return _context.Repositories.Any(e => e.NameRepos == name && e.NameUser == login);
        }

    }
}
