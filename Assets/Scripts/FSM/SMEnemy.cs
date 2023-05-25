using UnityEngine;

namespace FSM
{
    public class SMEnemy : CSMachine
    {
        public float waitTimer;
        public Vector3 moveDestination;
        public Transform attackTarget;

        public StateIdle SIdle;
        public StateMove SMove;
        public StateAttack SAttack;
        public StateDead SDead;

        public TransToMove IfInChaseRange;
        public TransToMove IfNeedWait;
        public TransToMove IfNeedGoToSomewhere;

        public TransToIdle IfOutChaseRange;
        public TransToIdle IfReachDestination;

        public SMEnemy() 
        {
            SIdle = new StateIdle();
            SMove = new StateMove();
            SAttack = new StateAttack();
            SDead = new StateDead();

            IfInChaseRange = new TransToMove("IfInChaseRange",SMove,0);
            IfNeedWait= new TransToMove("IfNeedWait",null,1);
            IfNeedGoToSomewhere= new TransToMove("IfNeedGoToSomewhere",SMove,2);
            IfOutChaseRange = new TransToIdle("IfOutChaseRange",SIdle,0);
            IfReachDestination = new TransToIdle("IfReachDestination",SIdle,1);
            
            BuildSM();
        }

        public void BuildSM()
        {
            this.AddState(SIdle);
            this.AddState(SMove);
            this.AddState(SAttack);
            this.AddState(SDead);

            SIdle.AddTransition(IfInChaseRange);
            SIdle.AddTransition(IfNeedWait);
            SIdle.AddTransition(IfNeedGoToSomewhere);
            
            SMove.AddTransition(IfOutChaseRange);
            SMove.AddTransition(IfReachDestination);
        }
    }
}