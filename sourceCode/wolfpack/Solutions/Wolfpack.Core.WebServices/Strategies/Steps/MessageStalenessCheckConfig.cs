namespace Wolfpack.Core.WebServices.Strategies.Steps
{
    public class MessageStalenessCheckConfig
    {
        public int MaxAgeInMinutes { get; set; }

        public MessageStalenessCheckConfig()
        {
            MaxAgeInMinutes = 5;
        }
    }
}