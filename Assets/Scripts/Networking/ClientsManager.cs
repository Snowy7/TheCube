using System.Collections.Generic;
using System.Linq;
using Actors.Player;
using Mirror;
using Snowy.Utils;
using UnityEngine;

namespace Networking
{
    public class ClientsManager : MonoSingleton<ClientsManager>
    {
        public List<Client> clients = new();
        
        public event System.Action<Client> OnLocalClientSpawned;
        
        public Client LocalClient { get; private set; }
        
        public void WaitForAllClients(System.Action<List<Client>> callback)
        {
            StartCoroutine(WaitForAllClientsRoutine(callback));
        }
        
        private System.Collections.IEnumerator WaitForAllClientsRoutine(System.Action<List<Client>> callback)
        {
            yield return new WaitUntil(() => clients.All(client => client.Player != null));
            callback(clients);
        }
        
        public void RegisterClient(Client client, bool isLocal = false)
        {
            clients.Add(client);

            if (!isLocal) return;
            
            LocalClient = client;
            OnLocalClientSpawned?.Invoke(client);
        }
        
        public void UnRegisterClient(Client client)
        {
            clients.Remove(client);
        }
        
        public void WaitForLocalClient(System.Action<Client> callback)
        {
            if (LocalClient != null)
            {
                if (NetworkClient.ready)
                    callback(LocalClient);
                else
                    StartCoroutine(WaitForLocalClientRoutine(callback));
            }
            else
            {
                OnLocalClientSpawned += client =>
                {
                    callback(client);
                    OnLocalClientSpawned -= callback;
                };
            }
        }
        
        public void WaitForClient(Player player, System.Action<Client> callback, float timeout = 10f)
        {
            StartCoroutine(WaitForClientRoutine(player, callback, timeout));
        }
        
        private System.Collections.IEnumerator WaitForClientRoutine(Player player, System.Action<Client> callback, float timeout)
        {
            float timer = 0;
            while (timer < timeout)
            {
                Client client = GetClient(player);
                if (client != null)
                {
                    callback(client);
                    yield break;
                }

                timer += Time.deltaTime;
                yield return null;
            }
        }
        
        private System.Collections.IEnumerator WaitForLocalClientRoutine(System.Action<Client> callback)
        {
            yield return new WaitUntil(() => NetworkClient.ready);
            callback(LocalClient);
        }

        public void Clear()
        {
            foreach (Client client in clients)
            {
                Destroy(client.gameObject);
            }

            clients.Clear();
        }

        public Client GetClient(Player player)
        {
            return clients.Find(client => client.Player == player);
        }
    }
}