namespace WFC2D
{
    /// <summary>
    /// Класс, представляющий набор тайлов (TileSet), где каждый тайл содержит информацию о совместимости с другими тайлами в различных направлениях.
    /// </summary>
    public class TileSet
    {
        /// <summary>
        /// Массив тайлов, составляющих набор.
        /// </summary>
        public Tile[] Tiles { get; }

        /// <summary>
        /// Количество тайлов в наборе.
        /// </summary>
        public int TileCount => Tiles.Length;

        /// <summary>
        /// Разрешение тайлов (например, размерность или детализация тайлов, используемая в генераторе).
        /// </summary>
        public int Resolution { get; set; }

        /// <summary>
        /// Путь к папке, где хранятся файлы тайлов (если применяется).
        /// </summary>
        public string FolderPaff { get; set; }

        /// <summary>
        /// Массив, хранящий информацию о совместимости тайлов в разных направлениях (вверх, вниз, влево, вправо).
        /// </summary>
        private HashSet<int>[][] CompatibilityMatrix;

        /// <summary>
        /// Конструктор класса, принимающий массив тайлов и автоматически инициализирующий кэш совместимости.
        /// </summary>
        /// <param name="tiles">Массив тайлов, которые будут включены в набор.</param>
        public TileSet(Tile[] tiles)
        {
            Tiles = tiles;
            InitializeCompatibilityMatrix();
        }

        /// <summary>
        /// Конструктор класса, принимающий двумерный массив рельефов тайлов и создающий набор тайлов.
        /// </summary>
        /// <param name="tileTerrains">Двумерный массив рельефов тайлов, где каждый рельеф состоит из 9 элементов.</param>
        public TileSet(int[][] tileTerrains)
        {
            var tileCount = tileTerrains.GetLength(0);
            Tile[] tiles = new Tile[tileCount];
            for (int i = 0; i < tileCount; i++)
            {
                tiles[i] = new Tile(tileTerrains[i]);
            }
            Tiles = tiles;
            InitializeCompatibilityMatrix();
        }

        /// <summary>
        /// Инициализация матрицы совместимости для всех тайлов.
        /// Этот метод создает пустую матрицу, в которой для каждого тайла и его направлений будут храниться соответствующие списки совместимых тайлов.
        /// </summary>
        private void InitializeCompatibilityMatrix()
        {
            int tileCount = Tiles.Length;
            CompatibilityMatrix = new HashSet<int>[tileCount][];

            for (int tileId = 0; tileId < tileCount; tileId++)
            {
                CompatibilityMatrix[tileId] = new HashSet<int>[4]; // Для направлений Up, Down, Left, Right
                for (int directionIndex = 0; directionIndex < 4; directionIndex++)
                {
                    CompatibilityMatrix[tileId][directionIndex] = new HashSet<int>();
                }
            }

            FillCompatibilityMatrix();
        }

        /// <summary>
        /// Заполнение кэша совместимости для всех направлений каждого тайла.
        /// Этот метод анализирует соседство каждого тайла с остальными и заполняет матрицу совместимости.
        /// </summary>
        private void FillCompatibilityMatrix()
        {
            int tileCount = TileCount;

            for (int tileId = 0; tileId < tileCount; tileId++)
            {
                Tile tile = Tiles[tileId];
                for (int directionIndex = 0; directionIndex < 4; directionIndex++)
                {
                    var tileSide = tile.TerrainSides[directionIndex];
                    int oppositeDirectionIndex = DataConverter.GetOppositeDirectionIndex(directionIndex);

                    for (int neighborId = 0; neighborId < tileCount; neighborId++)
                    {
                        var neighborTile = Tiles[neighborId];
                        var neighborSide = neighborTile.TerrainSides[oppositeDirectionIndex];

                        if (Tile.IsPossibleNeighbors(tileSide, neighborSide))
                        {
                            // Добавление ID соседнего тайла в список совместимых для данного направления
                            CompatibilityMatrix[tileId][directionIndex].Add(neighborId);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Получает список совместимых тайлов для заданного тайла и направления.
        /// </summary>
        /// <param name="tileId">ID тайла, для которого требуется получить совместимые тайлы.</param>
        /// <param name="directionIndex">Индекс направления (0 - Up, 1 - Down, 2 - Left, 3 - Right), для которого проверяется совместимость.</param>
        /// <returns>Список ID тайлов, которые совместимы с данным тайлом в указанном направлении.</returns>
        public HashSet<int> GetCompatibleTiles(int tileId, int directionIndex)
        {
            return CompatibilityMatrix[tileId][directionIndex];
        }
    }
}