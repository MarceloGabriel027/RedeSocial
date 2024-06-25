using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace projetoRedeSocial.Models
{
    public class Post
    {
        [Column("postId")]
        [Display(Name = "Post ID")]
        public int postId { get; set; }

        [Column("usuarioId")]
        [Display(Name = "Usuário ID")]
        public int usuarioId { get; set; }

        [Column("usuarioPost")]
        [Display(Name = "Usuário")]
        public Usuario? usuarioPost { get; set; }

        [Column("postTxt")]
        [Display(Name = "Conteudo")]
        public string? postTxt { get; set; }

        [Column("postArquivo")]
        [Display(Name = "Arquivo")]
        public string? postArquivo { get; set; }

        [Column("postDate")]
        [Display(Name = "Data da postagem")]
        public string? postDate { get; set; }

        [Column("postStatus")]
        [Display(Name = "Status")]
        public string? postStatus { get; set; }

    }
}
