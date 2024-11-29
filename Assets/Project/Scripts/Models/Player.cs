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
        public Meta meta;

        public class Meta
        {
            public string token;
        }
    }

    public class PlayerSessionParams
    {
        public string login;
        public string password;
    }

    public class PlayerLocationParams
    {
        public string location;
        public string position;
    }
}