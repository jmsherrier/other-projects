// Parasite — representative gameplay snippet (Unity, C#).
// Illustrates the event-driven, low-coupling approach used across the project:
// systems broadcast and listen through a ScriptableObject event channel
// instead of holding direct references to one another.
using System;
using UnityEngine;

namespace Parasite.Core
{
    /// <summary>
    /// A ScriptableObject event channel. Combat, questing, and world systems
    /// raise and subscribe to these without knowing about each other, keeping
    /// dependencies one-directional and each system independently testable.
    /// </summary>
    [CreateAssetMenu(menuName = "Parasite/Events/Game Event Channel")]
    public class GameEventChannel : ScriptableObject
    {
        public event Action<GameEvent> Raised;

        public void Raise(GameEvent payload) => Raised?.Invoke(payload);
    }

    public readonly struct GameEvent
    {
        public readonly string Id;
        public readonly object Data;

        public GameEvent(string id, object data = null)
        {
            Id = id;
            Data = data;
        }
    }

    /// <summary>
    /// Example listener: a quest tracker reacts to world events without ever
    /// referencing the systems that produce them.
    /// </summary>
    public class QuestTracker : MonoBehaviour
    {
        [SerializeField] private GameEventChannel worldEvents;

        private void OnEnable() => worldEvents.Raised += OnWorldEvent;
        private void OnDisable() => worldEvents.Raised -= OnWorldEvent;

        private void OnWorldEvent(GameEvent e)
        {
            if (e.Id == "objective.completed")
            {
                Debug.Log($"Objective advanced: {e.Data}");
            }
        }
    }
}
