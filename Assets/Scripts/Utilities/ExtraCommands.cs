using Networking;
using SnTerminal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utilities
{
    public static class ExtraCommands
    {
        [RegisterCommand(Name = "ghost_map", Help = "Enters the ghost test map", MinArgCount = 0, MaxArgCount = 0)]
        public static void CommandCreateLobby(CommandArg[] args)
        {
            SceneManager.LoadScene("GhostTestMap");
        }
        
        [RegisterCommand(Name = "main", Help = "Enters the main menu", MinArgCount = 0, MaxArgCount = 0)]
        public static void CommandLobby(CommandArg[] args)
        {
            SceneManager.LoadScene(0);
            
            // unlock cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}