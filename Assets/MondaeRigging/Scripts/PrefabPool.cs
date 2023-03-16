using System;
using System.Collections.Generic;

using Photon.Pun;

using Umbrace.Unity.PurePool;

using UnityEngine;

namespace Umbrace.Unity.PurePool.Photon {
	
	using PoolManagerBase = PoolManagerBase<GameObjectPoolManagerSettings,GameObjectPool,GameObjectPoolSettings,GameObject,GameObject>;

	/// <summary>
	/// A bridge class to allow object pooling to be used by Photon Unity Networking 2 (PUN 2).
	/// </summary>
	/// <remarks>
	/// <para>
	/// To use this class for pooling with Photon, instantiate a new instance of it, set the <see cref="Manager"/> property, and assign the instance to <c>PhotonNetwork.PrefabPool</c>.
	/// </para>
	/// <para>
	/// Please note that pooled GameObjects don't get the usual Awake and Start calls. OnEnable will be called (by your pool) but the
	/// networking values are not updated yet when that happens. OnEnable will have outdated values for PhotonView (isMine, etc.).
	/// You might have to adjust scripts.
	/// </para>
	/// <para>
	/// PUN will call OnPhotonInstantiate (see IPunInstantiateMagicCallback). This should be used to setup the re-used object with regards to networking values / ownership.
	/// </para>
	/// </remarks>
	[Serializable]
	public class PrefabPool : IPunPrefabPool {

		// TODO: Move the resourceCache into NamedGameObjectPoolManager.

		[SerializeField, HideInInspector]
		private NamedGameObjectPoolManager manager;
		private readonly Dictionary<string, GameObject> resourceCache = new Dictionary<string, GameObject>();
		
		/// <summary>
		/// Gets a dictionary containing mappings from string names to their GameObject prefabs, used as a cache to prevent unnecessary <see cref="Resource.Load"/> calls.
		/// </summary>
		public Dictionary<string, GameObject> ResourceCache {
			get { return this.resourceCache; }
		}

		/// <summary>
		/// Gets or sets the pool manager to be used by Photon.
		/// </summary>
		/// <remarks>
		/// The <see cref="NamedGameObjectPoolManager"/> assigned to this property must meet the following requirements:
		/// <list type="bullet">
		/// <item><description><see cref="NamedGameObjectPoolManager.UseResources"/> must be set to true.</description></item>
		/// <item><description>The <see cref="PoolManagerBase.AcquireMode"/> property of <see cref="NamedGameObjectPoolManager.Manager"/> must be set to <see cref="AcquireNoPoolMode.Instantiate"/> or <see cref="AcquireNoPoolMode.CreatePool"/>.</description></item>
		/// <item><description>In the case of <see cref="AcquireNoPoolMode.CreatePool"/>, the <see cref="SharedPoolSettings{TSource}.InstantiateWhenEmpty"/> property of <see cref="PoolManagerBase.DefaultPoolSettings"/> must be set to true.</description></item>
		/// </list>
		/// </remarks>
		public NamedGameObjectPoolManager Manager {
			get { return this.manager; }
			set {
				if (value == null) throw new ArgumentNullException();

				// Ensure Resources.Load is allowed. Unlikely all prefab names will have been set up in the manager.
				if (!value.UseResources) throw new ArgumentException("The manager must be set to load from Resources to use it with Photon. Set UseResources to true.");

				// Ensure an instance can always be acquired, even when no pool exists.
				if (value.Manager.AcquireMode == AcquireNoPoolMode.Error) throw new ArgumentException("The manager must be set to an AcquireMode that always allows instantiation. Set AcquireMode to CreatePool or Instantiate.");
				if (value.Manager.AcquireMode == AcquireNoPoolMode.CreatePool && !value.Manager.DefaultPoolSettings.InstantiateWhenEmpty) {
					throw new ArgumentException("The manager must be set to always allow instantiation." +
												"When using an AcquireMode of CreatePool, set DefaultPoolSettings.InstantiateWhenEmpty to true.");
				}

				this.manager = value;
			}
		}

		/// <inheritdoc />
		public void Destroy(GameObject gameObject) {
			this.manager.Release(gameObject);
		}

		/// <inheritdoc />
		public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation) {
			GameObject instance;

			// Check the resourceCache for the prefab, to avoid calling Resources.Load unnecessarily.
			GameObject prefab;
			if (this.resourceCache.TryGetValue(prefabId, out prefab)) {
				if (this.manager.Manager.TryAcquireDisabled(prefab, position, rotation, out instance)) {
					return instance;
				}

				// This should never occur, as an instance should always be available.
				DebugHelper.LogError("Failed to instantiate the prefab \"" + prefabId + "\". The internal resource cache knows the prefab, but the manager failed to acquire an instance of it.");
				return null;
			}
			
			// Try to acquire by a pre-defined name.
			if (this.manager.HasName(prefabId)) {
				if (this.manager.TryAcquireDisabled(prefabId, position, rotation, out instance)) {
					return instance;
				}
				
				// This should never occur, as an instance should always be available.
				DebugHelper.LogError("Failed to instantiate the prefab \"" + prefabId + "\". The manager has a GameObject by this name, but the manager failed to acquire an instance of it.");
				return null;
			}
			
			// Fallback to using Resources.Load, as there is no pre-defined name registered.
			prefab = Resources.Load<GameObject>(prefabId);
			if (prefab != null && this.manager.Manager.TryAcquireDisabled(prefab, position, rotation, out instance)) {
				this.resourceCache[prefabId] = prefab;
				return instance;
			}

			// No instance could be acquired. This should only occur if the prefab ID is not valid (no resource file with that name).
			DebugHelper.LogWarning("Failed to instantiate the prefab \"" + prefabId + "\". No GameObject resource with this name was found.");
			return null;
		}

	}

}