using kcp2k;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public interface ISessionComponent
{
    void Initialize();
	void Connect(SceneModuleParam param);

	void Disconnect();
}

/// <summary>
/// ��Ī�� ������ ��ġ �������� �����´�.
/// ��ġ ������ guid�� ���� ��ġ ��û�� �����ϸ�, ������ ������ ��� �ݹ��� ��´�.
/// �� �ο� ��ΰ� ���� �Ϸ�ǰ�, �غ� �Ϸ�Ǹ�, ������ StartHost�� ȣ���Ѵ�.
/// StartHost�� �Ϸ�� ���, �ش� ȣ��Ʈ�� ������ �ش� �� �ο� ����� StartClient�� ȣ���Ų��.
/// ��� ������ �Ϸ�ȴٸ�, ��ġ �������� ������ �����ϰ� ��ġ�� �ı��Ѵ�.
/// �� ���Ĵ� �������� �ʴ´�.
/// 
/// ��ġ ������ TCP ����̴�..
/// </summary>

public class MatchSessionManager : NetworkManager<MatchSessionManager>, ISessionComponent
{
    /// <summary>
    /// Open matches that are available for joining
    /// </summary>
    private Dictionary<Guid, MatchInfo> openMatches = new Dictionary<Guid, MatchInfo>();

    /// <summary>
    /// Player informations by Network Connection
    /// </summary>
    private PlayerInfo[] playerInfos = null;

    /// <summary>
    /// �����ΰ�?
    /// </summary>
    private bool isOwner = false;

    /// <summary>
    /// GUID of a match the local player has created
    /// </summary>
    private Guid localPlayerMatch = Guid.Empty;

    /// <summary>
    /// GUID of a match the local player has joined
    /// </summary>
    private Guid localJoinedMatch = Guid.Empty;

    /// <summary>
    /// GUID of a match the local player has selected in the Toggle Group match list
    /// </summary>
    private Guid selectedMatch = Guid.Empty;

    public void Connect(SceneModuleParam param)
    {
        StartClient();
    }

    public void Disconnect()
    {
        StopClient();
    }

    public override void Initialize()
    {
        base.Initialize();
        InitializeData();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        InitializeData();
        NetworkClient.RegisterHandler<ClientMatchMessage>(OnClientMatchMessage);
    }

    /// <summary>
    /// Called on the client when connected to a server.
    /// <para>The default implementation of this function sets the client as ready and adds a player. Override the function to dictate what happens when the client connects.</para>
    /// </summary>
    public override void OnClientConnect()
    {
        InitializeData();
        base.OnClientConnect();
    }

    /// <summary>
    /// Called on clients when disconnected from a server.
    /// <para>This is called on the client when it disconnects from the server. Override this function to decide what happens when the client disconnects.</para>
    /// </summary>
    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        InitializeData();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        NetworkClient.UnregisterHandler<ClientMatchMessage>();
        InitializeData();
    }

    internal void InitializeData()
    {
        isOwner = false;
        openMatches.Clear();
        selectedMatch = Guid.Empty;
        localPlayerMatch = Guid.Empty;
        localJoinedMatch = Guid.Empty;
    }

    /// <summary>
    /// Assigned in inspector to Create button
    /// </summary>
    [ClientCallback]
    public void RequestCreateMatch()
    {
        NetworkClient.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Create });
    }

    /// <summary>
    /// Assigned in inspector to Join button
    /// </summary>
    [ClientCallback]
    public void RequestJoinMatch()
    {
        if (selectedMatch == Guid.Empty) return;

        NetworkClient.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Join, matchId = selectedMatch });
    }

    /// <summary>
    /// Assigned in inspector to Leave button
    /// </summary>
    [ClientCallback]
    public void RequestLeaveMatch()
    {
        if (localJoinedMatch == Guid.Empty) return;

        NetworkClient.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Leave, matchId = localJoinedMatch });
    }

    /// <summary>
    /// Assigned in inspector to Cancel button
    /// </summary>
    [ClientCallback]
    public void RequestCancelMatch()
    {
        if (localPlayerMatch == Guid.Empty) return;

        NetworkClient.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Cancel });
    }

    /// <summary>
    /// Assigned in inspector to Ready button
    /// </summary>
    [ClientCallback]
    public void RequestReadyChange()
    {
        if (localPlayerMatch == Guid.Empty && localJoinedMatch == Guid.Empty) return;

        Guid matchId = localPlayerMatch == Guid.Empty ? localJoinedMatch : localPlayerMatch;

        NetworkClient.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Ready, matchId = matchId });
    }

    /// <summary>
    /// Assigned in inspector to Start button
    /// </summary>
    [ClientCallback]
    public void RequestStartMatch()
    {
        if (localPlayerMatch == Guid.Empty) return;

        NetworkClient.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Start });
    }

    /// <summary>
    /// Called from <see cref="MatchController.RpcExitGame"/>
    /// </summary>
    [ClientCallback]
    public void OnMatchEnded()
    {
        localPlayerMatch = Guid.Empty;
        localJoinedMatch = Guid.Empty;
    }

    private void OnClientMatchMessage(ClientMatchMessage msg)
    {
        switch (msg.clientMatchOperation)
        {
            case ClientMatchOperation.List:
                {
                    openMatches.Clear();
                    foreach (MatchInfo matchInfo in msg.matchInfos)
                        openMatches.Add(matchInfo.matchId, matchInfo);
                    break;
                }
            case ClientMatchOperation.Created:
                {
                    localPlayerMatch = msg.matchId;
                    playerInfos = msg.playerInfos;
                    isOwner = true;
                    break;
                }
            case ClientMatchOperation.Cancelled:
                {
                    localPlayerMatch = Guid.Empty;
                    break;
                }
            case ClientMatchOperation.Joined:
                {
                    localJoinedMatch = msg.matchId;
                    playerInfos = msg.playerInfos;
                    isOwner = false;
                    break;
                }
            case ClientMatchOperation.Departed:
                {
                    localJoinedMatch = Guid.Empty;
                    break;
                }
            case ClientMatchOperation.UpdateRoom:
                {
                    playerInfos = msg.playerInfos;
                    break;
                }
            case ClientMatchOperation.Started:
                {
                    OnStartGame();
                    break;
                }
        }
    }

    private void OnStartGame()
    {
        SceneManager.Instance.LoadScene(SceneType.Battle, new BattleSceneModuleParam(isOwner));
    }
}