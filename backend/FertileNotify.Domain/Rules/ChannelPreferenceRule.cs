namespace FertileNotify.Domain.Rules
{
    public static class ChannelPreferenceRule
    {
        public const int MaxChannelsPerUser = 3;

        public static bool CanAddChannel(int currentChannelCount)
        {
            return currentChannelCount < MaxChannelsPerUser;
        }
    }
}