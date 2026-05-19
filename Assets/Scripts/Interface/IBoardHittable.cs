using UnityEngine;

public interface IBoardHittable
{
    void HandleBallHit(BallView ballView, ContactPoint2D contact);
}