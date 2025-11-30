using UnityEngine;
using System.Collections.Generic;

public class TriggerCore : MonoBehaviour {
  public List<GameObject> contacts;
  public int contactCount;
  public bool breached { get { return contactCount > 0; } }

  void Start() {
    contacts = new List<GameObject>(16);
  }

  virtual public void FixedUpdate() {
    for (int i = contacts.Count - 1; i >= 0; i--) {
      if (contacts[i] == null || !contacts[i].activeSelf) {
        contacts.RemoveAt(i);
        contactCount--;
      }
    }
  }

  protected void AddContact(GameObject alpha) {
    if (!contacts.Contains(alpha)) {
      contacts.Add(alpha);
      contactCount++;
    }
  }


  protected void RemoveContact(GameObject alpha) {
    if (contacts.Contains(alpha)) {
      contacts.Remove(alpha);
      contactCount--;
    }
  }
}
