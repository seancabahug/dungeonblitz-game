using UnityEngine;
using NativeWebSocket;
using Newtonsoft.Json.Linq;

namespace DungeonBlitz
{
    public class WebSocketConnection : MonoBehaviour
    {
        private Game game;
        private WebSocket socket;

        void Start() {
            game = GetComponent<Game>();
        }
        void Update() {
            #if !UNITY_WEBGL || UNITY_EDITOR
                socket.DispatchMessageQueue();
            #endif
        }

        public async void Connect(string webSocketServerURL) {
            socket = new WebSocket(webSocketServerURL);

            socket.OnOpen += () =>
            {
                Debug.Log("Connection open!");
            };

            socket.OnError += (e) =>
            {
                Debug.Log("Error! " + e);
            };

            socket.OnClose += (e) =>
            {
                Debug.Log("Connection closed!");
            };

            socket.OnMessage += (bytes) =>
            {
                var message = System.Text.Encoding.UTF8.GetString(bytes);
                game.HandleEvent(message);
            };

            await socket.Connect();
        }

        public async void SendData(object data) {
            await socket.SendText(JObject.FromObject(data).ToString());
        }

        private async void OnApplicationQuit() {
            await socket.Close();
        }
    }
}