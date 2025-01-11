namespace ProjectModels
{
    public class PlayerData
    {
        public string id;
        public string login;
        public string displayName;
        public string location;
        public string position;
        public string token;
        public int bagPages;
        public bool isGuest = false;
    }

    public class SessionParams
    {
        public string login;
        public string password;
    }

    public class PlayerParams
    {
        public string location;
        public string position;
        public ItemData[] inventory;
    }
}