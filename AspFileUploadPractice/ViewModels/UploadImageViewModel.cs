using System.ComponentModel.DataAnnotations;

namespace AspFileUploadPractice.ViewModels
{
    public class UploadImageViewModel
    {
        [Required]
        [Display(Name = "Image")]
        public IFormFile SpeakerPicture { get; set; }
    }
}
