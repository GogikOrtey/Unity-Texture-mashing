using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseTextureClear : MonoBehaviour
{
    private Texture2D texture;
    [Range(0f, 1f)] private float shiftPercentage = 0.1f; // % ��������� ��������
    [Range(0, 100)] public int countOfOreaShifts = 70; // ���������� ��������, ������� ����� �������� �� ���� ��������
    public int maxShiftDistance = 2000; // ������������ ���������, �� ������� ����� ���� ���������� �������

    // �������� ������� � ������ ������� ��� ������
    public int minAreaWidth = 50;
    public int maxAreaWidth = 1000;

    // � ������ ����������� �������
    public int minAreaHeight = 50;
    public int maxAreaHeight = 1000;

    // �������� ���� ������ � ������� � ��������� (��������, � ���������), � ������ �� ������� �������������
    // �������, ��� ��� ������ �� PlayMode, ������� ��������� � ������������ 

    void Start()
    {
        Renderer renderer = this.gameObject.GetComponent<Renderer>();
        Texture2D sourceTexture = (Texture2D)renderer.material.mainTexture;

        Vector3 objectSize = renderer.bounds.size;
        Vector2 textureSize = new Vector2(sourceTexture.width, sourceTexture.height);

        // ������, ������� ��� �������� ����������� �� �������
        Vector2 repeatCount = renderer.material.mainTextureScale;

        // ������� ����� ��������, ��������������� ������� �������
        Texture2D texture = new Texture2D(Mathf.RoundToInt(textureSize.x * repeatCount.x), Mathf.RoundToInt(textureSize.y * repeatCount.y), TextureFormat.RGBA32, true);

        // ��������� ����� �������� ��������� �������� ��������
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

    // ����� ��� ������ �������� �������� �� ��������
    Texture2D ShiftAreaPixels5(Texture2D texture)
    {
        // �������� ��� ������� �� ��������
        Color[] pixels = texture.GetPixels();
        int totalPixels = pixels.Length;
        // ��������� ���������� �������� ��� ������
        int shiftPixelsCount = Mathf.RoundToInt(totalPixels * shiftPercentage);

        // ��������� ������� ������ ��� ��������� ���������� ��������
        for (int i = 0; i < countOfOreaShifts; i++)
        {
            // �������� ��������� �������
            int randomIndex = Random.Range(0, totalPixels);
            // �������� ��������� ���������� ��� ������ �� X � Y
            int shiftDistanceX = Random.Range(-maxShiftDistance, maxShiftDistance + 1);
            int shiftDistanceY = Random.Range(-maxShiftDistance, maxShiftDistance + 1);

            // �������� ��������� ������ � ������ ��� �������
            int areaWidth = Random.Range(minAreaWidth, maxAreaWidth + 1);
            int areaHeight = Random.Range(minAreaHeight, maxAreaHeight + 1);

            // ���������� ��� ������� � �������
            for (int x = 0; x < areaWidth; x++)
            {
                for (int y = 0; y < areaHeight; y++)
                {
                    // ��������� ������ ������� � ������ ��� ������
                    int indexX = (randomIndex % texture.width + x) % texture.width;
                    int indexY = (randomIndex / texture.width + y) % texture.height;
                    int index = indexY * texture.width + indexX;

                    int shiftedIndexX = (indexX + shiftDistanceX + texture.width) % texture.width;
                    int shiftedIndexY = (indexY + shiftDistanceY + texture.height) % texture.height;
                    int shiftedIndex = shiftedIndexY * texture.width + shiftedIndexX;

                    // ���� ��������� ������ � �������� ������ ��������, ������ ������� �������
                    if (shiftedIndex >= 0 && shiftedIndex < totalPixels)
                    {
                        Color temp = pixels[index];
                        pixels[index] = pixels[shiftedIndex];
                        pixels[shiftedIndex] = temp;
                    }
                }
            }
        }

        // ��������� ��������� � ��������
        texture.SetPixels(pixels);
        texture.Apply();

        // ���������� ���������� ��������
        return (texture);
    }

    // ������ ����� - ����� �������� � ��������
    void ShiftPixels()
    {
        Color[] pixels = texture.GetPixels();
        int totalPixels = pixels.Length;
        int shiftPixelsCount = Mathf.RoundToInt(totalPixels * shiftPercentage);

        for (int i = 0; i < shiftPixelsCount; i++)
        {
            int randomIndex = Random.Range(0, totalPixels);
            int shiftDistance = Random.Range(1, maxShiftDistance + 1);
            // ������������, ��� ������ �������� � �������� ������ ��������
            int shiftedIndex = (randomIndex + shiftDistance) % totalPixels;             

            // ������ ������� �������
            Color temp = pixels[randomIndex];
            pixels[randomIndex] = pixels[shiftedIndex];
            pixels[shiftedIndex] = temp;
        }

        texture.SetPixels(pixels);
        texture.Apply();
    }
}
