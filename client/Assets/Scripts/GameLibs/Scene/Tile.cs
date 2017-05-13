using UnityEngine;
using System.Collections;
namespace TKGame
{
    public class Tile
    {
        public bool m_bIsDigable = true;
        private Texture2D m_stSpriteTexture;
        public int Width
        {
            get { return m_stSpriteTexture.width; }
        }
        public int Height
        {
            get { return m_stSpriteTexture.height; }
        }
        private int m_iPosX;
        private int m_iPosY;

        public Tile(int posX, int posY, Texture2D texture)
        {
            m_iPosX = posX;
            m_iPosY = posY;
            m_stSpriteTexture = texture;
        }

        public float GetAlpha(int x, int y)
        {
            int textureX = x - m_iPosX;
            int textureY = y - m_iPosY;
            if(textureX < 0 || textureX > m_stSpriteTexture.width || textureY < 0 || textureY > m_stSpriteTexture.height)
            {
                return 0;
            }

            Color clr = m_stSpriteTexture.GetPixel(textureX, textureY);
            return clr.a;
        }

        public void Dig(int centerX, int centerY, Texture2D surface, Texture2D border = null)
        {
            if (!m_bIsDigable || m_stSpriteTexture == null)
            {
                return;
            }

            int posX = centerX - surface.width/2 - m_iPosX;
            int posY = centerY - surface.height/2 - m_iPosY;
            int width = surface.width;
            int height = surface.height;

            int srcWidth = m_stSpriteTexture.width;
            int srcHeight = m_stSpriteTexture.height;

            if (posX + width <= 0 || posX >= srcWidth || posY + height <= 0 || posY >= srcHeight)
            {//不在挖洞范围内
                return;
            } 

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    int x = i + posX;
                    int y = j + posY;
                    if (x >= 0 && x < srcWidth && y >= 0 && y < srcHeight)
                    {
                        Color color = surface.GetPixel(i, j);
                        Color c = m_stSpriteTexture.GetPixel(x, y);
                        c.a = (1 - color.a) * c.a;
                        m_stSpriteTexture.SetPixel(x, y, c);
                    }
                }
            }

            if (border != null)
            {
                posX = centerX - border.width / 2;
                posY = centerY - border.height / 2;
                width = border.width;
                height = border.height;
                for (int i = 0; i < width; ++i)
                {
                    for (int j = 0; j < height; ++j)
                    {
                        int x = i + posX;
                        int y = j + posY;
                        if (x >= 0 && x < srcWidth && y >= 0 && y < srcHeight)
                        {
                            Color color = border.GetPixel(i, j);
                            Color c = m_stSpriteTexture.GetPixel(x, y);
                            if (c.a > 0 && color.a > 0)
                            {
                                float a = color.a;
                                color = (1 - a) * c + a * color;
                                color.a = c.a;
                                m_stSpriteTexture.SetPixel(x, y, color);
                            }
                        }
                    }
                }
            }
            m_stSpriteTexture.Apply();
        }
    }
}