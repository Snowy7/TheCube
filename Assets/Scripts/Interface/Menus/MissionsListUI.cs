using Actors.Player;
using Networking;
using System.Collections.Generic;
using Interface;
using SnInput;
using UnityEngine;
using UnityEngine.InputSystem;

public class MissionsListUI : MonoBehaviour
{
    [SerializeField] MissionInfoUI missionInfoPrefab;
    [SerializeField] private Transform missionsParent;
    private List<GameObject> missionsList = new List<GameObject>();
    private Player player;
    
    private void OnEnable()
    {
        MenuManager.Instance.CanToggleMenu = false;
        player = ClientsManager.Instance.LocalClient.Player;
        SetCursorLockState(false);
        InputManager.OnLockCursor += DisableUI;
        SpawnMissions();
    }

    private void DisableUI(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            gameObject.SetActive(false);
        }
    }


    private void SpawnMissions()
    {
        // Gameplay elemts
        List<MissionData> missionsAvilable = MissionManager.Instance.GetAllMissionAvaliable();

        for (int i = 0; i < missionsAvilable.Count; i++)
        {
            MissionInfoUI mission = Instantiate(missionInfoPrefab, missionsParent);
            mission.Init(missionsAvilable[i].MissionName, missionsAvilable[i], 3, transform.root.gameObject);
            missionsList.Add(mission.gameObject);
        }
    }

    private void OnDisable()
    {
        foreach (var mission in missionsList)
        {
            Destroy(mission);
        }
        missionsList.Clear();
        SetCursorLockState(true);
        MenuManager.Instance.CanToggleMenu = true;
        
        // unsubscribe from event
        InputManager.OnLockCursor -= DisableUI;
    }

    private void SetCursorLockState(bool state)
    {
        player?.OnLockCursor(state);
    }
}
