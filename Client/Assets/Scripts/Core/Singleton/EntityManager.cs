using Mirror;
using NPOI.HPSF;
using StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public interface IEntityCaptureReceiver
{
	void OnCapture(FrameEntityMessage playerEntityMessage, FrameEntityMessage[] entitiyMessages);
}

public class EntityManager : Singleton<EntityManager>
{
    public EntityComponent PlayerEntity
    {
        get
        {
            if (entityDictionary.TryGetValue(PlayerEntityGuid, out var component))
            {
                return component;
            }

            Debug.LogError($"플레이어 오브젝트가 생성되기 전입니다. GUID : {PlayerEntityGuid}");
            return null;
        }
    }

    public int PlayerGuid { get; private set; }
	public int PlayerEntityGuid { get; private set; }
    private Dictionary<int, EntityComponent> entityDictionary = new Dictionary<int, EntityComponent>();
    private List<IEntityCaptureReceiver> entityCaptureReceivers = new List<IEntityCaptureReceiver>();

    private FrameEntityMessage[] currentFrameEntitiesSnapshot = null;
    private FrameEntityMessage currentFramePlayerEntitySnapshot = new FrameEntityMessage();

	protected override void OnAwakeInstance()
	{
        PlayerGuid = 0;
	}

	public EntityComponent GetEntityComponent(int guid)
    {
        if (entityDictionary.ContainsKey(guid) == false)
            return null;

        return entityDictionary[guid];
    }

    private IEnumerable<EntityComponent> GetMyEntities(int ownerGuid)
    {
        foreach(var entity in entityDictionary.Values)
        {
            if(entity.OwnerGuid == ownerGuid)
            {
                yield return entity;
            }
        }
    }

    public IEnumerator LoadAsyncPlayer(ENUM_ENTITY_TYPE characterType)
    {
        var handle = ResourceManager.Instance.LoadAsync<EntityMeditatorComponent>();
        while (!handle.IsDone || handle.Status != AsyncOperationStatus.Succeeded)
        {
            yield return null;
        }

        var prefab = handle.Result as GameObject;
        if (prefab == null)
            yield break;

        var obj = UnityEngine.Object.Instantiate(prefab);
        if (obj == null)
            yield break;

        var character = obj.GetComponent<EntityMeditatorComponent>();
        if (character == null)
            yield break;

		character.Initialize(PlayerGuid, characterType);
		character.SetEntityLayer(ENUM_LAYER_TYPE.Friendly);

        PlayerEntityGuid = character.Guid;

		mono.SetSingletonChild(this, character);
    }

    public IEnumerator LoadAsyncMonsters()
    {
        yield return null;
    }

    public IEnumerator LoadAsyncBoss()
    {
        yield return null;
    }

    public IEnumerator LoadAsyncPassiveObject()
    {
        yield return null;
    }

    public IEnumerator UnloadAsyncPlayer()
    {
        yield return null;
    }

    public IEnumerator UnloadAsyncMonsters()
    {
        yield return null;
    }

    public IEnumerator UnloadAsyncBoss()
    {
        yield return null;
    }

    public IEnumerator UnloadAsyncPassiveObject()
    {
        yield return null;
    }

	public override void OnPostUpdate(int deltaFrameCount, float deltaTime)
    {
        currentFramePlayerEntitySnapshot = MakeMyFrameEntityMessage();
		currentFrameEntitiesSnapshot = MakeFrameEntityMessages(PlayerGuid).ToArray();

		foreach (var receiver in entityCaptureReceivers)
        {
            receiver.OnCapture(currentFramePlayerEntitySnapshot, currentFrameEntitiesSnapshot);
		}

		foreach (var obj in entityDictionary.Values)
        {
            obj.OnPostUpdate();
        }
	}

    public override void OnLateUpdate(int deltaFrameCount, float deltaTime)
    {
		foreach (var obj in entityDictionary.Values)
        {
            obj.OnLateUpdate();
        }
    }

	private IEnumerable<FrameEntityMessage> MakeFrameEntityMessages(int ownerGuid)
	{
		foreach (var entity in GetMyEntities(ownerGuid))
		{
			yield return MakeFrameEntityMessage(entity);
		}
	}

	private FrameEntityMessage MakeMyFrameEntityMessage()
	{
		return MakeFrameEntityMessage(PlayerEntity);
	}

	private FrameEntityMessage MakeFrameEntityMessage(EntityComponent entity)
	{
		if (entity == null)
			return new FrameEntityMessage();

		var entityMessage = new FrameEntityMessage();

		entityMessage.entityGuid = entity.Guid;
		entityMessage.isGrounded = entity.IsGrounded;
		entityMessage.myEntityOffset = entity.Offset;
		entityMessage.myEntityVelocity = entity.Velocity;
		entityMessage.myEntityHitBox = entity.HitBox;
		entityMessage.myEntityPos = entity.Position;
		entityMessage.entityState = (int)entity.CurrentState;
        entityMessage.animationMessage = new FrameEntityAnimationMessage();
        entityMessage.animationMessage.keyFrame = entity.CurrentKeyFrame;
        entityMessage.animationMessage.normalizedTime = entity.CurrentNormalizedTime;

		return entityMessage;
	}

	private IEnumerable<EntityComponent> GetOverlapEntities(int entityGuid, Vector3 pos, Vector3 size, Vector3 offset)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(pos + offset, size, 0);
        if (colliders == null || colliders.Length == 0)
            yield break;

        var myEntity = GetEntityComponent(entityGuid);
        if (myEntity == null)
            yield break;
        
        foreach (var collider in colliders)
        {
            var entity = collider.GetComponent<EntityComponent>();
            if (entity == null)
                continue;

            if (entity.OwnerGuid == myEntity.OwnerGuid) // 내껀 제외
                continue;

            yield return entity;
        }
    }

    private IEnumerable<EntityComponent> GetOverlapEntities(FrameEntityMessage message)
    {
        return GetOverlapEntities(message.entityGuid, message.myEntityPos, message.myEntityHitBox, message.myEntityOffset);
    }

    public IEnumerable<int> GetOverlapEntitiyGuids(FrameEntityMessage message)
    {
        return GetOverlapEntities(message).Select(entity => entity.Guid);
    }

	public int Register(EntityComponent objectComponent)
    {
        int Guid = objectComponent.GetInstanceID();
        entityDictionary[Guid] = objectComponent;
        return Guid;
	}

    public int UnRegister(int Guid)
    {
        if(entityDictionary.ContainsKey(Guid))
            entityDictionary.Remove(Guid);

        return -1;
    }

    public void Register(IEntityCaptureReceiver receiver)
    {
        if(entityCaptureReceivers.Contains(receiver) == false)
		    entityCaptureReceivers.Add(receiver);
	}

	public void UnRegister(IEntityCaptureReceiver receiver)
	{
		if (entityCaptureReceivers.Contains(receiver))
			entityCaptureReceivers.Remove(receiver);
	}
}
