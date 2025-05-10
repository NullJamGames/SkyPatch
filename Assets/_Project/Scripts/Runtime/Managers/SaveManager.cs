using UnityEngine;
using Zenject;

namespace NJG.Runtime.Managers
{
    public class SaveManager : IInitializable, ILateDisposable
    {
        public void Initialize() { }

        public void LateDispose() { }

        public void UnlockLevel(int level)
        {
            PlayerPrefs.SetInt($"Level{level}", 1);
            PlayerPrefs.Save();
        }

        public bool IsLevelUnlocked(int level) => PlayerPrefs.GetInt($"Level{level}", 0) == 1;

        public void SelectCharacter(bool isMale)
        {
            PlayerPrefs.SetInt("Character", isMale ? 1 : 0);
            PlayerPrefs.Save();
        }

        public bool HasCharacter(out bool isMale)
        {
            if (PlayerPrefs.HasKey("Character"))
            {
                isMale = PlayerPrefs.GetInt("Character") == 1;
                return true;
            }

            isMale = false;
            return false;
        }
    }
}