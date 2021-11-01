using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{

    public static ParticleManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    [SerializeField] GameObject deathParticle;
    [SerializeField] ParticleSystem sprintParticle;
    [SerializeField] ParticleSystem dashParticle;
    [SerializeField] GameObject collisionParticle;

    public void startDeathParticle()
    {
        Instantiate(deathParticle, transform.position, Quaternion.identity);
    }

    public void startGroundCollisionParticle()
    {
        Instantiate(collisionParticle, transform.position + collisionParticle.transform.position, collisionParticle.transform.rotation);
    }

    public void startLeftCollisionParticle()
    {
        Instantiate(collisionParticle, transform.position + new Vector3(-0.5f, -0.2f, 0f), Quaternion.Euler(0, 90f, -90f));
    }
    
    public void startRightCollisionParticle()
    {
        Instantiate(collisionParticle, transform.position + new Vector3(0.5f,-0.2f,0f), Quaternion.Euler(0, -90f, 90f));
    }
    

    public void startSprintParticle()
    {
        if (!sprintParticle.isEmitting)
        {
            sprintParticle.Play();
        }
    }

    public void startDashParticle()
    {
        if (!dashParticle.isEmitting)
        {
            dashParticle.Play();
        }
    }
}
