using Photon.Pun;

using Umbrace.Unity.PurePool;

using UnityEngine;

namespace Umbrace.Unity.PurePool.Photon {

	public class PunPoolingSetup : MonoBehaviour {

		[SerializeField, HideInInspector]
		private PrefabPool punPrefabPool;
		
		private void Start() {
			// Create the PrefabPool object, which is the bridge between PUN and Pure Pool.
			this.punPrefabPool = new PrefabPool();

			// Set the manager to load by name from the Resources folders.
			// This is required as most objects spawned by PUN will not be set up in the manager already, and will need to be loaded.
			NamedGameObjectPoolManager.Instance.UseResources = true;

			// Assign the manager, so it knows how to acquire and release from the pools.
			this.punPrefabPool.Manager = NamedGameObjectPoolManager.Instance;

			// Set PUN to use the PrefabPool object.
			PhotonNetwork.PrefabPool = punPrefabPool;
		}
		
		private void OnEnable() {
			// Set PUN to use the PrefabPool object again.
			// Using OnEnable rather than ISerializationCallbackReceiver as the static initialiser for PhotonNetwork performs some actions that can't occur during deserialisation callbacks.
			PhotonNetwork.PrefabPool = this.punPrefabPool;
		}
		
	}

}