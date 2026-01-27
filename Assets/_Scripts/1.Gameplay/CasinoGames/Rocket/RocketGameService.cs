using System;
using Module.Core;
using UnityEngine;
using Zenject;

namespace Module.Gameplay
{
    public sealed class RocketGameService : ITickable
    {
        private readonly Func<float> _random01;

        public RocketGameState State { get; }

        public event Action OnStateChanged;
        public event Action<float> OnMultiplierChanged;
        public event Action OnRoundStarted;
        public event Action<float> OnRoundEnded;
        public event Action<float> OnClaimedWinnings;

        public RocketGameService(Func<float> random01 = null)
        {
            State = new RocketGameState();
            _random01 = random01 ?? UnityRandom01;
        }

        public void SetBet(float bet)
        {
            if (State.Phase == ERocketRoundPhase.Running) return;

            State.CurrentBet = bet;
            OnStateChanged?.Invoke();
        }

        public void ConfirmBet(float currentBalance)
        {
            if (State.CurrentBet < RocketGameConfig.minBet) return;
            if (State.CurrentBet > currentBalance) return;

            State.BetConfirmed = true;
            State.Phase = ERocketRoundPhase.Idle;
            OnStateChanged?.Invoke();
        }

        public bool CanStartRound(float currentBalance)
        {
            if (State.Phase == ERocketRoundPhase.Running) return false;
            if (!State.BetConfirmed) return false;
            return State.CurrentBet >= RocketGameConfig.minBet && State.CurrentBet <= currentBalance;
        }

        public void StartRound()
        {
            State.Phase = ERocketRoundPhase.Running;
            State.HasClaimed = false;
            State.ElapsedTime = 0f;
            State.CurrentMultiplier = 1f;

            State.ExplodeTime = CalculateExplosionTime();
            OnRoundStarted?.Invoke();
            OnStateChanged?.Invoke();
        }

        public void Tick()
        {
            if (State.Phase != ERocketRoundPhase.Running) return;

            State.ElapsedTime += Time.deltaTime;

            UpdateMultiplier();

            if (State.ElapsedTime >= State.ExplodeTime)
            {
                EndRound();
            }
        }

        public bool CanClaim() => State.Phase == ERocketRoundPhase.Running && !State.HasClaimed;

        public float Claim()
        {
            if (!CanClaim()) return 0f;

            State.HasClaimed = true;

            float winnings = State.CurrentBet * State.CurrentMultiplier;
            OnClaimedWinnings?.Invoke(winnings);
            OnStateChanged?.Invoke();

            return winnings;
        }

        private void EndRound()
        {
            State.Phase = ERocketRoundPhase.Ended;
            State.BetConfirmed = false;

            State.LastMultiplier = State.CurrentMultiplier;
            OnRoundEnded?.Invoke(State.LastMultiplier);
            OnStateChanged?.Invoke();
        }

        private void UpdateMultiplier()
        {
            State.CurrentMultiplier = (float)Math.Exp(RocketGameConfig.alpha * State.ElapsedTime);
            OnMultiplierChanged?.Invoke(State.CurrentMultiplier);
        }

        private float CalculateExplosionTime()
        {
            float maxTime = (float)(Math.Log(RocketGameConfig.maxMultiplier + 1f) / Math.Log(Math.E)) / RocketGameConfig.alpha;

            float u = Clamp01(_random01());
            const float k = 2f;

            return 1f + ((maxTime - 1f) * (float)Math.Pow(u, k));
        }

        private static float Clamp01(float v)
        {
            if (v < 0f) return 0f;
            if (v > 1f) return 1f;
            return v;
        }

        private static float UnityRandom01() => UnityEngine.Random.value;
    }
}
