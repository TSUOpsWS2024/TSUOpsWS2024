using static System.Net.Mime.MediaTypeNames;

namespace WFC2D
{
    public class Program
    {
        static void Main(string[] args)
        {

            /*int[][] tileTerrain =
                [
                   // 0 тайл воды
                   [0, 0, 0,
                    0, 0, 0,
                    0, 0, 0],
                   // 1 тайл травы
                   [1, 1, 1,
                    1, 1, 1,
                    1, 1, 1],
                   // 2 внешний угловой побережье
                   [0, 0, 0,
                    0, 2, 2,
                    0, 2, 1],
                   // 3 повёрнутый на 90° внешний угловой побережье
                   [0, 0, 0,
                    2, 2, 0,
                    1, 2, 0],
                   // 4 повёрнутый на 180° внешний угловой побережье
                   [1, 2, 0,
                    2, 2, 0,
                    0, 0, 0],
                   // 5 повёрнутый на 270° внешний угловой побережье
                   [0, 2, 1,
                    0, 2, 2,
                    0, 0, 0],
                   // 6 побережье
                   [0, 0, 0,
                    2, 2, 2,
                    1, 1, 1],
                   // 7 повёрнутый на 90° побережье
                   [0, 2, 1,
                    0, 2, 1,
                    0, 2, 1],
                   // 8 повёрнутый на 180° побережье
                   [1, 2, 0,
                    1, 2, 0,
                    1, 2, 0],
                   // 9 повёрнутый на 270° побережье
                   [1, 1, 1,
                    2, 2, 2,
                    0, 0, 0],
                   // 10 внутренний угловой побережье
                   [1, 1, 1,
                    1, 2, 2,
                    1, 2, 0],
                   // 11 повёрнутый на 90° внутренний угловой побережье
                   [1, 1, 1,
                    2, 2, 1,
                    0, 2, 1],
                   // 12 повёрнутый на 180° внутренний угловой побережье
                   [0, 2, 1,
                    2, 2, 1,
                    1, 1, 1],
                   // 13 повёрнутый на 270° внутренний угловой побережье
                   [1, 2, 0,
                    1, 2, 2,
                    1, 1, 1],
                ];

            TileSet tileSet = new TileSet(tileTerrain);
            tileSet.Resolution = 16;
            tileSet.FolderPaff = Path.Combine("TileSets", "Test");

            WFCGenerator.Generate(tileSet, 100, 200, 0, 42);

            DataConverter.SaveToFile(tileSet, Path.Combine(tileSet.FolderPaff, "data.json"));

            var tileSet2 = DataConverter.LoadFromFile(Path.Combine(tileSet.FolderPaff, "data.json"));*/
            WFCGenerator.GenerateFromJson("..\\..\\..\\TileSets\\Test\\data.json", 100, 200, 0, 42);
        }
    }
}
