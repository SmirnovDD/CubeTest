public enum ObjectToPlaceType
{
    None,
    Ground = 10,
    
    BUILDINGS = 100,
    SmallCubeWall,
    SmallCubeFloor,
    SmallCubeStairs,
    Door,
    
    СOMBAT = 1000,
    Turret,
    
    NON_COMBAT //RENAME TO SOMETHING REASONABLE, used to not destroy rigidbody on turret so its sensor will detect enemies onTriggerEnter
    
}