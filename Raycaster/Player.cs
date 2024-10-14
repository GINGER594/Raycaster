using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace player;

class Player
{
    private int size;
    private Vector2 speed = new Vector2(0, 2);
    private const int rotateSpeed = 2;
    public int angle { get; set; }
    public Rectangle playerRect { get; set; }
    public Player(int s, int x, int y)
    {
        size = s;
        angle = 0;
        playerRect = new Rectangle(x, y, size, size);
    }

    //method for moving player
    public void Move(char direction, List<Rectangle> blocks)
    {
        int angleModifier;
        switch (direction)
        {
            //forward
            case 'f':
                angleModifier = 0;
                break;
            //backward
            case 'b':
                angleModifier = 180;
                break;
            //right
            case 'r':
                angleModifier = 90;
                break;
            //left
            default:
                angleModifier = -90;
                break;
        }

        //creating a rectangle of where the player is going to be after they are moved
        Matrix playerRotation = Matrix.CreateRotationZ(MathHelper.ToRadians(angle + angleModifier));
        Vector2 movedPlayerPos = new Vector2(playerRect.X, playerRect.Y) + Vector2.Transform(speed, playerRotation);
        Rectangle movedPlayer = new Rectangle(Convert.ToInt32(movedPlayerPos.X), Convert.ToInt32(movedPlayerPos.Y), size, size);

        //only moving the player if the moved player rectangle does not collide with anything
        if (!CheckPlayerCollision(movedPlayer, blocks))
        {
            playerRect = movedPlayer;
        }
    }

    //method for rotating player
    public void Rotate(char direction)
    {
        switch (direction)
        {
            //left rotation
            case 'l':
                angle -= rotateSpeed;
                break;
            //right rotation
            default:
                angle += rotateSpeed;
                break;
        }
    }

    //method for checking player collision, returns true if there is a collision, and false if not
    public bool CheckPlayerCollision(Rectangle movedPlayer, List<Rectangle> blocks)
    {
        foreach (Rectangle block in blocks)
        {
            if (movedPlayer.Intersects(block))
            {
                return true;
            }
        }
        return false;
    }
}