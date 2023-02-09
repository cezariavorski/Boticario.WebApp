using System.ComponentModel;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Boticario.WebApp.Models
{
    [Table("repositories")]
    [Microsoft.EntityFrameworkCore.Index(nameof(NameRepos), nameof(NameUser), IsUnique = true)]
    public class Repos
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("ID")]
        public int ReposID { get; set; }

        [DisplayName("Nome Repos")]
        [JsonPropertyName("name")]
        public string NameRepos { get; set; }

        [DisplayName("Usuário")]
        [JsonPropertyName("login")]
        public string NameUser { get; set; }

        [DisplayName("Tópicos")]
        [JsonPropertyName("topics")]
        public string Topics { get; set; }

    }
}
