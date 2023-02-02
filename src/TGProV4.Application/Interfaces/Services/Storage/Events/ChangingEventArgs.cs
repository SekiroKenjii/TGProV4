namespace TGProV4.Application.Interfaces.Services.Storage.Events;

public class ChangingEventArgs : ChangedEventArgs
{
    public bool Cancel { get; set; }
}
