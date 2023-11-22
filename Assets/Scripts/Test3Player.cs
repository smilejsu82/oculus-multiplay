using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

// RpcSources : 전송할 수 있는 피어
// RpcTargets : 피어가 실행되는 피어
// 1. All : 모두에게 전송 / 세션 내의 모든 피어에 의해서 실행됨(서버 포함)
// 2. Proxies : 나 말고 전송 / 객체에 대하여 입력 권한 또는 상태 권한을 갖고 있지 않는 피어에 의해 실행됨
// 3. InputAuthority : 입력 권한 있는 피어만 전송 / 객체에 대한 입력 권한이 있는 피어에 의해 실행됨
// 4. StateAuthority : 상태 권한 있는 피어만 전송 / 객체에 대한 상태 권한이 있는 피어에 의해 실행됨
// RpcInfo
// - Tick : 어떤 곳에서 틱이 전송되었는지
// - Source : 어떤 플레이어(PlayerRef)가 보냈는지
// - Channel : Unrealiable 또는 Reliable RPC로 보냈는지 여부
// - IsInvokeLocal : 이 RPC를 원래 호출한 로컬 플레이어인지의 여부
// * 공식 문서엔 HostMode를 설정하지 않았지만 이걸 쓰지 않으면 계속 원격 플레이어가 된다. (기본이 서버 모드여서 그런 듯)

public class Test3Player : NetworkBehaviour, INetworkRunnerCallbacks
{
    [Networked] public string playerName { get; set; }
    
    [Networked] public Vector3 headPosition { get; set; }
    [Networked] public Vector3 leftHandPosition { get; set; }
    [Networked] public Vector3 leftHandRotation { get; set; }
    [Networked] public Vector3 rightHandPosition { get; set; }
    [Networked] public Vector3 rightHandRotation { get; set; }


    [SerializeField] private Transform headTrans;
    [SerializeField] private Transform leftHandTrans;
    [SerializeField] private Transform rightHandTrans;
    
    private Transform centerEyeAnchor;
    private Transform leftHandAnchor;
    private Transform rightHandAnchor;
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_Configure(string name)    {
        playerName = name;
        Debug.LogFormat("playerName: <color=yellow>{0}</color>", playerName);
    }

    public void Init(NetworkRunner runner)
    {
        Debug.LogFormat("[Init] {0}", runner.IsServer);
        
        if(Object.HasInputAuthority && runner.IsServer) //host 
            runner.AddCallbacks(this);
    }

    public override void Spawned()
    {
        Debug.Log("Spawned");
        Debug.Log(Object == this.GetComponent<NetworkObject>());    //true
        Debug.LogFormat("Object.HasInputAuthority: {0}", Object.HasInputAuthority); //내 입력권한 여부 
        Debug.LogFormat("Object.HasStateAuthority: {0}", Object.HasStateAuthority); //내 상태권한 여부
        
        //자신거
        EventDispatcher.instance.Dispatch("Spawned", this);    
    }

    public void SetleftHandAnchor(Transform leftHandAnchor)
    {
        Debug.LogFormat("SetleftHandAnchor: {0}", leftHandAnchor);
        
        this.leftHandAnchor = leftHandAnchor;
    }
    public void SetRightHandAnchor(Transform rightHandAnchor)
    {
        Debug.LogFormat("SetRightHandAnchor: {0}", rightHandAnchor);
        
        this.rightHandAnchor = rightHandAnchor;
    }

    public void SetCenterEyeAnchor(Transform centerEyeAnchor)
    {
        Debug.LogFormat("SetCenterEyeAnchor: {0}", leftHandAnchor);
        
        this.centerEyeAnchor = centerEyeAnchor;
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        Debug.LogFormat("[OnInput] : {0}", runner.IsServer);
        
        var data = new NetworkInputData();
        Debug.LogFormat("centerEyeAnchor : {0}", centerEyeAnchor);
        Debug.LogFormat("leftHandAnchor : {0}", leftHandAnchor);
        data.headPosition = this.centerEyeAnchor.position;
        
        data.leftHandPosition = this.leftHandAnchor.position;
        data.leftHandRotation = this.leftHandAnchor.rotation.eulerAngles;
        
        data.rightHandPosition = this.rightHandAnchor.position;
        data.rightHandRotation = this.rightHandAnchor.rotation.eulerAngles;
        
        Debug.LogFormat("data.headPosition : {0}", data.headPosition);
        Debug.LogFormat("data.leftHandPosition : {0}", data.leftHandPosition);
        
        input.Set(data);
    }
    
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            this.headPosition = data.headPosition;
            this.leftHandPosition = data.leftHandPosition;
            this.leftHandRotation = data.leftHandRotation;
            this.rightHandPosition = data.rightHandPosition;
            this.rightHandRotation = data.rightHandRotation;
            
            Debug.LogFormat("{0}, {1}, {2}", data.headPosition, data.leftHandPosition, data.rightHandPosition);
        }
    }

    public override void Render()
    {
        this.headTrans.position = this.headPosition;
        
        this.leftHandTrans.position = this.leftHandPosition;
        this.leftHandTrans.rotation = Quaternion.Euler(this.leftHandRotation);
        
        this.rightHandTrans.position = this.rightHandPosition;
        this.rightHandTrans.rotation = Quaternion.Euler(this.rightHandRotation);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }
}

public struct NetworkInputData : INetworkInput
{
    public Vector3 headPosition;
    public Vector3 leftHandPosition;
    public Vector3 leftHandRotation;
    public Vector3 rightHandPosition;
    public Vector3 rightHandRotation;
    
}

