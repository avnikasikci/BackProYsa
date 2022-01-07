using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Hosting;
using System.Text.RegularExpressions;

namespace BackProYsa.lib
{
    public static class DokumanHelper
    {
        //private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;
        //private static IWebHostEnvironment _env; 
        private static string _WebRootPath;
        public static bool IsInitialized { get; private set; }

        public static void Initialize(string WebRootPath)//IHostingEnvironment env)
        {
            if (IsInitialized)
                throw new InvalidOperationException("Object already initialized");

            //_env = env;
            _WebRootPath = WebRootPath;
            IsInitialized = true;
        }

        public static string GetServerMapPath(string FilePath)
        {
            if (!string.IsNullOrEmpty(FilePath))
            {
                FilePath = FilePath.Replace("~/", "");
                if (FilePath[0] == '/') { FilePath = FilePath.Substring(1); }
            }


            if (!IsInitialized)
                throw new InvalidOperationException("Object already initialized");
            return Path.Combine(_WebRootPath, FilePath);
            //return Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, FilePath);
        }

        public enum FileTypes
        {
            Resim = 1,
            Dosya = 2,
            Video = 3,
            Url = 4
        };

        public static string ReplaceTurkishCharacter(string Text)
        {
            var unaccentedText = String.Join("", Text.Normalize(NormalizationForm.FormD)
                    .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));
            return unaccentedText;
        }

        public static FileTypes GetFileType(string FileName)
        {
            if (",.pdf,.docx,.xlsx,.rar,.zip,.txt".IndexOf(Path.GetExtension(FileName.ToLower())) > 0)
            {
                return FileTypes.Dosya;
            }
            else if (",.jpg,.jpeg,.gif,.png,.bmp".IndexOf(Path.GetExtension(FileName.ToLower())) > 0)
            {
                return FileTypes.Resim;

            }
            else if (",.webm,.ogg,.mp4,".IndexOf(Path.GetExtension(FileName.ToLower())) > 0)
            {
                return FileTypes.Video;
            }

            return FileTypes.Dosya; //Hiçbir Doküman kategorisine girmezse Dosya olarak kaydedilir!!!
        }



        public static byte[] GetBytesFromFile(string fullFilePath)
        {
            // this method is limited to 2^32 byte files (4.2 GB)

            FileStream fs = null;
            try
            {
                fs = File.OpenRead(fullFilePath);
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                return bytes;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }

        }

        //File Name içindeki boşlukları siler yerine "_" yapar. Türkçe karakterleri ingilizce karaktere çevirir. Yüklenen dokümanları background olarak kullanınca bu sorun karşımıza çıktı.
        public static string ConvertToFileName(string urlText)
        {
            var FileNameWithoutExtension = Path.GetFileNameWithoutExtension(urlText);
            var FileNameExtension = Path.GetExtension(urlText);
            List<string> urlParts = new List<string>();
            string rt = "";
            Regex r = new Regex(@"[a-z]+", RegexOptions.IgnoreCase);
            //foreach (Match m in r.Matches(Utility.ReplaceTRToEn(FileNameWithoutExtension)))
            //{
            //    urlParts.Add(m.Value);
            //}

            var count = 0;
            for (int i = 0; i < urlParts.Count; i++)
            {
                count++;
                rt = rt + urlParts[i];
                if (count < urlParts.Count) { rt = rt + "_"; }
            }

            rt = rt + FileNameExtension;
            return rt;
        }

        #region Dosya Yükleme İşlemleri Sabitleri
        public static string DefaultImage = "default.jpg";
        public static bool FolderExistOrCreate(string FilePath/*, int TenantId*/)// klasör varmı diye kontrol et, yoksa oluşturulur.
        {
            try
            {
                FilePath = GetServerMapPath(FilePath);// gelen Path server MapPath olarak düzenlenir. Zaten server MapPath ise değişiklik yapılmaz.
                //FilePath = HttpContext.Current.Server.MapPath(FilePath);// gelen Path server MapPath olarak düzenlenir. Zaten server MapPath ise değişiklik yapılmaz.
            }
            catch { }

            try
            {
                if (!Directory.Exists(/*Path.GetDirectoryName(FilePath)*/FilePath)) // klasör yoksa oluşturulur.
                {
                    Directory.CreateDirectory(FilePath);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        public static string CreateFileName()
        {
            return DateTime.Now.Year + DateTime.Now.Ticks.ToString(); // YEni dosya oluşturulurken verilecek dosya ismi. Dosya ismi başına Yıl atandı bu sayede dosya içinde yıl bazlı sıralı görünebilir.
        }
        public static string CreateThumbName(string FileName)
        {
            string FilePath = !string.IsNullOrEmpty(Path.GetDirectoryName(FileName)) ? Path.GetDirectoryName(FileName) + "/" : "";
            return FilePath + "s_" + Path.GetFileName(FileName);
        }
        public static string ExistImageOrDefault(string FilePath, string FileName)
        {
            if (string.IsNullOrEmpty(FileName)) { Path.Combine(FilePath, DefaultImage); } // resim null veya boş gelirse default resim atanır.
            string Fpath = Path.Combine(GetServerMapPath(FilePath), FileName ?? ""); //ekrana basılacak resmin tam yolu alınıyor. örg: C://Picture/xxx
            return Path.Combine(FilePath, File.Exists(Fpath) ? (FileName) : DefaultImage); // serverda belirtilen resim var mı diye kontrol ediliyor. eğer yoksa defult resim basılıyor.
        }

        public static bool ExistImage(string FilePathAndName) // Resim gerçekten varsa true yoksa false.  default resim ise yine false döner. 
        {
            return (!FilePathAndName.Contains(DefaultImage) && File.Exists(FilePathAndName));
        }

        //public static string GetTablePath(Tables tableName)
        //{
        //    return "/Attachment/" + tableName.ToString();
        //}

        public static bool SaveAsFile(IFormFile postedFile, FileTypes FileType, string FilePath, string FileName, int TenantId)
        {
            //foreach (IFormFile postedFile in postedFiles)
            //{
            string physicalPath = "";
            if (FileType == FileTypes.Dosya)
            {
                physicalPath = Path.Combine(FilePath + "/Documents/", FileName);
            }
            else if (FileType == FileTypes.Resim)
            {
                physicalPath = Path.Combine(FilePath + "/Visuals/", FileName);
            }
            else if (FileType == FileTypes.Video)
            {
                physicalPath = Path.Combine(FilePath + "/Video/", FileName);
            }
            try
            {
                // gelen Path server MapPath olarak düzenlenir. Zaten server MapPath ise değişiklik yapılmaz.
                physicalPath = GetServerMapPath("~/UserData/Tenant" + TenantId + "/" + physicalPath);// HttpContext.Current.Server.MapPath("~/UserData/Tenant" + TenantId + "/" + physicalPath);

            }
            catch { }

            FolderExistOrCreate(physicalPath/*, TenantId*/);



            //string fileName = Path.GetFileName(file.FileName);
            using (FileStream stream = new FileStream(physicalPath, FileMode.Create))
            {
                postedFile.CopyTo(stream);
                //uploadedFiles.Add(fileName);
            }
            //}
            //file.SaveAs(physicalPath);
            return true;
        }

        public static bool MoveFile(string SourceFilePath, string SoruceFileName, string DestinationFilePath, string DestinationFileName, int TenantId)
        {
            return MoveCopyFile(true, SourceFilePath, SoruceFileName, DestinationFilePath, DestinationFileName, TenantId);
        }
        public static bool CopyFile(string SourceFilePath, string SoruceFileName, string DestinationFilePath, string DestinationFileName, int TenantId)
        {
            return MoveCopyFile(false, SourceFilePath, SoruceFileName, DestinationFilePath, DestinationFileName, TenantId);
        }

        private static bool MoveCopyFile(bool IsMovefile, string SourceFilePath, string SoruceFileName, string DestinationFilePath, string DestinationFileName, int TenantId)
        {
            string physicalPathSource = Path.Combine(SourceFilePath, SoruceFileName);
            string physicalPathDestination = Path.Combine(DestinationFilePath, DestinationFileName);

            try
            {
                physicalPathSource = GetServerMapPath(physicalPathSource);//HttpContext.Current.Server.MapPath(physicalPathSource); // gelen Path server MapPath olarak düzenlenir. Zaten server MapPath ise değişiklik yapılmaz.
            }
            catch { }

            try
            {
                physicalPathDestination = GetServerMapPath(physicalPathDestination); //HttpContext.Current.Server.MapPath(physicalPathDestination); // gelen Path server MapPath olarak düzenlenir. Zaten server MapPath ise değişiklik yapılmaz.
            }
            catch { }

            FolderExistOrCreate(physicalPathDestination/*, TenantId*/);
            if (File.Exists(physicalPathDestination)) { File.Delete(physicalPathDestination); }// Klasör içinde aynı isimde dosya varsa silmek için

            if (IsMovefile) { File.Move(physicalPathSource, physicalPathDestination); }
            else { File.Copy(physicalPathSource, physicalPathDestination); }

            return true;
        }


        /// <summary>
        /// \~turkish Resim tipinde yüklenen dokümanın sistem içinde gerekli yerlerde küçük hali gösterilemektedir. Bu methodda resim boyutları olması gereken boyutlar hesaplanıyor. 
        /// \~english
        /// </summary>
        /// <param name="org">\~turkish Dokuman olarak yüklenen resmin orjinal hali
        /// \~english</param>
        /// <returns></returns>
        static Size Boyutu(System.Drawing.Image org)
        {
            const int max = 200;// resmimizin max olacak boyutu.
            int uzunluk = org.Width;// orjinal resmin uzunluğu
            int genislik = org.Height;// orjinal resmin genişliği
            double f;// küçültme oranımız.
            if (uzunluk > genislik)// hangi kenar büyük kontrolü
            {
                f = (double)max / uzunluk;//küçültme oranını hesapla
            }
            else
            {
                f = (double)max / genislik;//küçültme oranı hesapla.
            }
            return new Size((int)(uzunluk * f), (int)(genislik * f));
            //oluşturulan yeni boyutu gönder.
        }

        /// <summary>
        /// \~turkish Resim tipinde yüklenen dokümanın sistem içinde gerekli yerlerde küçük hali gösterilemektedir. Bu methodda yeniden boyutlandırılarak küçük resim sisteme yüklenir. 
        /// \~english
        /// </summary>
        public static void SaveThumb(string FileName, string PathSource, string PathDestinition)
        {
            FolderExistOrCreate(PathDestinition/*,0*/);
            //if (!Directory.Exists(Server.MapPath("~/UserData/Tenant" + _UserContext.Tenant.TenantId + "/Attachment/" + TableType.ToString() + "/Visuals/KucukResim/")))
            //{
            //    Directory.CreateDirectory(Server.MapPath("~/UserData/Tenant" + _UserContext.Tenant.TenantId + "/Attachment/" + TableType.ToString() + "/Visuals/KucukResim/"));
            //}
            Image img = Image.FromFile(GetServerMapPath(Path.Combine(PathSource, FileName)));//HttpContext.Current.Server.MapPath(PathSource + FileName));
            //img adında bir nesne tanımlayarak orjinal resmi çağırıyoruz.
            Size byt = Boyutu(img);// yazdığımı Boyutu fonksiyonu yardımıyla yeni boyutumuzu alıyoruz.
            System.Drawing.Image kucukResim = img.GetThumbnailImage(byt.Width, byt.Height, null, IntPtr.Zero);
            // Boyutu küçültme oranımıza göre küçültüp küçük resmi oluşturuyoruz.
            kucukResim.Save(GetServerMapPath(Path.Combine(PathDestinition, CreateThumbName(FileName))));// HttpContext.Current.Server.MapPath(FileName));
            //Küçük Resimler Visuals/Kucukresim Klasörü içine Dosya adının başına small_ eklenerek kaydedilir
            img.Dispose();
            kucukResim.Dispose();
        }

        #endregion
    }

}
