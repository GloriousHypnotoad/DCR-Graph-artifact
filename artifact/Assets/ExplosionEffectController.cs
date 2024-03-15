using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ExplosionEffectController : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private float initialGravityModifier;
    private float initialStartSpeed;

    void Start()
    {
        // Get the Particle System component
        particleSystem = GetComponent<ParticleSystem>();

        // Store initial values
        initialGravityModifier = particleSystem.main.gravityModifier.constant;
        initialStartSpeed = particleSystem.main.startSpeed.constant;

        // Start the particle system
        particleSystem.Play();
    }

    void Update()
    {
        // Get the Particle System.MainModule
        var mainModule = particleSystem.main;

        // Calculate the lifetime threshold for changing the behavior
        float velocityThreshold = mainModule.startLifetime.constant * 0.1f;

        // Check each particle
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
        int numParticlesAlive = particleSystem.GetParticles(particles);

        // Modify particles that have passed the velocity threshold of their lifetime
        for (int i = 0; i < numParticlesAlive; i++)
        {
            if (particles[i].remainingLifetime < velocityThreshold)
            {
                // Reduce the speed to a quarter
                particles[i].velocity *= 0.25f;
                
                // Increase the gravity to make particles float down
                //particles[i].velocity += Vector3.down * initialGravityModifier * 2f; // Double the gravity effect
            }
        }

        // Apply the particle changes back to the system
        particleSystem.SetParticles(particles, numParticlesAlive);
    }
}
