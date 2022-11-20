using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShellanderGames.WeaponWheel
{
	/// <summary>
	/// Parses inputs and caches previous frame inputs.
	/// </summary>
	public class SgInputParser
	{
		private InputActionMap m_InputActionMap;
		private readonly Dictionary<string, SgInputActionI> m_InputActionByName = new Dictionary<string, SgInputActionI>();

		public SgInputParser(InputActionMap inputActionMap)
		{
			SetInputActionMap(inputActionMap);
		}

		/// <summary>
		/// Change input action map. Clears input cache.
		/// </summary>
		/// <param name="inputActionMap">action map to change to</param>
		public void SetInputActionMap(InputActionMap inputActionMap) {
			m_InputActionMap = inputActionMap;
			m_InputActionByName.Clear();
		}

		/// <summary>
		/// Create a float action (e.g. button) and cache inputs if an action with the same name hasn't been added before.
		/// </summary>
		/// <param name="actionName"></param>
		/// <returns></returns>
		public SgFloatInputAction CreateFloatAction(string actionName)
		{
			SgFloatInputAction action = new SgFloatInputAction(actionName, FindInputAction(actionName));
			action = (SgFloatInputAction)TryAddCachedAction(actionName, action);
			return action;
		}
		/// <summary>
		/// Create a float action (e.g. a pointer or stick) and cache inputs if an action with the same name hasn't been added before.
		/// </summary>
		/// <param name="actionName"></param>
		/// <returns></returns>
		public SgVector2InputAction CreateVector2Action(string actionName)
		{
			SgVector2InputAction action = new SgVector2InputAction(actionName, FindInputAction(actionName));
			action = (SgVector2InputAction)TryAddCachedAction(actionName, action);
			return action;
		}

		/// <summary>
		/// Get input action by name
		/// </summary>
		/// <param name="actionName"></param>
		/// <returns></returns>
		public SgInputActionI Get(string name)
		{
			return m_InputActionByName[name];
		}

		private SgInputActionI TryAddCachedAction(string name, SgInputActionI sgInputAction)
		{
			if (!m_InputActionByName.ContainsKey(name))
			{
				m_InputActionByName[name] = sgInputAction;
			}
			return m_InputActionByName[name];
		}

		private InputAction FindInputAction(string name)
		{
			return m_InputActionMap.FindAction(name, false);
		}

		public void UpdateCache()
		{
			foreach (SgInputActionI cachedAction in m_InputActionByName.Values)
			{
				cachedAction.UpdateCache();
			}
		}

		/// <summary>
		/// Generic input action with helper methods.
		/// </summary>
		public interface SgInputActionI
		{
			public void UpdateCache();
			/// <summary>
			/// Action is pressed.
			/// </summary>
			/// <returns></returns>
			public abstract bool IsPressed();
			/// <summary>
			/// Action is pressed now and wasn't before.
			/// </summary>
			/// <returns></returns>
			public abstract bool WasPressedNow();
			/// <summary>
			/// Action is not pressed and was before.
			/// </summary>
			/// <returns></returns>
			public abstract bool WasReleasedNow();
		}
		/// <summary>
		/// Generic input action implementation with helper methods.
		/// </summary>
		public abstract class SgInputActionWrapper<T> : SgInputActionI where T : struct
		{
			public InputAction inputAction;
			protected T currentValue;
			protected T previousValue;
			public bool doIgnore;

			public SgInputActionWrapper(string name, InputAction inputAction)
			{
				this.inputAction = inputAction;
				if (inputAction == null)
				{
					doIgnore = true;
					Debug.Log("Ignoring input action: " + name);
				}
				else
				{
					doIgnore = false;
				}
			}

			public T CurrentValue => currentValue;

			public abstract bool IsPressed();

			public void UpdateCache()
			{
				if (doIgnore)
				{
					return;
				}
				previousValue = currentValue;
				currentValue = inputAction.ReadValue<T>();
			}

			public abstract bool WasPressedNow();

			public abstract bool WasReleasedNow();
		}

		/// <summary>
		/// Float input action.
		/// </summary>
		public class SgFloatInputAction : SgInputActionWrapper<float>
		{
			public SgFloatInputAction(string name, InputAction inputAction) : base(name, inputAction)
			{
			}

			public override bool IsPressed()
			{
				return currentValue != 0;
			}

			public override bool WasPressedNow()
			{
				return currentValue != 0 && previousValue == 0;
			}

			public override bool WasReleasedNow()
			{
				return currentValue == 0 && previousValue != 0;
			}
		}

		/// <summary>
		/// Vector2 input action.
		/// </summary>
		public class SgVector2InputAction : SgInputActionWrapper<Vector2>
		{
			public SgVector2InputAction(string name, InputAction inputAction) : base(name, inputAction) { }

			public override bool IsPressed()
			{
				return currentValue != Vector2.zero;
			}
			/// <summary>
			/// In this case, it means that the stick has a value now and didn't before
			/// </summary>
			/// <returns></returns>
			public override bool WasPressedNow()
			{
				return currentValue != Vector2.zero && previousValue == Vector2.zero;
			}
			/// <summary>
			/// In this case, it means that the stick hasn't a value now and did before
			/// </summary>
			/// <returns></returns>
			public override bool WasReleasedNow()
			{
				return currentValue == Vector2.zero && previousValue != Vector2.zero;
			}
		}

	}

}