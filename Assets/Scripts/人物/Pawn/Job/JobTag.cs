using System;

namespace ChenChen_AI
{
    public enum JobTag : byte
    {
        /// <summary>
        /// 通用类型的工作
        /// </summary>
        Misc,
        /// <summary>
        /// 其他类型的工作
        /// </summary>
        MiscWork,
        /// <summary>
        /// 野外工作
        /// </summary>
        Fieldwork,
        /// <summary>
        /// 闲置状态
        /// </summary>
        Idle,
        /// <summary>
        /// 在心理状态中的工作
        /// </summary>
        InMentalState,
        /// <summary>
        /// 满足需求的工作
        /// </summary>
        SatisfyingNeeds,
        /// <summary>
        /// 已被征召的命令
        /// </summary>
        DraftedOrder,
        /// <summary>
        /// 未指定的领主职责
        /// </summary>
        UnspecifiedLordDuty,
        /// <summary>
        /// 等待其他人完成收集物品
        /// </summary>
        WaitingForOthersToFinishGatheringItems,
        /// <summary>
        /// 躺在床上
        /// </summary>
        TuckedIntoBed,
        /// <summary>
        /// 因医疗原因休息
        /// </summary>
        RestingForMedicalReasons,
        /// <summary>
        /// 更换服装
        /// </summary>
        ChangingApparel,
        /// <summary>
        /// 逃离
        /// </summary>
        Escaping,
        /// <summary>
        /// 加入商队
        /// </summary>
        JoiningCaravan,
        /// <summary>
        /// 训练动物行为
        /// </summary>
        TrainedAnimalBehavior,
        /// <summary>
        /// 卸载自己的库存
        /// </summary>
        UnloadingOwnInventory
    }
}
