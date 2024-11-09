public class OneWayObject_Player : OneWayObject
{
    public override bool CanWalkOnOneWay => player.CanWalkOnOneWay;

    private Player player;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }
}
