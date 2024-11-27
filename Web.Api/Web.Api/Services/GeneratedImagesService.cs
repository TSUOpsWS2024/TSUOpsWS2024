using System.IO;
using WFC2D;

namespace Web.Api.Services
{
    public class GeneratedImagesService
    {
        public GeneratedImagesService() { 
        }   
        public static FileStream Work()
        {
            WFCGenerator.GenerateFromJson("..\\..\\Domain\\WFC2D\\Tilesets\\Test\\data.json", 100, 100, 1, 42);
            var files = Directory.GetFiles(".\\Result\\");
            FileStream fs = File.OpenRead(files[files.Length - 1]);
            return fs;
        }
    }
}
