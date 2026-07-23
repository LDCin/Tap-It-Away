public class LevelState
{
    public int RemainingCubeCount { get; private set; }
    public int RemainingHeartCount { get; private set; }

    public bool IsCompleted => RemainingCubeCount <= 0;
    public bool IsFailed => RemainingHeartCount <= 0;

    public LevelState(int cubeCount, int heartCount)
    {
        RemainingCubeCount = cubeCount;
        RemainingHeartCount = heartCount;
    }

    public void RemoveCube()
    {
        if (RemainingCubeCount > 0)
        {
            RemainingCubeCount--;
        }
    }

    public void LoseHeart()
    {
        if (RemainingHeartCount > 0)
        {
            RemainingHeartCount--;
        }
    }
}