using CommunityToolkit.Mvvm.Messaging.Messages;
using SampleMauiMvvmApp.Models;

namespace SampleMauiMvvmApp.Messages
{
    public class CustomerUpdateMessage : ValueChangedMessage<Customer>
    {
        public CustomerUpdateMessage(Customer value) : base(value)
        {

        }
    }
}
