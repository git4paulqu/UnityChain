using System.Collections.Generic;
using UnityChain.FrameDebugger;
using UnityEngine;

namespace UnityChain
{
    public partial class Chain
    {
        public void ReadData(UpdateSynchronousData data)
        {
            if (!IsNeedUpdate())
            {
                return;
            }
			
            UpdateParticlesRender();
        }
        
        public void ReadData(FixedUpdateSynchronousData synchronousData,
                             System.Action simulateCallback = null,
                             System.Action constraintCallback = null,
                             System.Action collisionsCallback = null,
                             System.Action angelesCallback = null)
        {
            ApplyFixedUpdateFameData(synchronousData);
            
            float deltaTime = synchronousData.deltaTime;
            
            if (!IsNeedUpdate())
            {
                return;
            }
			
            Simulate(deltaTime);
            InvokeAction(simulateCallback);

            for (int i = 0; i < iterations; i++)
            {
                ApplyConstraint(i);
                InvokeAction(constraintCallback);

                AdjustCollisions(i);
                InvokeAction(collisionsCallback);
            }
			
            SetAngles();
            InvokeAction(angelesCallback);
        }
        
        public void ReadData(ChainSnapshotData chainSnapshotData)
        {
            if (null == chainSnapshotData)
            {
                return;
            }

            ApplyChainSnapshotData(chainSnapshotData);
        }

        public static UnityChain.Chain Instantiate(ChainSnapshotData chainSnapshotData)
        {
            if (null == chainSnapshotData)
            {
                return null;
            }
            
            return InstantiateChain(chainSnapshotData);
        }

        public ChainSnapshotData CaptureChainSnapshotData()
        {
            if (null == m_transformCache)
            {
                return null;
            }
           
            ChainSnapshotData chainSnapshotData = new ChainSnapshotData();
            chainSnapshotData.name = m_transformCache.name;
            chainSnapshotData.position = m_transformCache.position;
            chainSnapshotData.scalse = m_transformCache.localScale;
            chainSnapshotData.rotation = m_transformCache.rotation;
            
            chainSnapshotData.error = m_error;
            chainSnapshotData.bounds = CaptureChainColliderSnapshotData(bounds);
            chainSnapshotData.particles = CaptureParticleSnapshotDataCollection();

            PostProcessRuntimeData(chainSnapshotData);
            
            return chainSnapshotData;
        }

        private void ApplyFixedUpdateFameData(FixedUpdateSynchronousData synchronousData)
        {
            if (null == m_transformCache)
            {
                return;
            }

            ChainSynchronousData chainSynchronousData = synchronousData.chainData;
            m_transformCache.position = chainSynchronousData.chainPosition;
            m_transformCache.rotation = chainSynchronousData.chainRotation;
            m_transformCache.localScale = chainSynchronousData.chainScale;

            List<ParticleSynchronousData> particleRuntimeDatas = synchronousData.particleDatas;
            if (null == particleRuntimeDatas)
            {
                return;
            }

            foreach (var particleRuntimeData in particleRuntimeDatas)
            {
                int index = particleRuntimeData.index;
                if (index >= ParticleCount)
                {
                    continue;
                }

                UnityChain.Particle particle = particles[index];
                particle.SetForce(particleRuntimeData.force);
            }
        }
        
        private void WriteUpdateData(float time)
        {
            if (!IsEnableWriteData())
            {
                return;
            }
            
            UpdateSynchronousData synchronousData = new UpdateSynchronousData();
            synchronousData.deltaTime = time;
            PostProcessRuntimeData(synchronousData);
            
            UnityChainFrameDebuggerClientProxy.SynchronousUpdateData(synchronousData);
        }

        private void WriteLateUpdateData(float time)
        {
            if (!IsEnableWriteData())
            {
                return;
            }
            
            if (null == m_transformCache)
            {
                return;
            }
            
            FixedUpdateSynchronousData synchronousData = new FixedUpdateSynchronousData();
            synchronousData.deltaTime = time;
            PostProcessRuntimeData(synchronousData);

            WriteChainSynchronousData(synchronousData);
            WriteParticlesData(synchronousData);
            
            UnityChainFrameDebuggerClientProxy.SynchronousFixedUpdateData(synchronousData);
        }
        
        private void WriteChainSynchronousData(FixedUpdateSynchronousData synchronousData)
        {
            ChainSynchronousData chainSynchronousData = new ChainSynchronousData();
            synchronousData.chainData = chainSynchronousData;
            
            chainSynchronousData.chainError = m_error;
            chainSynchronousData.chainPosition = m_transformCache.position;
            chainSynchronousData.chainRotation = m_transformCache.rotation;
            chainSynchronousData.chainScale = m_transformCache.localScale;
        }

        private void WriteParticlesData(FixedUpdateSynchronousData synchronousData)
        {
            List<ParticleSynchronousData> particleSynchronousDatas = new List<ParticleSynchronousData>();
            synchronousData.particleDatas = particleSynchronousDatas;

            int count = ParticleCount;
            if (count < 1)
            {
                return;
            }

            for (int i = 0; i < count; i++)
            {
                UnityChain.Particle p = particles[i];
                if (null == p)
                {
                    continue;
                }
                
                ParticleSynchronousData particleSynchronousData = new ParticleSynchronousData();
                particleSynchronousData.index = p.Index;
                particleSynchronousData.force = p.Force;
            }
        }

        #region Snapshot

        private ParticleSnapshotData[] CaptureParticleSnapshotDataCollection()
        {
            int count = ParticleCount;
            if (count < 1)
            {
                return null;
            }

            ParticleSnapshotData[] collection = new ParticleSnapshotData[count];

            for (int i = 0; i < count; i++)
            {
                Particle particle = particles[i];
                collection[i] = CaptureParticleSnapshotData(particle);
            }
            
            return collection;
        }

        private ParticleSnapshotData CaptureParticleSnapshotData(UnityChain.Particle particle)
        {
            if (null == particle || null == particle.Owner)
            {
                return null;
            }
            
            ParticleSnapshotData particleSnapshotData = new ParticleSnapshotData();
            particleSnapshotData.name = particle.Owner.name;
            particleSnapshotData.transformPosition = particle.Owner.position;
            particleSnapshotData.transformRotation = particle.Owner.rotation;
            particleSnapshotData.transformScalse = particle.Owner.localScale;

            particleSnapshotData.velocityDampen = particle.velocityDampen;
            particleSnapshotData.stiffness = particle.stiffness;
            particleSnapshotData.gravity = particle.gravity;
            particleSnapshotData.Index = particle.Index;
            particleSnapshotData.LinkDistance = particle.LinkDistance;
            particleSnapshotData.PrePosition = particle.PrePosition;
            particleSnapshotData.Position = particle.Position;
            particleSnapshotData.Rotation = particle.Rotation;
            particleSnapshotData.Force = particle.Force;
            particleSnapshotData.IsCollide = particle.IsCollide;
            particleSnapshotData.particleCollider = CaptureChainColliderSnapshotData(particle.particleCollider);
            
            return particleSnapshotData;
        }

        private ChainColliderSnapshotData CaptureChainColliderSnapshotData(ChainCollider chainCollider)
        {
            if (null == chainCollider)
            {
                return null;
            }
            
            ChainColliderSnapshotData chainColliderSnapshotData = new ChainColliderSnapshotData();
            chainColliderSnapshotData.size = chainCollider.size;
            chainColliderSnapshotData.center = chainCollider.center;
            return chainColliderSnapshotData;
        }

        private static UnityChain.Chain InstantiateChain(ChainSnapshotData chainSnapshotData)
        {
            GameObject obj = new GameObject();
            obj.name = chainSnapshotData.name;
            obj.transform.position = chainSnapshotData.position;
            obj.transform.rotation = chainSnapshotData.rotation;
            obj.transform.localScale = chainSnapshotData.scalse;

            UnityChain.Chain chain = obj.AddComponent<UnityChain.Chain>();
           
            ParticleSnapshotData[] particleSnapshotDatas = chainSnapshotData.particles;
            if (null != particleSnapshotDatas)
            {
                foreach (var item in particleSnapshotDatas)
                {
                    InstantiateParticle(obj, item);
                }
            }
            
            chain.m_error = chainSnapshotData.error;
            chain.bounds = InstantiateChainCollider(obj, chainSnapshotData.bounds);
            chain.OnValidate();
            
            chain.ApplyChainSnapshotData(chainSnapshotData);

            return chain;
        }

        private static void InstantiateParticle(GameObject parent, ParticleSnapshotData particleSnapshotData)
        {
            if (null == particleSnapshotData)
            {
                return;
            }

            GameObject particleObject = new GameObject();
            particleObject.name = particleSnapshotData.name;
            particleObject.transform.SetParent(parent.transform);
            
            UnityChain.Particle particle = particleObject.AddComponent<UnityChain.Particle>();
            InstantiateChainCollider(particleObject, particleSnapshotData.particleCollider);
            
            particle.transform.position = particleSnapshotData.Position;
        }

        private static UnityChain.ChainCollider InstantiateChainCollider(GameObject obj, ChainColliderSnapshotData chainColliderSnapshotData)
        {
            if (null == chainColliderSnapshotData)
            {
                return null;
            }

            UnityChain.ChainCollider chainCollider = obj.GetComponent<UnityChain.ChainCollider>();
            if (null == chainCollider)
            {
                chainCollider = obj.AddComponent<UnityChain.ChainCollider>();
            }

            return chainCollider;
        }

        private void ApplyChainSnapshotData(ChainSnapshotData chainSnapshotData)
        {
            if (null == m_transformCache)
            {
                return;
            }

            m_transformCache.position = chainSnapshotData.position;
            m_transformCache.rotation = chainSnapshotData.rotation;
            m_transformCache.localScale = chainSnapshotData.scalse;

            m_error = chainSnapshotData.error;
            ApplyChainColliderSnapshotData(bounds, chainSnapshotData.bounds);

            ApplyParticlesSnapshotData(chainSnapshotData.particles);
        }

        private void ApplyParticlesSnapshotData(ParticleSnapshotData[] particleSnapshotDatas)
        {
            if (null == particleSnapshotDatas)
            {
                return;
            }

            int count = particleSnapshotDatas.Length;
            for (int i = 0; i < count; i++)
            {
                UnityChain.Particle particle = GetParticle(i);
                ApplyParticleSnapshotDatas(particle, particleSnapshotDatas[i], m_transformCache.localScale);
            }
        }

        private static void ApplyParticleSnapshotDatas(Particle particle, ParticleSnapshotData particleSnapshotData, Vector3 parentScale)
        {
            if (null == particle || null == particleSnapshotData)
            {
                return;
            }

            Vector3 localScale = Vector3.zero;
            localScale.x = particleSnapshotData.transformScalse.x / parentScale.x;
            localScale.y = particleSnapshotData.transformScalse.y / parentScale.y;
            localScale.z = particleSnapshotData.transformScalse.z / parentScale.z;
            
            particle.Owner.position = particleSnapshotData.transformPosition;
            particle.Owner.rotation = particleSnapshotData.transformRotation;
            particle.Owner.localScale = localScale;
            
            particle.velocityDampen = particleSnapshotData.velocityDampen;
            particle.stiffness = particleSnapshotData.stiffness;
            particle.gravity = particleSnapshotData.gravity;
            
            particle.SetPrePosition(particleSnapshotData.PrePosition);
            particle.SetPosition(particleSnapshotData.Position, EParticlePositionChangedEvent.FrameDebug);
            particle.SetRotation(particleSnapshotData.Rotation);
            particle.SetForce(particleSnapshotData.Force);
            particle.SetIsCollide(particleSnapshotData.IsCollide);

            ApplyChainColliderSnapshotData(particle.particleCollider, particleSnapshotData.particleCollider);
        }

        private static void ApplyChainColliderSnapshotData(ChainCollider collider, ChainColliderSnapshotData colliderSnapshotData)
        {
            if (null == collider || null == colliderSnapshotData)
            {
                return;
            }

            collider.size = colliderSnapshotData.size;
            collider.center = colliderSnapshotData.center;
        }

        #endregion // Snapshot

        private void PostProcessRuntimeData(UnityChain.FrameDebugger.RuntimeData data)
        {
            if (null == data)
            {
                return;
            }

            data.uid = GetInstanceID();
            data.frame = Time.frameCount;
        }

        private bool IsEnableWriteData()
        {
            return UnityChainRuntimeDefine.EnablFrameDebuggerCollectData;
        }

        private void InvokeAction(System.Action action)
        {
            if (null == action)
            {
                return;
            }
            
            action.Invoke();
        }
    }
}