public class PlayerPieceDifficulty
{
    public int playerNumber;

    public float difficulty;

    public BottlePiece nextPiece;

    public PlayerPieceDifficulty(int playerNumber)
    {
        this.playerNumber = playerNumber;
    }

    public void OnDifficultyChanged(float newDifficulty, int newLevelDisplayNumber)
    {
        difficulty = newDifficulty;
    }

}