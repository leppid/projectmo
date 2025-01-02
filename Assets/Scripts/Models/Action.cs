namespace ProjectModels
{
    public class ActionData
    {
        public string type () => location != null ? "location" : battle != null ? "battle" : null;
        public bool isBattle() => type() == "battle";
        public bool isLocation() => type() == "location";
        public string buttonText() => location != null ? "Enter" : battle != null ? "Attack" : null;
        public LocationData location;
        public BattleData battle;
    }
}