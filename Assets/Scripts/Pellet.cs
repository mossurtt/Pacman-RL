using System;
using UnityEngine;
using System.Linq;

public class Pellet : MonoBehaviour
{
    [HideInInspector] public IGameManager gameManager;
    public int points = 10;

    private void Start()
    {
        gameManager = GetComponentInParent<IGameManager>();
    }

    protected virtual void Eat()
    {
        gameManager.PelletEaten(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            Eat();
        }
    }

}
