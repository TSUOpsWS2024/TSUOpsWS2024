using SkiaSharp;

namespace WFC2D
{
    /// <summary>
    /// Класс для рендеринга области данных в изображение, используя тайлы и значения энтропии.
    /// </summary>
    public class AreaRenderer : IDisposable
    {
        /// <summary>
        /// Массив битмапов, содержащих изображения всех тайлов из набора.
        /// </summary>
        private SKBitmap[] TileSetSKBitmaps { get; }

        /// <summary>
        /// Массив битмапов, содержащих изображения значений энтропии для каждой ячейки области.
        /// </summary>
        private SKBitmap[] EntropyBitmaps { get; }

        /// <summary>
        /// Разрешение (размер) одного тайла.
        /// </summary>
        private int Resolution { get; }

        /// <summary>
        /// Конструктор класса, инициализирующий рендерер для заданного набора тайлов.
        /// Загружает изображения тайлов и создает изображения для значений энтропии.
        /// </summary>
        /// <param name="tileSet">Набор тайлов, из которого будут загружаться изображения.</param>
        public AreaRenderer(TileSet tileSet)
        {
            int tileCount = tileSet.TileCount;
            Resolution = tileSet.Resolution;

            // Инициализация массива для хранения битмапов тайлов
            TileSetSKBitmaps = new SKBitmap[tileCount];

            // Загрузка битмапов тайлов с диска
            for (int i = 0; i < tileCount; i++)
            {
                string tilePath = Path.Combine(tileSet.FolderPaff, "Assets", $"{i}.png");
                TileSetSKBitmaps[i] = SKBitmap.Decode(tilePath);
            }

            // Инициализация массива для хранения битмапов значений энтропии
            EntropyBitmaps = new SKBitmap[tileCount + 1]; // от 0 до tileCount включительно

            // Подготавливаем кисть для текста значений энтропии
            using (var paint = new SKPaint { Color = SKColors.Black, TextSize = 12 })
            {
                // Создание битмапов для всех значений энтропии
                for (int i = 0; i <= tileCount; i++)
                {
                    // Создаем новый битмап для текущего значения энтропии
                    var entropyBitmap = new SKBitmap(Resolution, Resolution);
                    using (var canvas = new SKCanvas(entropyBitmap))
                    {
                        // Очищаем фон битмапа, заполняя его прозрачным цветом
                        canvas.Clear(SKColors.Transparent);

                        // Подготавливаем текст для отображения значения энтропии
                        string entropyText = i.ToString();
                        SKRect textBounds = new SKRect();
                        paint.MeasureText(entropyText, ref textBounds);

                        // Определяем координаты, чтобы текст оказался по центру битмапа
                        float x = (Resolution - textBounds.Width) / 2 - textBounds.Left;
                        float y = (Resolution - textBounds.Height) / 2 - textBounds.Top;

                        // Рисуем текст на битмапе
                        canvas.DrawText(entropyText, x, y, paint);
                    }

                    // Сохраняем созданный битмап в массиве значений энтропии
                    EntropyBitmaps[i] = entropyBitmap;
                }
            }
        }

        /// <summary>
        /// Метод рендеринга области данных в изображение SKBitmap.
        /// Рисует на изображении тайлы и значения энтропии для каждой ячейки области.
        /// </summary>
        /// <param name="data">Двумерный массив с данными ячеек области.</param>
        /// <returns>Рендеренное изображение области.</returns>
        public SKBitmap RenderAreaToBitmap(AreaCell[,] data)
        {
            int areaWidth = data.GetLength(1);
            int areaHeight = data.GetLength(0);

            int imageWidth = areaWidth * Resolution;
            int imageHeight = areaHeight * Resolution;

            // Создаем итоговое изображение
            var bitmap = new SKBitmap(imageWidth, imageHeight);
            using (var canvas = new SKCanvas(bitmap))
            {
                // Заполняем фон изображения прозрачным цветом
                canvas.Clear(SKColors.Transparent);

                // Рисуем каждую ячейку из data на холсте
                for (int i = 0; i < areaHeight; i++)
                {
                    for (int j = 0; j < areaWidth; j++)
                    {
                        AreaCell cell = data[i, j];
                        float x = j * Resolution;
                        float y = i * Resolution;

                        if (cell.CollapsedId != -1)
                        {
                            // Если ячейка "коллапсирована", рисуем тайл
                            canvas.DrawBitmap(TileSetSKBitmaps[cell.CollapsedId], x, y);
                        }
                        else if (cell.Entropy > 0)
                        {
                            // Если энтропия больше 0, отображаем ее значение
                            canvas.DrawBitmap(EntropyBitmaps[cell.Entropy], x, y);
                        }
                    }
                }
            }

            // Возвращаем полученное изображение
            return bitmap;
        }

        /// <summary>
        /// Освобождает все ресурсы, связанные с объектами класса, включая битмапы тайлов и значений энтропии.
        /// </summary>
        public void Dispose()
        {
            // Освобождение всех битмапов тайлов
            foreach (var bitmap in TileSetSKBitmaps)
            {
                bitmap?.Dispose();
            }

            // Освобождение всех битмапов значений энтропии
            foreach (var bitmap in EntropyBitmaps)
            {
                bitmap?.Dispose();
            }
        }
    }
}