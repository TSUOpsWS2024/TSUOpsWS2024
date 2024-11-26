using System.Text.Json;

namespace WFC2D
{
    public static class DataConverter
    {
        public static int GetOppositeDirectionIndex(int input)
        {
            return input % 2 == 0 ? input + 1 : input - 1;
        }

        /// <summary>
        /// Сериализует TileSet в JSON.
        /// </summary>
        /// <param name="tileSet">Экземпляр TileSet для сериализации.</param>
        /// <returns>Строка JSON.</returns>
        public static string Serialize(TileSet tileSet)
        {
            // Преобразуем тайлы в формат DTO
            var dto = new TileSetDTO
            {
                Tiles = new List<int[]>(),
                Resolution = tileSet.Resolution,
                FolderPaff = tileSet.FolderPaff
            };

            foreach (var tile in tileSet.Tiles)
            {
                // Восстанавливаем исходный 3x3 массив рельефа из сторон тайла
                var terrain = new int[9]
                {
                    tile.TerrainSides[0].Item1, tile.TerrainSides[0].Item2, tile.TerrainSides[0].Item3,
                    tile.TerrainSides[2].Item2, -1, tile.TerrainSides[3].Item2,
                    tile.TerrainSides[1].Item1, tile.TerrainSides[1].Item2, tile.TerrainSides[1].Item3
                };

                dto.Tiles.Add(terrain);
            }

            return JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true });
        }

        /// <summary>
        /// Десериализует TileSet из JSON.
        /// </summary>
        /// <param name="json">Строка JSON.</param>
        /// <returns>Экземпляр TileSet.</returns>
        public static TileSet Deserialize(string json)
        {
            var dto = JsonSerializer.Deserialize<TileSetDTO>(json);
            if (dto == null || dto.Tiles == null)
            {
                throw new ArgumentException("Invalid JSON data for TileSet.");
            }

            // Преобразуем DTO обратно в TileSet
            var tileSet = new TileSet(dto.Tiles.ToArray())
            {
                Resolution = dto.Resolution,
                FolderPaff = dto.FolderPaff
            };

            return tileSet;
        }

        /// <summary>
        /// Сериализует TileSet и сохраняет в JSON-файл.
        /// </summary>
        /// <param name="tileSet">Экземпляр TileSet для сохранения.</param>
        /// <param name="filePath">Путь к файлу для сохранения.</param>
        public static void SaveToFile(TileSet tileSet, string filePath)
        {
            var json = Serialize(tileSet);
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Считывает JSON-файл и десериализует его в TileSet.
        /// </summary>
        /// <param name="filePath">Путь к файлу JSON.</param>
        /// <returns>Экземпляр TileSet.</returns>
        public static TileSet LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            var json = File.ReadAllText(filePath);
            return Deserialize(json);
        }
    }
}