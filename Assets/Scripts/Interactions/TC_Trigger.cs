using UnityEngine;

public class TC_Trigger : TriggerCore {
  void OnTriggerEnter(Collider other) {
    AddContact(other.gameObject);
  }

  void OnTriggerExit(Collider other) {
    RemoveContact(other.gameObject);
  }
}
