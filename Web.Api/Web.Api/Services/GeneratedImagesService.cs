using WFC2D;

namespace Web.Api.Services
{
    public class GeneratedImagesService
    {
        public GeneratedImagesService() { 
        }   
        public static void Work()
        {
            WFCGenerator.GenerateFromJson("..\\..\\Domain\\WFC2D\\Tilesets\\Test\\data.json", 100, 200, 0, 42);
        }
    }
}
