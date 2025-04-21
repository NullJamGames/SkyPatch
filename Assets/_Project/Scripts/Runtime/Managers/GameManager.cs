using NJG.Utilities.Singletons;

namespace NJG.Runtime.Managers
{
    public class GameManager : SingletonPersistent<GameManager>
    {
        public int Score { get; private set; }

        public void AddScore(int score)
        {
            Score += score;
        }
    }
}