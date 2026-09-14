public enum SplitState
{
    None, //not doing anything waiting
    Running, //split is actively updating
    Complete, //finalized time is set
    Paused,

}