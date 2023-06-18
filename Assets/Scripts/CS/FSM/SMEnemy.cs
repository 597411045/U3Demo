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

        public TransToAttack IfInAttackRange;
        public TransToIdle IfOutAttackRange;

        public SMEnemy() 
        {
            SIdle = new StateIdle();
            SMove = new StateMove();
            SAttack = new StateAttack();
            SDead = new StateDead();

            
            IfInAttackRange = new TransToAttack("IfInAttackRange", SAttack, 5);
            IfInChaseRange = new TransToMove("IfInChaseRange",SMove,10);
            IfNeedWait= new TransToMove("IfNeedWait",null,20);
            IfNeedGoToSomewhere = new TransToMove("IfNeedGoToSomewhere", SMove, 30);

            IfOutChaseRange = new TransToIdle("IfOutChaseRange",SIdle,10);
            IfReachDestination = new TransToIdle("IfReachDestination",SIdle,20);

            IfOutAttackRange = new TransToIdle("IfOutAttackRange", SIdle, 0);
            
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
            SIdle.AddTransition(IfInAttackRange);
            
            SMove.AddTransition(IfOutChaseRange);
            SMove.AddTransition(IfReachDestination);
            SMove.AddTransition(IfInAttackRange);
            
            SAttack.AddTransition(IfOutAttackRange);
        }
    }
}