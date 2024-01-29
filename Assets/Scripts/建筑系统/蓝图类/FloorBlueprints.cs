using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ����ϵͳ
{
    public class FloorBlueprints : BuildingBlueprintBase
    {
        /*  
        *  Placed�������õ�Ŀ��㣬Ȼ����ӵ����������
        *  Build�������ڼ��ٹ�����
        *  Complete�����������ʱ��������Ϊ0��������ʱ��״̬
        *  Cancel�������ڵ�ȡ������ʱ��ɾ����ͼ��Object
        */
        public override void Placed()
        {
            
            // ��ɰ�͸������ʾ��δ���
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = new Color(1, 1, 1, 0.5f);

            // ��ӵ���������ϵͳ��
            BuildingSystemManager.Instance.AddTask(this);
        }

        public override void Build(float thisWorkload)
        {           
            _workloadRemainder -= thisWorkload;
            Debug.Log("���ڽ���,ʣ��" + _workloadRemainder);
        }


        public override void Complete()
        {
            
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = new Color(1, 1, 1, 1f);

            BuildingSystemManager.Instance.CompleteTask(this);
        }

        public override void Cancel()
        {
            
            Destroy(this.gameObject);
            BuildingSystemManager.Instance.CanelTask(this);
        }

    }
}