using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemController : MonoBehaviour
{
    void Awake()
    {
        // Check if an Event System already exists in the scene
        EventSystem existingEventSystem = FindObjectOfType<EventSystem>();

        if (existingEventSystem == null)
        {
            // If no Event System exists, create one
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();

            Debug.Log("Event System created dynamically.");
        }
        else
        {
            Debug.Log("Event System already exists in the scene.");
        }
    }
}
