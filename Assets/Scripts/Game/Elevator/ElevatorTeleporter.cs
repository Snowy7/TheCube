using Mirror;
using Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorTeleporter : NetworkSingleton<ElevatorTeleporter>
{
    public List<GameObject> PlayersInElevator = new List<GameObject>();
    public Animator DoorAnimator;
    private MissionData currentMission;



    public void SetMissionData(MissionData missionData)
    {
        if (!isServer) return;

        int id = MissionManager.instance.GetAllMissionAvaliable().IndexOf(missionData);
        Debug.Log("Mission Updated");
        OpenElevator(id);

    }

    [Server]
    private void OpenElevator(int missionid)
    {
        RPC_OpenElevator(missionid);
        Debug.Log("Server Asked");
    }
    [ClientRpc(includeOwner =false)]
    private void RPC_OpenElevator(int missionID)
    {
        DoorAnimator.SetTrigger("Open");
        List<MissionData> missions = MissionManager.instance.GetAllMissionAvaliable();
        currentMission = missions[missionID];
        Debug.Log("The Door Should be opened");
        StartCoroutine(CheckPlayersInElevator());
    }

    IEnumerator CheckPlayersInElevator()
    {
        yield return new WaitForSeconds(1);
        if (PlayersInElevator.Count >= ClientsManager.Instance.clients.Count)
        {
            MissionManager.instance.MissionStart(currentMission);
            CloseDoor();
        }else
        {
            StartCoroutine(CheckPlayersInElevator());
        }

    }

    public void AddPlayer(GameObject obj)
    {
        if (isServer)
        {
            Server_AddPlayer(obj);
        }
        else
        {
            Cmd_AddPlayer(obj);
        }
    }

    private void CloseDoor()
    {
        ServerCloseDoor();
    }

    [Server]
    private void ServerCloseDoor()
    {
        RPC_CloseDoor();
    }

    [ClientRpc]
    private void RPC_CloseDoor()
    {
        DoorAnimator.SetTrigger("Close");
    }
    [Server]
    private void Server_AddPlayer(GameObject obj)
    {
        RPC_AddPlayer(obj);
    }
    [Command]
    private void Cmd_AddPlayer(GameObject obj)
    {
        RPC_AddPlayer(obj);
    }
    [ClientRpc]
    private void RPC_AddPlayer(GameObject obj)
    {
        if (!PlayersInElevator.Contains(obj))
            PlayersInElevator.Add(obj);
    }
}
