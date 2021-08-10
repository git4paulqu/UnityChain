using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChain
{
	public partial class Chain : MonoBehaviour
	{
		private void Start()
		{
			m_transformCache = transform;
			
			ChainError error = null == m_transformCache ? ChainError.TransformNull : ChainError.Default;
			SetError(error);
			
			PrepareParticles();
			
			PrepareColliders();
			
			PrepareState();
		}

		private void Update()
		{
			if (!IsWork4Update())
			{
				return;
			}
			
			float time = Time.deltaTime;
			WriteUpdateData(time);

			if (!IsNeedUpdate())
			{
				return;
			}
			
			UpdateParticlesRender();
		}

		private void FixedUpdate()
		{
			if (!IsWork4Update())
			{
				return;
			}
			
			float time = Time.smoothDeltaTime;
			WriteLateUpdateData(time);

			if (!IsNeedUpdate())
			{
				return;
			}

			UpdateChain(time);
		}

		private void OnValidate()
		{
			particles = GetComponentsInChildren<UnityChain.Particle>();

			Start();
		}

		private void PrepareParticles()
		{
			int count = ParticleCount;
			if (count <= 0)
			{
				return;
			}

			count--;
			for (int i = 0; i < count; i++)
			{
				UnityChain.Particle p1 = particles[i];
				UnityChain.Particle p2 = particles[i + 1];
				
				bool p1Null = CheckParticleNull(p1, i);
				bool p2Null = CheckParticleNull(p2, i + 1);

				if (p1Null || p2Null)
				{
					continue;
				}
				
				p1.Prepare(i);
				p2.Prepare(i+1);

				float distance = (p1.Position - p2.Position).magnitude;
				p2.SetLinkDistance(distance);
			}
		}

		private bool CheckParticleNull(Particle paticle, int index)
		{
			if (null != paticle)
			{
				return false;
			}

			name = null == m_transformCache ? "NULL" : m_transformCache.name;
			UnityChainLogger.LogError("particle: name:{0}, index:{1} can not be null.", name, index);
			SetError(ChainError.ParticleNull);

			return true;
		}

		private void PrepareColliders()
		{
			bounds = GetComponent<ChainCollider>();
			if (null != bounds)
			{
				bounds.Reset();
			}

			particleColliders.Clear();
			
			int count = ParticleCount;
			for (int i = 0; i < count; i++)
			{
				UnityChain.Particle p = particles[i];
				if (null == p || null == p.particleCollider)
				{
					continue;
				}
				particleColliders.Add(p.particleCollider);
			}
		}

		private void UpdateParticlesRender()
		{
			int count = ParticleCount;
			for (int i = 0; i < count; i++)
			{
				UnityChain.Particle particle = particles[i];
				particle.UpdateRender();
			}
		}

		private void UpdateChain(float deltaTime)
		{
			PrepareState();
			
			Simulate(deltaTime);
			
			for (int i = 0; i < iterations; i++)
			{
				ApplyConstraint(i);

				AdjustCollisions(i);
			}
			
			SetAngles();
		}

		private void PrepareState()
		{
			Vector3 position = m_transformCache.position;
			m_movement = position - m_prePosition;
			m_prePosition = position;
		}

		private void Simulate(float deltaTime)
		{
			int count = ParticleCount;
			if (count <= 0)
			{
				return;
			}

			for (int i = 0; i < count; i++)
			{
				Particle particle = particles[i];
				Vector3 position = particle.Position;
				
				if (i == 0)
				{
					particle.SetPrePosition(position);
					particle.SetPosition(m_transformCache.position, EParticlePositionChangedEvent.SimulateHead);
					continue;
				}

				Vector3 rmove = m_movement * particle.inert;
				Vector3 velocity = position - particle.PrePosition;
				//FixedVelocity(ref velocity);

				velocity += particle.Force;
				particle.SetForce(Vector3.zero);
                
				velocity *= particle.velocityDampen;

				if (particle.IsCollide)
				{
					velocity = Vector3.zero;
				}

				Vector3 prePosition = position + rmove;
				particle.SetPrePosition(prePosition);
                
				// calculate new position
				Vector3 newPosition = position + rmove + velocity;
				newPosition += particle.gravity * deltaTime;

				particle.SetPosition(newPosition, EParticlePositionChangedEvent.Simulate);
			}
		}
		
		private void FixedVelocity(ref Vector3 velocity)
		{
			velocity.x += velocity.y;
			velocity.y = 0;

			float x = clampVelocity.x;
			//float y = clampVelocity.y;
			float z = clampVelocity.z;

			velocity.x = Mathf.Clamp(velocity.x, -x, x);
			//velocity.y = Mathf.Clamp(velocity.y, -y, y);
			velocity.z = Mathf.Clamp(velocity.z, -z, z);
		}
		
		private void ApplyConstraint(int iteration = 0)
		{
			int count = ParticleCount;
			if (count <= 0)
			{
				return;
			}

			count--;
			for (int i = 0; i < count; i++)
			{
				UnityChain.Particle p1 = particles[i];
				UnityChain.Particle p2 = particles[i + 1];

				Vector3 v1 = p1.Position;
				Vector3 v2 = p2.Position;

				float linkDistance = p2.LinkDistance;

				// Get the current distance between rope nodes
				float currentDistance = (v1 - v2).magnitude;
				float difference = Mathf.Abs(currentDistance - linkDistance);
				Vector3 direction = Vector3.zero;

				// determine what direction we need to adjust our nodes
				if (currentDistance > linkDistance)
				{
					direction = (v1 - v2).normalized;
				}
				else if (currentDistance < linkDistance)
				{
					direction = (v2 - v1).normalized;
				}

				// calculate the movement vector
				Vector3 movement = direction * difference;

				// apply correction
				//v1 -= (movement * p1.stiffness);
				v2 += (movement * p2.stiffness);

				//p1.SetPosition(v1);
				p2.SetPosition(v2, EParticlePositionChangedEvent.Constraint);
			}
		}
		
		private void AdjustCollisions(int iteration = 0)
		{
			if (null == bounds || null == particleColliders)
			{
				return;
			}

			int count = particleColliders.Count;
			for (int i = 0; i < count; i++)
			{
				ChainCollider collider = particleColliders[i];
				if (null == collider)
				{
					continue;
				}

				UnityChain.Particle particle = collider.Owner;
				if (null == particle)
				{
					continue;
				}

				particle.SetIsCollide(false);

				Vector3 penetration = bounds.ComputePenetration(collider);
				if (penetration == Vector3.zero)
				{
					continue;
				}

				Vector3 position = particle.Position;
				position += penetration;
				particle.SetPosition(position, EParticlePositionChangedEvent.Collision);
				particle.SetIsCollide(true);
			}
		}
		
		private void SetAngles()
		{
			int count = ParticleCount;
			if (count <= 0)
			{
				return;
			}

			Vector3 right = m_transformCache.right;
			
			count--;
			int p1Max = count - 1;
			for (int i = 0; i < count; i++)
			{
				UnityChain.Particle p1 = particles[i];
				UnityChain.Particle p2 = particles[i + 1];

				Vector3 up = (p1.Position - p2.Position).normalized;
				Vector3 forwad = Vector3.Cross(right, up);
				Quaternion quaternion = Quaternion.LookRotation(forwad, up);
				p1.SetRotation(quaternion);
				
				if (i == p1Max)
				{
					p2.SetRotation(p1.Rotation);	
				}
			}
		}
		
		private void SetError(ChainError error)
		{
			m_error = error;
		}

		public Particle GetParticle(int index)
		{
			int totalCount = ParticleCount;
			if (index < 0 || index >= totalCount)
			{
				return null;
			}

			return particles[index];
		}

		private bool IsNeedUpdate()
		{
			if (m_error != ChainError.Default)
			{
				return false;
			}

			if (null == m_transformCache)
			{
				SetError(ChainError.TransformNull);
				return false;
			}
			
			return true;
		}
		
		private bool IsWork4Update()
		{
			return !UnityChainRuntimeDefine.EnablFrameDebuggerReplay;
		}
		
		public int ParticleCount
		{
			get {
				if (null == particles)
				{
					return 0;
				}

				return particles.Length;
			}
		}

		public ChainCollider bounds;
		public UnityChain.Particle[] particles;
		public List<ChainCollider> particleColliders = new List<ChainCollider>();
		
		private int iterations = 1;
		private ChainError m_error;
		private Transform m_transformCache;

		public Vector3 m_prePosition;
		public Vector3 m_movement;
		public Vector3 clampVelocity = new Vector3(0.0001f, 0.0001f, 0.0001f);
	}
}