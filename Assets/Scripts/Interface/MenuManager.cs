using Snowy.Utils;

namespace Interface
{
    public class MenuManager : MonoSingleton<MenuManager>
    {
        public bool CanToggleMenu { get; set; } = true;

        public static bool CanToggle
        {
            get => Instance.CanToggleMenu;
            set => Instance.CanToggleMenu = value;
        }
    }
}