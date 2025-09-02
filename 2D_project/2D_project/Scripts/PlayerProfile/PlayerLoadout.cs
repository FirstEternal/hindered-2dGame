using System;

internal class PlayerLoadout
{
    private bool isLockedIn;

    private float[] _playerStats = new float[5];

    public float[] playerStats
    {
        get
        {
            if (isLockedIn)
                throw new InvalidOperationException("Stats are locked during gameplay.");
            return _playerStats;
        }
        set
        {
            _playerStats = value;
        }
    }


    private int[] _elements = new int[3];
    public int[] elements
    {
        get
        {
            if (isLockedIn)
                throw new InvalidOperationException("Stats are locked during gameplay.");
            return _elements;
        }
        set
        {
            elements = value;
        }
    }

}