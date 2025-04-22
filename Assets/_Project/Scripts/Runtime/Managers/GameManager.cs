using NJG.Utilities.Singletons;

namespace NJG.Runtime.Managers
{
    public class GameManager : SingletonPersistent<GameManager>
    {
        public int Energy { get; private set; }

        public void AddEnergy(int energy)
        {
            Energy += energy;
        }
        
        public bool HasEnoughEnergy(int energy)
        {
            return Energy >= energy;
        }
    }
}