using System;
using NJG.Runtime.Entity;

namespace NJG.Runtime.UI.Tooltips
{
    public interface ITooltipProvider
    {
        public string GetTooltipText(PlayerInventory playerInventory);
        public event Action<string> OnTooltipTextChanged;
    }
}