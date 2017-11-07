using System;
using System.Collections.Generic;


    public interface IFsmState<T>
    {
        T GetStateType();
        void Enter();
        void Leave();

        void Update();

    }

    public class FsmStateBase<T> : IFsmState<T>
    {
        protected readonly T stateType;
        protected readonly FsmControllerBase<T> controller;


        public T GetStateType()
        {
            return stateType;
        }

        public virtual void Enter()
        {

        }

        public FsmStateBase(T stateType, FsmControllerBase<T> controller)
        {
            this.stateType = stateType;
            this.controller = controller;
        }

        public virtual void Leave()
        {

        }

        public virtual void Update()
        {

        }
    }

