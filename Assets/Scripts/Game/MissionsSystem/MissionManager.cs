using System;
using Game;
using Game.WorldSystem;
using UnityEngine;
using Mirror;
using Networking;
using UnityEngine.Events;
using Snowy.Utils;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class MissionManager : NetworkSingleton<MissionManager>
{
    public UnityEvent OnMissionStart;
    public UnityEvent OnMissionEnd;
    
    [SerializeField] MissionData[] missions;
    [SyncVar] public bool isMissionActive;
    [SyncVar] public int currentMissionIndex = -1;
    
    private bool m_isEndingMission;
    public MissionData CurrentMission => missions[currentMissionIndex];

    [Server]
    public void MissionStart(MissionData data)
    {
        WorldsManager.Instance.LoadWorld(data.worldData, 5, true);
        WorldsManager.Instance.OnSceneLoadedEvent.AddListener(OnSceneLoaded);
        isMissionActive = true;
        m_isEndingMission = false;
        currentMissionIndex = Array.IndexOf(missions, data);
    }
    
    public void MissionEnd()
    {
        Debug.Log("Try Mission End");
        if (!isMissionActive) return;
        
        if (isServer)
        {
            Debug.Log("Server Mission End");
            ServerMissionEnd();
        }
        else
        { 
            CmdMissionEnd();
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode, WorldData worldData)
    {
        Invoke(nameof(SendMissionStart), 1f);
    }

    private void SendMissionStart()
    {
        RpcMissionStart();
    }
    
    public List<MissionData> GetAllMissionAvaliable()
    {
        return missions.ToList();
    }
    
    [Server]
    private void ServerMissionEnd()
    {
        Debug.Log("Mission End");
        if (m_isEndingMission) return;
        Debug.Log("Mission End 2");
        // Teleport back
        m_isEndingMission = true;
        isMissionActive = false;
        currentMissionIndex = -1;
        WorldsManager.Instance.LoadWorld(0, 5, false);
        
        RpcMissionEnd();
    }
    
    [Command(requiresAuthority = false)]
    private void CmdMissionEnd()
    {
        Debug.Log("CmdMissionEnd");
        ServerMissionEnd();
    }
    
    [ClientRpc]
    private void RpcMissionEnd()
    {
        OnMissionEnd.Invoke();
        isMissionActive = false;
        currentMissionIndex = -1;
        // TODO: Show the ui
    }
    
    [ClientRpc]
    private void RpcMissionStart()
    {
        OnMissionStart.Invoke();
        
        // TODO: Show the ui
    }
    
    
}
