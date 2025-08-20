# FTOptixNetPlugin.Extensions

这是为 FTOptix 编写的 .Net 组件扩展库，便于开发者迅速解决问题。

FTOptix Version >= 1.6

包括:

- DateTime
    - GetWeekFirstDaySun
    - GetWeekFirstDayMon
    - GetWeekLastDaySat
    - GetWeekLastDaySun
    - GetMonthLastDay
    - GetMonthFirstDay
- LogHelper
    - GetLogFilePath
- ModelEventObserver
    - Event ： Added / Removed
- ResourceUri
    - ToResourceUri
- Store
    - GetStoreType
    - TableExist
    - Query
    - InsertOneRow
    - Insert
    - ExecuteSql
- UANode
    - RegisterAddAndRemoveObserver
    - GetTypeNodeId
    - GetVariableValue
    - SetVariableValue
    - ClearAll
    - GetVariableValue
    - DeepClone
    - DeepCopy
    - GetCurrentProjectBrowsePath
    - IsInStartedSubtree
    - JsonSerialize
    - JsonDeserialize
- UAVariable
    - AddDynamicLinkToVariableBit
    - AddDynamicLinkToArrayElement
    - HasDynamicLink
    - HasConverter

