using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseTextureClear : MonoBehaviour
{
    private Texture2D texture;
    [Range(0f, 1f)] private float shiftPercentage = 0.1f; // % изменения текстуры
    [Range(0, 100)] public int countOfOreaShifts = 70; // Количество областей, которые будут сдвинуты по всей текстуре
    public int maxShiftDistance = 2000; // Максимальная дистанция, на которую может быть перенесена область

    // Задаются верхние и нижние границы для высоты
    public int minAreaWidth = 50;
    public int maxAreaWidth = 1000;

    // и ширины переносимой области
    public int minAreaHeight = 50;
    public int maxAreaHeight = 1000;

    // Добавьте этот скрипт к объекту с текстурой (например, к плоскости), и скрипт всё сделает автоматически
    // Помните, что при выходе из PlayMode, тестура сбросится к оригинальной 

    void Start()
    {
        Renderer renderer = this.gameObject.GetComponent<Renderer>();
        Texture2D sourceTexture = (Texture2D)renderer.material.mainTexture;

        Vector3 objectSize = renderer.bounds.size;
        Vector2 textureSize = new Vector2(sourceTexture.width, sourceTexture.height);

        // Узнаем, сколько раз текстура повторяется на объекте
        Vector2 repeatCount = renderer.material.mainTextureScale;

        // Создаем новую текстуру, соответствующую размеру объекта
        Texture2D texture = new Texture2D(Mathf.RoundToInt(textureSize.x * repeatCount.x), Mathf.RoundToInt(textureSize.y * repeatCount.y), TextureFormat.RGBA32, true);

        // Заполняем новую текстуру пикселями исходной текстуры
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                texture.SetPixel(x, y, sourceTexture.GetPixel(x % sourceTexture.width, y % sourceTexture.height));
            }
        }

        texture.Apply();

        texture = ShiftAreaPixels5(texture);

        texture.filterMode = FilterMode.Trilinear;

        renderer.material.mainTextureScale = new Vector2(1, 1);
        renderer.material.mainTexture = texture;

        print("Texture is done!");
    }

    // Метод для сдвига областей пикселей на текстуре
    Texture2D ShiftAreaPixels5(Texture2D texture)
    {
        // Получаем все пиксели из текстуры
        Color[] pixels = texture.GetPixels();
        int totalPixels = pixels.Length;
        // Вычисляем количество пикселей для сдвига
        int shiftPixelsCount = Mathf.RoundToInt(totalPixels * shiftPercentage);

        // Повторяем процесс сдвига для заданного количества областей
        for (int i = 0; i < countOfOreaShifts; i++)
        {
            // Выбираем случайный пиксель
            int randomIndex = Random.Range(0, totalPixels);
            // Выбираем случайное расстояние для сдвига по X и Y
            int shiftDistanceX = Random.Range(-maxShiftDistance, maxShiftDistance + 1);
            int shiftDistanceY = Random.Range(-maxShiftDistance, maxShiftDistance + 1);

            // Выбираем случайную ширину и высоту для области
            int areaWidth = Random.Range(minAreaWidth, maxAreaWidth + 1);
            int areaHeight = Random.Range(minAreaHeight, maxAreaHeight + 1);

            // Перебираем все пиксели в области
            for (int x = 0; x < areaWidth; x++)
            {
                for (int y = 0; y < areaHeight; y++)
                {
                    // Вычисляем индекс пикселя и индекс для сдвига
                    int indexX = (randomIndex % texture.width + x) % texture.width;
                    int indexY = (randomIndex / texture.width + y) % texture.height;
                    int index = indexY * texture.width + indexX;

                    int shiftedIndexX = (indexX + shiftDistanceX + texture.width) % texture.width;
                    int shiftedIndexY = (indexY + shiftDistanceY + texture.height) % texture.height;
                    int shiftedIndex = shiftedIndexY * texture.width + shiftedIndexX;

                    // Если сдвинутый индекс в пределах границ текстуры, меняем местами пиксели
                    if (shiftedIndex >= 0 && shiftedIndex < totalPixels)
                    {
                        Color temp = pixels[index];
                        pixels[index] = pixels[shiftedIndex];
                        pixels[shiftedIndex] = temp;
                    }
                }
            }
        }

        // Применяем изменения к текстуре
        texture.SetPixels(pixels);
        texture.Apply();

        // Возвращаем измененную текстуру
        return (texture);
    }

    // Старый метод - сдвиг пикселей в текстуре
    void ShiftPixels()
    {
        Color[] pixels = texture.GetPixels();
        int totalPixels = pixels.Length;
        int shiftPixelsCount = Mathf.RoundToInt(totalPixels * shiftPercentage);

        for (int i = 0; i < shiftPixelsCount; i++)
        {
            int randomIndex = Random.Range(0, totalPixels);
            int shiftDistance = Random.Range(1, maxShiftDistance + 1);
            // Обеспечиваем, что индекс остается в пределах границ текстуры
            int shiftedIndex = (randomIndex + shiftDistance) % totalPixels;             

            // Меняем местами пиксели
            Color temp = pixels[randomIndex];
            pixels[randomIndex] = pixels[shiftedIndex];
            pixels[shiftedIndex] = temp;
        }

        texture.SetPixels(pixels);
        texture.Apply();
    }
}
