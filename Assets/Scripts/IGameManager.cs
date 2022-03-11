using UnityEngine;

    public interface IGameManager
    {
        public void GhostEaten(Ghost ghost);
        public void PacmanEaten();
        public void PelletEaten(Pellet pellet);
        public void PowerPelletEaten(PowerPellet pellet);
    }
