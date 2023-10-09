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

public interface IMatchSessionSubscriber
{
    void OnChangedMyMatch(bool isOwner, Guid matchId);
    void OnChangedMatchList(Dictionary<Guid, MatchInfo> openMatches);
    void OnChangedPlayerInfos(PlayerInfo[] playerInfos);
}

/// <summary>
/// 매칭이 가능한 매치 정보들을 가져온다.
/// 매치 정보의 guid를 통해 매치 신청이 가능하며, 참가에 성공한 경우 콜백을 뱉는다.
/// 방 인원 모두가 참여 완료되고, 준비가 완료되면, 방장이 StartHost를 호출한다.
/// StartHost가 완료된 경우, 해당 호스트의 서버로 해당 방 인원 모두의 StartClient를 호출시킨다.
/// 모두 접속이 완료된다면, 매치 서버와의 접속을 종료하고 매치를 파괴한다.
/// 그 이후는 관여하지 않는다.
/// 
/// 매치 서버는 TCP 기반이다..
/// </summary>

public class MatchSessionManager : NetworkManager<MatchSessionManager>, ISessionComponent
{
    private Dictionary<Guid, MatchInfo> openMatches = new Dictionary<Guid, MatchInfo>();
    private PlayerInfo[] playerInfos = null;

    private Guid localPlayerMatch = Guid.Empty;
    private Guid selectedMatch = Guid.Empty;

	private bool isOwner = false;

	private List<IMatchSessionSubscriber> subscribers = new List<IMatchSessionSubscriber>();

    public void Connect(SceneModuleParam param)
    {
        StartClient();
    }

    public void Disconnect()
    {
        StopClient();
    }

    public void SelectMatch(Guid matchId)
    {
        selectedMatch = matchId;
	}

    public void SubscribeMatchInfo(IMatchSessionSubscriber subscriber)
    {
        if(subscribers.Contains(subscriber) == false)
		    subscribers.Add(subscriber);
	}

    public void UnSubscribeMatchInfo(IMatchSessionSubscriber subscriber)
    {
		if (subscribers.Contains(subscriber))
			subscribers.Remove(subscriber);
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
        if (localPlayerMatch == Guid.Empty) return;

        NetworkClient.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Leave, matchId = localPlayerMatch });
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
        if (localPlayerMatch == Guid.Empty) return;

        NetworkClient.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Ready, matchId = localPlayerMatch });
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
					localPlayerMatch = msg.matchId;
                    playerInfos = msg.playerInfos;
                    isOwner = false;
                    break;
                }
            case ClientMatchOperation.Departed:
                {
					localPlayerMatch = Guid.Empty;
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

    public void Tick()
    {
        
    }

    public void OnTickServer()
    {
        
    }
}