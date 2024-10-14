using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using player;

namespace minimap;

class MiniMap
{
    private int size;
    private float scale;

    //colors
    private Color bgColor = new Color(60, 0, 100);
    private Color playerColor = new Color(0, 220, 0);
    private Color rayColor = new Color(220, 0, 220);
    private Color blockColor = new Color(0, 220, 220);
    public MiniMap(int s, int blockSize, int mapLength)
    {
        size = s;
        scale = (float)size / (blockSize * mapLength);
    }

    //method for drawing minimap
    public void DrawMap(SpriteBatch _spriteBatch, Texture2D blankTexture, Player player, float fov, float rays, List<int[]> raylengths, List<Rectangle> blocks)
    {
        //drawing minimap background
        _spriteBatch.Draw(blankTexture, new Vector2(0, 0), new Rectangle(0, 0, size, size), bgColor, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
        
        //drawing mini player
        Rectangle scaledPlayer = new Rectangle(Convert.ToInt32(player.playerRect.X * scale), Convert.ToInt32(player.playerRect.Y * scale), Convert.ToInt32(player.playerRect.Width * scale), Convert.ToInt32(player.playerRect.Height * scale));
         _spriteBatch.Draw(blankTexture, new Vector2(scaledPlayer.X, scaledPlayer.Y), scaledPlayer, playerColor, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
       
       //drawing mini rays
       var rayAngle = fov / -2;
        for (int i = 0; i < raylengths.Count; i++)
        {
            _spriteBatch.Draw(blankTexture, new Vector2(scaledPlayer.X + scaledPlayer.Width / 2, scaledPlayer.Y + scaledPlayer.Height / 2), null, rayColor, MathHelper.ToRadians(player.angle + rayAngle + 90), new Vector2(0, 0), new Vector2(raylengths[i][0] * scale, 1), SpriteEffects.None, 0);
            rayAngle += fov / rays;
        }

        //drawing mini blocks
        foreach (Rectangle block in blocks)
        {
            Rectangle scaledBlock = new Rectangle(Convert.ToInt32(block.X * scale), Convert.ToInt32(block.Y * scale), Convert.ToInt32(block.Width * scale), Convert.ToInt32(block.Height * scale));
            _spriteBatch.Draw(blankTexture, new Vector2(scaledBlock.X, scaledBlock.Y), scaledBlock, blockColor, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
        }
    }
}