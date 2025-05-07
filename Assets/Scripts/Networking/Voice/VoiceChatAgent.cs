using Mirror;
using Steamworks;
using UnityEngine;

namespace Networking.Voice
{
    public class VoiceChatAgent : NetworkBehaviour
    {
        public AudioSource audioSource;

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            
            // Start recording audio
            SteamUser.StartVoiceRecording();
        }

        private void Update()
        {
            if (!isLocalPlayer) return;
         
            AudioProcessing();
        }

        void AudioProcessing()
        {
            uint compressed;
            
            EVoiceResult ret = SteamUser.GetAvailableVoice(out compressed);

            if (ret == EVoiceResult.k_EVoiceResultOK && compressed > 1024)
            {
                byte[] destBuffer = new byte[1024];
                uint bytesWritten;
                ret = SteamUser.GetVoice(true, destBuffer, 1024, out bytesWritten);
                if (ret == EVoiceResult.k_EVoiceResultOK && bytesWritten > 0)
                {
                    // Send the voice data to the server
                    CmdSendVoiceData(destBuffer, bytesWritten);
                }
            }
        }
        
        [Command]
        void CmdSendVoiceData(byte[] data, uint length)
        {
            // send to all clients but the sender
            foreach (NetworkConnection conn in NetworkServer.connections.Values)
            {
                if (conn != connectionToClient)
                {
                    TargetReceiveVoiceData(conn, data, length);
                }
            }
        }
        
        [TargetRpc]
        void TargetReceiveVoiceData(NetworkConnection conn, byte[] data, uint length)
        {
            byte[] destBuffer2 = new byte[22050 * 2];
            EVoiceResult ret = SteamUser.DecompressVoice(data, length, destBuffer2, (uint)destBuffer2.Length, out var bytesWritten2, 22050);
            if(ret == EVoiceResult.k_EVoiceResultOK && bytesWritten2 > 0)
            {
                audioSource.clip = AudioClip.Create(UnityEngine.Random.Range(100,1000000).ToString(), 22050, 1, 16000, false);

                float[] test = new float[22050];
                for (int i = 0; i < test.Length; ++i)
                {
                    test[i] = (short)(destBuffer2[i * 2] | destBuffer2[i * 2 + 1] << 8) / 32768.0f;
                }
                audioSource.clip.SetData(test, 0);
                audioSource.Play();
            }
        }
    }
}