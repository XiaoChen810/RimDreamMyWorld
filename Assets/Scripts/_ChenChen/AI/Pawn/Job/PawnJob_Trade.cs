using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Trade : PawnJob
    {
        //�������ʱ��
        private readonly static float tick = 120;

        private Animal animal;
        private float tradeDuration = 5;
        private float timer = 0;

        public PawnJob_Trade(Pawn pawn, GameObject target) : base(pawn, tick, new TargetPtr(target))
        {
            if(!target.TryGetComponent<Animal>(out animal))
            {
                Debug.LogError("���󣡴����������ȷ");
            }
        }

        public override bool OnEnter()
        {
            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            //�߼�
            if (animal == null)
            {
                DebugLogDescription = "��Ӧ�������󣬸�Ŀ�궯�����������";
                return false;
            }

            if (!animal.WaitToTrade)
            {
                DebugLogDescription = "�ö��ﲢ����Ҫ�Լ�ȥѱ��";
                return false;
            }

            if (!pawn.MoveController.GoToHere(target.GameObject,endReachedDistance: pawn.WorkRange))
            {
                DebugLogDescription = "�޷��ƶ���Ŀ��";
                return false;
            }

            animal.Trade();

            pawn.JobToDo(target.GameObject);
            this.Description = "ǰ��ѱ��" + target.GameObject.name;
            
            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            //�߼�
            if (target.GameObject == null)
            {
                return StateType.Failed;
            }

            if (pawn.MoveController.ReachDestination)
            {
                pawn.JobDoing();
                timer += Time.deltaTime;
                pawn.ChangeMyBar (1 - timer / tradeDuration);

                if(timer >= tradeDuration)
                {
                    // �����Ƿ�ɹ�ѱ�����˳�
                    animal.CompleteTrade();
                    return StateType.Success;
                }
            }
            return StateType.Doing;
        }

        public override void OnExit()
        {
            base.OnExit();

            //�߼� 
            pawn.ChangeMyBar(0);
            animal.StopTrade();
        }

        public override void OnInterrupt()
        {
            OnExit();
        }
    }
}