using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class GameManagerAgent : Agent, IGameManager
{
    private const float addMultiplier = 1f;
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;

    public int ghostMultiplier { get; private set; } = 1;
    public int score { get; private set; }
    public int lives { get; private set; }

    //private float rewardMultiplier = 1f;
    

    public override void Initialize()
    {
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(score);
        sensor.AddObservation(lives);
    }

    private void NewGame()
    {
        //rewardMultiplier = 1f;
        SetScore(0);
        SetLives(1);
        NewRound();
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        switch (actionBuffers.DiscreteActions[0])
        {
            case 1: pacman.movement.SetDirection(Vector2.up);
                break;
            case 2: pacman.movement.SetDirection(Vector2.down);
                break;
            case 3: pacman.movement.SetDirection(Vector2.left);
                break;
            case 4: pacman.movement.SetDirection(Vector2.right);
                break;
        }
        
        // var actionX = 2f * Mathf.Clamp(actionBuffers.ContinuousActions[0], -1f, 1f);
        // var actionY = 2f * Mathf.Clamp(actionBuffers.ContinuousActions[1], -1f, 1f);
        //
        // pacman.movement.SetDirection(new Vector2(actionX,actionY));
    }

    public override void OnEpisodeBegin()
    {
        NewGame();
    }

    // private void Update()
    // {
    //     if (this.lives <= 0 && Input.anyKeyDown)
    //     {
    //         NewGame();
    //     }
    // }

    private void NewRound()
    {
        foreach (Transform pellet in this.pellets)
        {
            pellet.gameObject.SetActive(true);
        }

        ResetState();
    }

    private void ResetState()
    {
        ResetGhostMultiplier();

        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].ResetState();
        }

        this.pacman.ResetState();
    }

    private void GameOver()
    {
        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].gameObject.SetActive(false);
        }

        this.pacman.gameObject.SetActive(false);
    }

    private void SetScore(int score)
    {
        this.score = score;
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
    }

    public void GhostEaten(Ghost ghost)
    {
        int points = ghost.points * this.ghostMultiplier;

        SetScore(this.score + points);
        //SetReward(points/10f);
        this.ghostMultiplier++;
    }

    public void PacmanEaten()
    {
        this.pacman.gameObject.SetActive(false);

        SetLives(this.lives - 1);
        SetReward(-500);

        if(this.lives > 0)
        {
            ResetState();
            //Invoke(nameof(ResetState), 3.0f);
        }
        else
        {
            // GameOver();
            EndEpisode();
        }
    }

    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);

        SetScore(this.score + pellet.points);

        //SetReward(pellet.points * rewardMultiplier / 10f);
        SetReward(pellet.points);

        //rewardMultiplier += addMultiplier;

        if(!HasRemainingPellets())
        {
            this.pacman.gameObject.SetActive(false);
            //Invoke(nameof(NewRound), 3.0f);
            NewRound();
        }
    }

    public void PowerPelletEaten(PowerPellet pellet)
    {
        for (var i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].frightened.Enable(pellet.duration);
        }

        PelletEaten(pellet);
        CancelInvoke();
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);
    }

    private bool HasRemainingPellets()
    {
        foreach (Transform pellet in this.pellets)
        {
            if (pellet.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    private void ResetGhostMultiplier()
    {
        this.ghostMultiplier = 1;
    }
}
