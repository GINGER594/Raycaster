using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using player;

namespace caster;

class Caster
{
    private float fov;
    private float rays;
    private int maxLength;
    private const int lengthIncrement = 2;
    private const int cornerHitBoxSize = 2;
    public Caster(float f, float r, int m)
    {
        fov = f;
        rays = r;
        maxLength = m;
    }
    
    //ray casting method, returns list of ray lengths
    public List<int[]> CastRays(Player player, List<Rectangle> blocks)
    {
        //raylengths[0] = length
        //raylengths[1] = corner or not: 0 = not, 1 = corner
        List<int[]> rayLengths = new List<int[]>();
        for (float i = fov / -2; i < fov / 2; i += fov / rays)
        {
            for (int length = 0; length < maxLength; length += lengthIncrement)
            {
                Matrix lineRotation = Matrix.CreateRotationZ(MathHelper.ToRadians(player.angle + i));
                Vector2 linePos = new Vector2(player.playerRect.X + player.playerRect.Width / 2, player.playerRect.Y + player.playerRect.Height / 2) + Vector2.Transform(new Vector2(0, length), lineRotation);
                Rectangle linePoint = new Rectangle(Convert.ToInt32(linePos.X), Convert.ToInt32(linePos.Y), 1, 1);
                var collisionType = CheckRayCollision(linePoint, blocks);
                if (collisionType != 0)
                {
                    rayLengths.Add([length, collisionType - 1]);
                    break;
                }
            }
        }
        return rayLengths;
    }

    //collision checking method for rays
    //0 = no collision
    //1 = normal collision,
    //2 = corner collision
    private int CheckRayCollision(Rectangle point, List<Rectangle> blocks)
    {
        foreach (Rectangle block in blocks)
        {
            if (point.Intersects(block))
            {
                //checking if its a corner
                Rectangle topLeft = new Rectangle(block.X, block.Y, cornerHitBoxSize, cornerHitBoxSize);
                Rectangle topRight = new Rectangle(block.X + block.Width - cornerHitBoxSize, block.Y,  cornerHitBoxSize, cornerHitBoxSize);
                Rectangle bottomLeft = new Rectangle(block.X, block.Y + block.Height - cornerHitBoxSize, cornerHitBoxSize, cornerHitBoxSize);
                Rectangle bottomRight = new Rectangle(block.X + block.Width - cornerHitBoxSize, block.Y + block.Height - cornerHitBoxSize, cornerHitBoxSize, cornerHitBoxSize);
                if (point.Intersects(topLeft) || point.Intersects(topRight) || point.Intersects(bottomLeft) || point.Intersects(bottomRight))
                {
                    return 2;
                }
                return 1;
            }
        }
        return 0;
    }
}