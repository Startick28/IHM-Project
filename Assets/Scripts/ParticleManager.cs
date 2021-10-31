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

    public void startCollisionParticle()
    {
        Instantiate(collisionParticle, transform.position + collisionParticle.transform.position, collisionParticle.transform.rotation);
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
