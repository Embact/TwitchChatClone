namespace SignalR.Utility
{
    public enum Role
    {
        Broadcaster,
        Moderator,
        Vip,
        Prime,
        Verified
    }

    public enum MessageState
    {
        Sent,
        Waiting,
        Deleted,
        Self
    }

    public enum ActionMethod
    {
        User,
        Message,
        Team
    }

    public enum ActionUser
    {
        EditUser,
        BanUser,
        TimeoutUser
    }
    public enum ActionMessage
    {
        EditMessage,
        DeleteMessage
    }
}
