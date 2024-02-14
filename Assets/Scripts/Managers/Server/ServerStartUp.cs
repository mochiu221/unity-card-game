using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class ServerStartUp : NetworkBehaviour
{

    private void Awake() 
    {
        string[] args = System.Environment.GetCommandLineArgs();
        string serverAddress = "127.0.0.1";
        ushort serverPort = 7777;
        bool isServer = false;
        bool isListenServer = false;

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-server" && i + 1 < args.Length)
            {
                isServer = true;
                serverAddress = args[i + 1];
            }

            if (args[i] == "-port" && i + 1 < args.Length)
            {
                serverPort = (ushort) int.Parse(args[i+1]);
            }

            if (args[i] == "-listen" && i + 1 < args.Length && args[i+1] == "1")
            {
                isListenServer = true;
            }
        }

        if (isServer)
        {
            // Invoke("startHostFn", 1.0f); //invoke時に引数でポートを渡すようにしてみる
            StartCoroutine(startHostFn(1.0f, serverAddress, serverPort, isListenServer));
        }
        else
        {
            //パラメータがない場合、1秒後にクライアントとして動作させる場合
            //StartCoroutine(startClientFn(1.0f));
        };

        IEnumerator startHostFn(float delay, string serverAddress, ushort serverPort, bool isListenServer)
        {
            yield return new WaitForSeconds(delay);

            if (isListenServer)
            {
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                    serverAddress,
                    serverPort,
                    "0.0.0.0"
                );
            }
            else
            {
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                    serverAddress,
                    serverPort
                );
            }


            Debug.Log("started as Host: " + serverAddress + " Port:" + serverPort + " isListenSever:" + isListenServer);

            NetworkManager.Singleton.StartHost();

            Debug.Log("started!");
        }

        // IEnumerator startClientFn(float delay)
        // {
        //     yield return new WaitForSeconds(delay);
        //     NetworkManager.Singleton.StartClient();
        // }

    }
}
