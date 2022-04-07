using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace WebAppBV.ViewModels
{
    public class ImportFileViewModel
    {
        [Required(ErrorMessage = "Selecione o arquivo.")]
        public IFormFile FormFile { get; set; }
    }
}
