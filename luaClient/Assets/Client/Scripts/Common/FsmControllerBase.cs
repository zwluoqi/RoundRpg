using System;
using System.Collections.Generic;


    public interface IFsmController<T>
    {
        //FsmStateBase<T> currentState;
        IFsmState<T> GetCurrentState();

        void InitState();

        IFsmState<T> GetState(T stateType);

        void SwitchState(T stateType);

		void UpdateState();

    }

	public class FsmControllerBase<T> : IFsmController<T>
    {
        protected FsmStateBase<T> currentState;

		public IFsmState<T> GetCurrentState()
        {
            return currentState;
        }

        public virtual void InitState()
        {
            allStates = new Dictionary<T, FsmStateBase<T>>();
            //add state into the dictionary
            //allStates[T] = new FsmStateBase<T>(T,this);
        }

        protected Dictionary<T, FsmStateBase<T>> allStates;

        public IFsmState<T> GetState(T stateType)
        {
            return allStates[stateType];
        }

        public T GetCurrentStateType()
        {
            if(currentState != null)
            {
                return currentState.GetStateType();
            }
            else
            {
                return default(T);
            }
        }

        public virtual void SwitchState(T stateType)
        {
            if (currentState != null && currentState.GetStateType().Equals(stateType))
            {
                return;
            }
            if (currentState != null)
            {
                currentState.Leave();
            }
            currentState = allStates[stateType];
            currentState.Enter();
        }

		public virtual void UpdateState()
        {
            if(currentState != null)
            {
				currentState.Update();
            }
        }

    }
    
	

