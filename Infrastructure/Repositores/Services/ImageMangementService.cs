using Ecom.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repositores.Services
{
    public class ImageMangementService : IImageManagmentService
    {
        private readonly IFileProvider file;

        public ImageMangementService(IFileProvider file)
        {
            this.file = file;
        }
        public async Task<List<string>> AddImageAsync(IFormFileCollection files, string src)
        {
            var saveImageSrc = new List<string>();
            var ImageDirectory = Path.Combine("wwwroot", "Images", src);
            if (!Directory.Exists(ImageDirectory))
            {
                Directory.CreateDirectory(ImageDirectory);
            }
            foreach (var item in files)
            {
                if(item.Length > 0)
                {
                    var ImageName = item.FileName;
                    var ImageSrc = $"/Images/{src}/{ImageName}";
                    var Root = Path.Combine(ImageDirectory, ImageName);
                    using(FileStream steam = new FileStream(Root, FileMode.Create))
                    {
                        await item.CopyToAsync(steam);
                    }
                    saveImageSrc.Add(ImageSrc);
                }
            }
            return saveImageSrc;
        }

        public void DeleteImageAsync(string src)
        {
            var info = file.GetFileInfo(src);
            var root = info.PhysicalPath;
            if (File.Exists(root))
            {
                File.Delete(root);
            }
        }
    }
}
