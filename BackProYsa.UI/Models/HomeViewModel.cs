using Microsoft.AspNetCore.Http;
using System;

namespace BackProYsa.UI.Models
{
    public class HomeViewModel
    {
        public string Image { get; set; }
        public IFormFile ImageUpload { get; set; }

    }
}
