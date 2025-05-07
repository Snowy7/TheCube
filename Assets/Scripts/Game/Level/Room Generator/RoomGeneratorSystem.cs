using System;
using UnityEngine;
using Mirror;
using Networking;
using System.Collections;
using Game;
using Network;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using Snowy.CSharp;

namespace RoomGenerator
{
    // A Synced class for generating rooms in the level over the network
    public class RoomGeneratorSystem : NetworkSingleton<RoomGeneratorSystem>
    {
        [SerializeField] private RoomSizeSet[] roomSets;
        Dictionary<RoomType, int> roomTypeCounter = new Dictionary<RoomType, int>();
        [SyncVar] SyncDictionary<int, int> map = new SyncDictionary<int, int>();

        /// <summary>
        /// Called when the client starts
        /// </summary>
        public override void OnStartClient()
        {
            base.OnStartClient();

            map.OnChange += OnMapChange;

            if (isServer)
            {
                Generate();
            }
            else
            {
                // LATE JOIN
                foreach (var pair in map)
                {
                    OnMapChange(SyncDictionary<int, int>.Operation.OP_SET, pair.Key, pair.Value);
                }
            }
        }

        /// <summary>
        /// Called when the map changes
        /// </summary>
        /// <param name="op"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void OnMapChange(SyncDictionary<int, int>.Operation op, int key, int value)
        {
            if (!isClient) return;

            if (op == SyncDictionary<int, int>.Operation.OP_SET || op == SyncDictionary<int, int>.Operation.OP_ADD)
            {
                RoomInfo room = roomSets[key].rooms[value];
                Transform spawnPoint = roomSets[key].spawnPosition;
                SpawnRoom(room, spawnPoint);
            }
        }

        /// <summary>
        /// Generate the rooms for the level
        /// </summary>
        [Server]
        void Generate()
        {
            if (!isServer) return;

            Dictionary<int, int> initialMap = new();

            for (var i = 0; i < roomSets.Length; i++)
            {
                var roomSet = roomSets[i];
                // Get a random room from the room set
                RoomInfo room = GetRoom(roomSet.rooms, out int index);

                // Add the room to the map
                initialMap.Add(i, index);
            }

            // Check for minimum amount for each room type;
            initialMap = CheckForMinAmount(initialMap);

            // Set the map
            map = new SyncDictionary<int, int>(initialMap);

            // spawn for server
            foreach (var pair in initialMap)
            {
                OnMapChange(SyncDictionary<int, int>.Operation.OP_SET, pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Check for minimum amount of rooms
        /// </summary>
        Dictionary<int, int> CheckForMinAmount(Dictionary<int, int> map)
        {
            // Retrieve all the room types, by getting all the room types from the room sets and then getting the distinct room types
            List<RoomInfo> roomInfos = roomSets.SelectMany(r => r.rooms).Distinct().ToList();

            foreach (var roomInfo in roomInfos)
            {
                int ammount = roomTypeCounter[roomInfo.roomType];
                if(roomInfo.minAmount < ammount)
                {
                    map.Add(roomInfos.IndexOf(roomInfo),ammount +1);
                }
            }

            return map;
        }

        /// <summary>
        /// Get a room from the room set
        /// </summary>
        /// <param name="rooms"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        RoomInfo GetRoom(RoomInfo[] rooms, out int index)
        {
            List<RoomInfo> roomsList = rooms.ToList();
            List<RoomInfo> blackList = new List<RoomInfo>();
            index = Random.Range(0, rooms.Length);
            RoomInfo room = rooms[index];

            int safeCounter = 0;
            bool chosen = false;

            while (!chosen && safeCounter < 25)
            {
                if (roomTypeCounter.TryGetValue(room.roomType, out int value))
                {
                    if (room.maxAmount <= value)
                    {
                        blackList.Add(room);
                        roomsList.Remove(room);
                    }
                    else
                    {
                        roomTypeCounter[room.roomType]++;
                        chosen = true;
                    }
                }
                else
                {
                    roomTypeCounter[room.roomType] = 1;
                    chosen = true;
                }
                

                room = roomsList[Random.Range(0, roomsList.Count)];
                index = rooms.ToList().IndexOf(room);

                safeCounter++;
            }

            if (!chosen)
            {
                // Take the room with the lowest minAmount
                room = blackList[0];
                index = rooms.ToList().IndexOf(room);
            }


            return room;
        }

        /// <summary>
        /// Spawn a room at a spawn point
        /// </summary>
        /// <param name="info"></param>
        /// <param name="spawnPoint"></param>
        void SpawnRoom(RoomInfo info, Transform spawnPoint)
        {
            // TODO:
            // Instantiate the room prefab at the spawn point
            Instantiate(info.roomPrefab, spawnPoint.position, Quaternion.identity);
        }

        /*[SerializeField] private RoomSizeSet[] Rooms;
        [SerializeField] private int WorldSeed = 00000;
        private Dictionary<RoomSizeSet, int> RoomsHasSpawned = new Dictionary<RoomSizeSet, int>();

        private void Start()
        {
            WorldSeed = Random.Range(0, 99999999);
            if (isServer) SpawnRandomRooms();
        }
        [Server]
        public void SpawnRandomRooms()
        {
            RPC_SpawnRandomRooms();
        }

        [ClientRpc]
        public void RPC_SpawnRandomRooms()
        {
            for (int i = 0; i < Rooms.Length; i++)
            {
                Random.InitState(WorldSeed * i);
                RoomSizeSet roomSize = Rooms[Random.Range(0,Rooms.Length)];
                if (RoomsHasSpawned.TryGetValue(Rooms[i], out int value))
                {
                    if (value < Rooms[i].MaxAmmount)
                    {
                        GameObject roomprefab = Instantiate(Rooms[i].room.roomPrefab, Rooms[i].spawnPosition.position, Quaternion.identity);
                        NetworkClient.RegisterPrefab(roomprefab);
                        RoomsHasSpawned[Rooms[i]] = value + 1;
                    }
                }
                else
                {
                    Rooms[i].MaxAmmount = Random.Range(1, 4);
                    GameObject roomprefab = Instantiate(Rooms[i].room.roomPrefab, Rooms[i].spawnPosition.position, Quaternion.identity);
                    NetworkClient.RegisterPrefab(roomprefab);
                    RoomsHasSpawned.Add(Rooms[i], 1);
                }

                Debug.Log($"Key: {Rooms[i].room.name}, Value: {RoomsHasSpawned[Rooms[i]]}");
            }
        }


        private void GetAllRoomsAndChoose()
        {
            for (int i = 0; i < Rooms.Length; i++)
            {
                Random.InitState(WorldSeed * i);
                if (RoomsHasSpawned.TryGetValue(Rooms[i], out int value))
                {
                    if (value < Rooms[i].MaxAmmount)
                    {
                        GameObject roomprefab = Instantiate(Rooms[i].room.roomPrefab, Rooms[i].spawnPosition.position, Quaternion.identity);
                        NetworkClient.RegisterPrefab(roomprefab);
                        RoomsHasSpawned[Rooms[i]] = value + 1;
                    }
                }
                else
                {
                    Rooms[i].MaxAmmount = Random.Range(1, 4);
                    GameObject roomprefab = Instantiate(Rooms[i].room.roomPrefab, Rooms[i].spawnPosition.position, Quaternion.identity);
                    NetworkClient.RegisterPrefab(roomprefab);
                    RoomsHasSpawned.Add(Rooms[i], 1);
                }

                Debug.Log($"Key: {Rooms[i].room.name}, Value: {RoomsHasSpawned[Rooms[i]]}");
            }
        }*/

    }
}
