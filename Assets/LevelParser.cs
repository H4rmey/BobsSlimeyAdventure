using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TileId
{
    Empty   = -1,
    Water   = 0,
    Ground  = 1,
    Player  = 2,
    Start   = 3,
    End     = 4,
    Self    = 5,
    Shrink  = 6,
    Narrow  = 7,
    Grow    = 8
}

public class LevelParser : MonoBehaviour
{
    Texture2D t_level;
    public int levelAmount = 6;

    public struct LevelData 
    {
        public List<TileId> layerBackground;
        public List<TileId> layerItem;

        public Vector2 gridSize;
    }

    public LevelData[] levels = new LevelData[6];

    Texture2D rotateTexture(Texture2D originalTexture, bool clockwise)
    {
        Color32[] original = originalTexture.GetPixels32();
        Color32[] rotated = new Color32[original.Length];
        int w = originalTexture.width;
        int h = originalTexture.height;

        int iRotated, iOriginal;

        for (int j = 0; j < h; ++j)
        {
            for (int i = 0; i < w; ++i)
            {
                iRotated = (i + 1) * h - j - 1;
                iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                rotated[iRotated] = original[iOriginal];
            }
        }

        Texture2D rotatedTexture = new Texture2D(h, w);
        rotatedTexture.SetPixels32(rotated);
        rotatedTexture.Apply();
        return rotatedTexture;
    }

    public void GenerateLevels()
    {
        for (int k = 0; k < levelAmount; k++)
        {
            string currentLevelName = "Levels/Level" + (k + 1);
            t_level = Resources.Load(currentLevelName) as Texture2D;

            t_level = rotateTexture(t_level, true);

            Color[] colors = t_level.GetPixels();

            levels[k].layerBackground   = new List<TileId>();
            levels[k].layerItem         = new List<TileId>();
            levels[k].gridSize          = new Vector2(t_level.width, t_level.height);

            /*
             *  Background
             */
            for (int i = 0; i < colors.Length; i++)
            {
                Color col = colors[i];
                if (col == new Color(0f, 0f, 0f, 1f)) //black/Ground
                {
                    levels[k].layerBackground.Add(TileId.Ground);
                }
                else if (col == new Color(0f, 1f, 1f, 1f)) //cyan/water
                {
                    levels[k].layerBackground.Add(TileId.Water);
                }
                else
                {
                    levels[k].layerBackground.Add(TileId.Ground);
                }
            }


            /*
             *  Items
             */
            for (int i = 0; i < colors.Length; i++)
            {
                Color col = colors[i];
                if (col == new Color(1f, 0f, 0f, 1f)) //red/end
                {
                    levels[k].layerItem.Add(TileId.End);
                }
                else if (col == new Color(1f, 1f, 0f, 1f)) //yellow/water
                {
                    levels[k].layerItem.Add(TileId.Shrink);
                }
                else if (col == new Color(1f, 1f, 0f, 1f)) //green/water
                {
                    levels[k].layerItem.Add(TileId.Narrow);
                }
                else if (col == new Color(0f, 0f, 1f, 1f)) //blue/water
                {
                    levels[k].layerItem.Add(TileId.Grow);
                }
                else if (col == new Color(1f, 0f, 1f, 1f)) //magenta/start
                {
                    levels[k].layerItem.Add(TileId.End);
                }
                else
                {
                    levels[k].layerItem.Add(TileId.Empty);
                }
            }
        }
    }
}
