using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickUpItem : MonoBehaviour
{
    [SerializeField] private bool destroyOnPickUp = true;
    [SerializeField] private LayerMask canBePickUpBy;
    [SerializeField] private AudioClip pickUpSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(canBePickUpBy.value == (canBePickUpBy.value | (1 << other.gameObject.layer)))
        {
            OnPickedUp(other.gameObject);
            if (pickUpSound)
                SoundManager.PlayClip(pickUpSound);

            if (destroyOnPickUp)
            {
                Destroy(gameObject);
            }
        }
    }

    protected abstract void OnPickedUp(GameObject receiver);
}
