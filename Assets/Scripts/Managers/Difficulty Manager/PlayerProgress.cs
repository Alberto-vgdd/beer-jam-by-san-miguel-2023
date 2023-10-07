public class PlayerProgress
{
    public int playerNumber;
    public int levelNumber;
    public float difficulty;
    public int completedBoxes;
    public int objectiveCompletedBoxes;
    public int totalScore;
    public int livesLeft;

    public PlayerProgress(int playerNumber)
    {
        this.playerNumber = playerNumber;
    }

    public void Reset()
    {
        levelNumber = 0;
        difficulty = 0;
        completedBoxes = 0;
        objectiveCompletedBoxes = 0;
        totalScore = 0;
        livesLeft = 3;
    }
}