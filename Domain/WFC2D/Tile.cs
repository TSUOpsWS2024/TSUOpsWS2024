namespace WFC2D
{
    /// <summary>
    /// Перечисление направлений для тайлов.
    /// </summary>
    public enum EDirection
    {
        /// <summary>
        /// Направление вверх.
        /// </summary>
        Up = 0,

        /// <summary>
        /// Направление вниз.
        /// </summary>
        Down = 1,

        /// <summary>
        /// Направление влево.
        /// </summary>
        Left = 2,

        /// <summary>
        /// Направление вправо.
        /// </summary>
        Right = 3,
    }

    /// <summary>
    /// Перечисление типов рельефа для тайлов.
    /// </summary>
    public enum ETerrain
    {
        /// <summary>
        /// Вода.
        /// </summary>
        Water = 0,

        /// <summary>
        /// Трава.
        /// </summary>
        Grass = 1,

        /// <summary>
        /// Побережье.
        /// </summary>
        LandSide = 2,

        /// <summary>
        /// Склон холма.
        /// </summary>
        HillSide = 3,

        /// <summary>
        /// Место перед склоном холма.
        /// </summary>
        FootHills = 4,
    }

    /// <summary>
    /// Класс, представляющий тайл в двумерном пространстве.
    /// </summary>
    public class Tile
    {
        /// <summary>
        /// Массив масок совместимости для различных типов тайлов.
        /// </summary>
        private static readonly int[] CompatibilityMask =
        {
            0b00001, // Water
            0b00010, // Grass
            0b00100, // LandSide
            0b01000, // HillSide
            0b00111  // FootHills (Water, Grass, LandSide)
        };

        /// <summary>
        /// Множество уникальных типов тайлов, основанных на хеше рельефа.
        /// </summary>
        private static readonly HashSet<int> TileTypes = new();

        /// <summary>
        /// Массив сторон тайла, представленных как кортежи трех целых чисел.
        /// </summary>
        public (int, int, int)[] TerrainSides { get; } = new (int, int, int)[4];

        /// <summary>
        /// Генерация хеша рельефа для заданного массива типов рельефа.
        /// </summary>
        /// <param name="terrain">Массив из 9 целых чисел, представляющих рельеф.</param>
        /// <returns>Хеш рельефа.</returns>
        /// <exception cref="ArgumentException">Возникает, если длина массива не равна 9.</exception>
        private static int GetTerrainHash(int[] terrain)
        {
            if (terrain.Length != 9)
            {
                throw new ArgumentException("Array must have exactly 9 elements.");
            }

            int hash = 17;
            const int prime = 31;

            foreach (var t in terrain)
            {
                hash = hash * prime + t;
            }
            return hash;
        }

        /// <summary>
        /// Конструктор класса Tile. Инициализирует тайл на основе переданного массива типов рельефа.
        /// </summary>
        /// <param name="terrain">Массив из 9 целых чисел, представляющих рельеф тайла.</param>
        /// <exception cref="ArgumentException">Возникает, если длина массива не равна 9 или если тайл с таким рельефом уже существует.</exception>
        public Tile(int[] terrain)
        {
            if (terrain.Length != 9)
                throw new ArgumentException("Array must have exactly 9 elements.");

            if (!TileTypes.Add(GetTerrainHash(terrain)))
                throw new ArgumentException("Same Tile already exists.");

            // Инициализация сторон тайла
            TerrainSides =
            [
                (terrain[0], terrain[1], terrain[2]), // Up
                (terrain[6], terrain[7], terrain[8]), // Down
                (terrain[0], terrain[3], terrain[6]), // Left
                (terrain[2], terrain[5], terrain[8])  // Right
            ];
        }

        /// <summary>
        /// Вспомогательный метод для вычисления масок совместимости сторон тайла.
        /// </summary>
        /// <param name="t1">Первый тип рельефа.</param>
        /// <param name="t2">Второй тип рельефа.</param>
        /// <param name="t3">Третий тип рельефа.</param>
        /// <returns>Кортеж масок совместимости для трех типов рельефа.</returns>
        private static (int, int, int) GetSideMask(int t1, int t2, int t3)
        {
            return (CompatibilityMask[t1], CompatibilityMask[t2], CompatibilityMask[t3]);
        }

        /// <summary>
        /// Проверяет возможность соседства двух тайлов в указанных направлениях.
        /// </summary>
        /// <param name="tileA">Первый тайл.</param>
        /// <param name="tileB">Второй тайл.</param>
        /// <param name="directionA">Направление для первого тайла.</param>
        /// <param name="directionB">Направление для второго тайла.</param>
        /// <returns>True, если тайлы могут быть соседями, иначе False.</returns>
        public static bool IsPossibleNeighbors(Tile tileA, Tile tileB, EDirection directionA, EDirection directionB)
        {
            var (terrainA1, terrainA2, terrainA3) = tileA.TerrainSides[(int)directionA];
            var (terrainB1, terrainB2, terrainB3) = tileB.TerrainSides[(int)directionB];

            return IsPossibleNeighbors((terrainA1, terrainA2, terrainA3), (terrainB1, terrainB2, terrainB3));
        }

        /// <summary>
        /// Проверяет возможность соседства двух сторон тайлов.
        /// </summary>
        /// <param name="a">Сторона первого тайла, представлена как кортеж типов рельефа.</param>
        /// <param name="b">Сторона второго тайла, представлена как кортеж типов рельефа.</param>
        /// <returns>True, если стороны могут быть соседями, иначе False.</returns>
        public static bool IsPossibleNeighbors((int A1, int A2, int A3) a, (int B1, int B2, int B3) b)
        {
            return (CompatibilityMask[a.A1] & CompatibilityMask[b.B1]) != 0 &&
                   (CompatibilityMask[a.A2] & CompatibilityMask[b.B2]) != 0 &&
                   (CompatibilityMask[a.A3] & CompatibilityMask[b.B3]) != 0;
        }
    }
}