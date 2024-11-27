using SkiaSharp;

namespace WFC2D
{
    /// <summary>
    /// Класс для генерации областей с использованием алгоритма WFC (Wave Function Collapse).
    /// </summary>
    public static class WFCGenerator
    {
        /// <summary>
        /// Сид для генератора случайных чисел.
        /// </summary>
        public static int Seed { get; private set; }

        /// <summary>
        /// Идентификатор внешнего тайла.
        /// </summary>
        private static int OuterTileId { get; set; } = -1;

        /// <summary>
        /// Набор тайлов для генерации.
        /// </summary>
        private static TileSet TileSet { get; set; }

        /// <summary>
        /// Экземпляр генератора случайных чисел.
        /// </summary>
        private static Random Random { get; set; }

        /// <summary>
        /// Матрица клеток области.
        /// </summary>
        private static AreaCell[,] AreaMap { get; set; }

        /// <summary>
        /// Словарь потенциальных клеток для коллапсирования.
        /// </summary>
        private static Dictionary<(int, int), AreaCell> PotentialCells { get; set; } = new();

        /// <summary>
        /// Высота области.
        /// </summary>
        private static int AreaHeight { get; set; }

        /// <summary>
        /// Ширина области.
        /// </summary>
        private static int AreaWidth { get; set; }

        /// <summary>
        /// Основной метод генерации области.
        /// </summary>
        /// <param name="tileSet">Набор тайлов для генерации.</param>
        /// <param name="areaH">Высота области.</param>
        /// <param name="areaW">Ширина области.</param>
        /// <param name="outerTileId">Идентификатор внешнего тайла.</param>
        /// <param name="seed">Сид для генератора случайных чисел.</param>
        public static void Generate(TileSet tileSet, int areaH, int areaW, int outerTileId, int seed)
        {
            ConfigureGenerator(tileSet, areaH, areaW, outerTileId, seed);
            ExecuteWFC();
            SaveGenerationResultImage();
        }

        /// <summary>
        /// Метод генерации с сохранением истории шагов генерации.
        /// </summary>
        /// <param name="tileSet">Набор тайлов для генерации.</param>
        /// <param name="areaH">Высота области.</param>
        /// <param name="areaW">Ширина области.</param>
        /// <param name="outerTileId">Идентификатор внешнего тайла.</param>
        /// <param name="seed">Сид для генератора случайных чисел.</param>
        public static void GenerateWithSavingGenerationHistory(TileSet tileSet, int areaH, int areaW, int outerTileId, int seed)
        {
            ConfigureGenerator(tileSet, areaH, areaW, outerTileId, seed);
            ExecuteWFCWithHistory();
            SaveGenerationResultImage();
        }

        /// <summary>
        /// Метод генерации области из JSON файла.
        /// </summary>
        /// <param name="tileSetName">Имя набора тайлов.</param>
        /// <param name="areaH">Высота области.</param>
        /// <param name="areaW">Ширина области.</param>
        /// <param name="outerTileId">Идентификатор внешнего тайла.</param>
        /// <param name="seed">Сид для генератора случайных чисел.</param>
        public static void GenerateFromJson(string tileSetName, int areaH, int areaW, int outerTileId, int seed)
        {
            var path = Path.GetFullPath("../../../");
            var tileSet = DataConverter.LoadFromFile(Path.Join(path, "TileSets", tileSetName, "data.json"));
            Generate(tileSet, areaH, areaW, outerTileId, seed);
        }

        /// <summary>
        /// Конфигурирует генератор с заданными параметрами.
        /// </summary>
        /// <param name="tileSet">Набор тайлов.</param>
        /// <param name="areaH">Высота области.</param>
        /// <param name="areaW">Ширина области.</param>
        /// <param name="outerTileId">Идентификатор внешнего тайла.</param>
        /// <param name="seed">Сид для генератора случайных чисел.</param>
        private static void ConfigureGenerator(TileSet tileSet, int areaH, int areaW, int outerTileId, int seed)
        {
            Seed = seed;
            OuterTileId = outerTileId;
            TileSet = tileSet;
            AreaHeight = areaH;
            AreaWidth = areaW;
            Random = new Random(seed);
            
            ConstructArea(areaH, areaW, tileSet.TileCount);
            PotentialCells.TryAdd((0, 0), AreaMap[0, 0]);
        }

        /// <summary>
        /// Строит матрицу клеток для области.
        /// </summary>
        /// <param name="areaH">Высота области.</param>
        /// <param name="areaW">Ширина области.</param>
        /// <param name="tileCount">Количество тайлов в наборе.</param>
        private static void ConstructArea(int areaH, int areaW, int tileCount)
        {
            AreaMap = new AreaCell[areaH, areaW];
            for (int i = 0; i < areaH; i++)
            {
                for (int j = 0; j < areaW; j++)
                {
                    AreaMap[i, j] = new AreaCell(tileCount);
                }
            }
            ConfigureBorderCells();
        }

        /// <summary>
        /// Конфигурирует клетки на границах области.
        /// </summary>
        private static void ConfigureBorderCells()
        {
            HashSet<int> boundValidValues;

            // Обход верхней границы
            boundValidValues = TileSet.GetCompatibleTiles(OuterTileId, (int)EDirection.Down);
            for (int col = 0; col < AreaWidth; col++)
            {
                AreaMap[0, col].CellVariats = AreaMap[0, col].CellVariats.Intersect(boundValidValues).ToList();
            }

            // Обход нижней границы
            boundValidValues = TileSet.GetCompatibleTiles(OuterTileId, (int)EDirection.Up);
            for (int col = 0; col < AreaWidth; col++)
            {
                AreaMap[AreaHeight - 1, col].CellVariats = AreaMap[AreaHeight - 1, col].CellVariats.Intersect(boundValidValues).ToList();
            }

            // Обход левой границы 
            boundValidValues = TileSet.GetCompatibleTiles(OuterTileId, (int)EDirection.Right);
            for (int row = 0; row < AreaHeight; row++)
            {
                AreaMap[row, 0].CellVariats = AreaMap[row, 0].CellVariats.Intersect(boundValidValues).ToList();
            }

            // Обход правой границы
            boundValidValues = TileSet.GetCompatibleTiles(OuterTileId, (int)EDirection.Left);
            for (int row = 0; row < AreaHeight; row++)
            {
                AreaMap[row, AreaWidth - 1].CellVariats = AreaMap[row, AreaWidth - 1].CellVariats.Intersect(boundValidValues).ToList();
            }
        }

        /// <summary>
        /// Выполняет шаги алгоритма WFC.
        /// </summary>
        private static void ExecuteWFC()
        {
            do
            {
                ItitWFCStep();

            } while (PotentialCells.Count > 0);

            Console.WriteLine($"Генерация обсласти {AreaWidth} на {AreaHeight} с сидом {Seed} успешно завершена.");
        }

        /// <summary>
        /// Выполняет шаги алгоритма WFC с сохранением истории шагов.
        /// </summary>
        private static void ExecuteWFCWithHistory()
        {
            var path = Path.GetFullPath("../../../");
            var stepsFolder = Path.Join(path, "GenerationSteps");

            // Очищаем папку перед сохранением
            if (Directory.Exists(stepsFolder))
            {
                Directory.GetFiles(stepsFolder).ToList().ForEach(File.Delete);
            }
            else
            {
                // Создаем папку, если она не существует
                Directory.CreateDirectory(stepsFolder);
            }

            // Создаем объект для рендеринга карты
            using (var areaRenderer = new AreaRenderer(TileSet))
            {
                int stepCounter = 0;

                do
                {
                    // Сохраняем изображение после каждого шага
                    SaveCurrentAreaImage(stepsFolder, stepCounter, areaRenderer);

                    // Выполняем один шаг алгоритма WFC
                    ItitWFCStep();

                    stepCounter++;
                } while (PotentialCells.Count > 0);

                Console.WriteLine($"Генерация области {AreaWidth} на {AreaHeight} с сидом {Seed} успешно завершена.");
            }
        }

        /// <summary>
        /// Сохраняет изображение текущего состояния области.
        /// </summary>
        /// <param name="folderPath">Путь к папке для сохранения изображения.</param>
        /// <param name="stepCounter">Номер шага.</param>
        /// <param name="areaRenderer">Рендерер области.</param>
        private static void SaveCurrentAreaImage(string folderPath, int stepCounter, AreaRenderer areaRenderer)
        {
            string filePath = Path.Combine(folderPath, $"WFC St{stepCounter} s{Seed}.png");

            // Рендерим текущее состояние карты в изображение
            var resultBitmap = areaRenderer.RenderAreaToBitmap(AreaMap);

            // Сохраняем изображение в формате PNG
            using (var fileStream = File.OpenWrite(filePath))
            {
                resultBitmap.Encode(fileStream, SKEncodedImageFormat.Png, 100);
            }
        }

        /// <summary>
        /// Сохраняет финальный результат генерации области в файл.
        /// </summary>
        private static void SaveGenerationResultImage()
        {
            var path = Path.GetFullPath("../../../");
            var resultFolder = Path.Join(path, "Result");
            var resultFileName = $"WFC R h{AreaHeight} w{AreaWidth} s{Seed}.png";
            var resultPath = Path.Combine(resultFolder, resultFileName);
            // Очищаем папку перед сохранением
            if (Directory.Exists(resultFolder))
            {
                Directory.GetFiles(resultFolder).ToList().ForEach(File.Delete);
            }
            else
            {
                // Создаем папку, если она не существует
                Directory.CreateDirectory(resultFolder);
            }
            using (var areaRenderer = new AreaRenderer(TileSet))
            using (var fileStream = File.OpenWrite(resultPath))
            {
                var resultBitmap = areaRenderer.RenderAreaToBitmap(AreaMap);
                // Кодируем bitmap в формат PNG и записываем в поток
                resultBitmap.Encode(fileStream, SKEncodedImageFormat.Png, 100);
            }
            Console.WriteLine($"Изображение сохранено как \"{resultPath}\"");
        }
        private static void ItitWFCStep()
        {
            // Получаем координаты клетки с минимальной энтропией
            var (row, col) = GetLowestEntropyCellCoordinates();

            // Коллапсируем выбранную клетку
            CollapseCell(row, col);
        }
        private static (int, int) GetLowestEntropyCellCoordinates()
        {
            if (PotentialCells.Count == 0) throw new InvalidOperationException("Все клетки уже были сколлапсированы");

            int lowestEntropy = int.MaxValue;
            List<(int, int)> lowestEntropyCoordinates = new();

            // Находим координаты клеток с минимальной энтропией
            foreach (var kvp in PotentialCells)
            {
                var (coordinates, cell) = (kvp.Key, kvp.Value);

                if (cell.Entropy < lowestEntropy)
                {
                    lowestEntropy = cell.Entropy;
                    lowestEntropyCoordinates.Clear();
                    lowestEntropyCoordinates.Add(coordinates);
                }
                else if (cell.Entropy == lowestEntropy)
                {
                    lowestEntropyCoordinates.Add(coordinates);
                }
            }

            // Возвращаем случайные координаты из тех, что обладают минимальной энтропией
            int randomIndex = Random.Next(lowestEntropyCoordinates.Count);
            return lowestEntropyCoordinates[randomIndex];
        }
        private static void CollapseCell(int row, int col)
        {
            // Получаем ячейку из матрицы по указанным координатам
            var cell = AreaMap[row, col];

            // Генерируем случайный индекс в пределах количества допустимых вариантов тайлов
            int randomIndex = Random.Next(cell.Entropy);

            // Выбираем вариант тайла в который сколлапсировала клетка
            var collapsedId = cell.CellVariats[randomIndex];

            // Сохраняем вариант тайла, в который сколлапсировала клетка
            cell.CollapsedId = collapsedId;

            // Удаляем клетку из списка потенциальных для коллапсирования
            PotentialCells.Remove((row, col));

            // Уменьшаем энтропию соседей
            DecreaseNeighborsEntropy(collapsedId, row - 1, col, EDirection.Up);       // Верхний сосед
            DecreaseNeighborsEntropy(collapsedId, row + 1, col, EDirection.Down);     // Нижний сосед
            DecreaseNeighborsEntropy(collapsedId, row, col - 1, EDirection.Left);     // Левый сосед
            DecreaseNeighborsEntropy(collapsedId, row, col + 1, EDirection.Right);    // Правый сосед
        }
        private static void DecreaseNeighborsEntropy(int collapsedId, int neighborRow, int neighborCol, EDirection collapsedSide)
        {
            // Проверяем, находится ли сосед внутри границ матрицы
            if (neighborRow >= 0 && neighborRow < AreaHeight && neighborCol >= 0 && neighborCol < AreaWidth)
            {
                // Сохраняем ссылку на клетку соседа
                var neighborCell = AreaMap[neighborRow, neighborCol];

                // Если клетка соседа ещё находится в суперпозиции (не сколлапсировала)
                if (neighborCell.CollapsedId == -1)
                {
                    // Добавляем её в список потенциальных клеток, если её там ещё нет
                    PotentialCells.TryAdd((neighborRow, neighborCol), neighborCell);

                    var possibleNeighbors = TileSet.GetCompatibleTiles(collapsedId, (int)collapsedSide);

                    neighborCell.CellVariats = possibleNeighbors.Intersect(neighborCell.CellVariats).ToList();
                }
            }
        }
    }
}
